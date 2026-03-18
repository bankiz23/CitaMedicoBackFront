import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PacientesService } from '../../../../core/services/pacientes.service';
import { Paciente, PacienteCreate } from '../../../../core/interfaces/paciente.interface';
import { AlertService } from '../../../../core/services/alert.service';
@Component({
  selector: 'app-pacientes-list',
  standalone: false,
  templateUrl: './pacientes-list.component.html',
  styleUrls: ['./pacientes-list.component.css'],
})
export class PacientesListComponent implements OnInit {
  visibleNuevoPaciente = false;
  cargando = false;

  pacientes: Paciente[] = [];
  pacientesFiltrados: Paciente[] = [];
  filtroTexto = '';

  pacienteForm: FormGroup;
  modoEdicion = false;
  pacienteEditandoId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private pacientesService: PacientesService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService,
  ) {
    this.pacienteForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      fechaNacimiento: ['', Validators.required],
      telefono: ['', [Validators.required, Validators.minLength(10)]],
      correoElectronico: ['', [Validators.required, Validators.email]],
    });
  }

  ngOnInit(): void {
    this.obtenerPacientes();
  }

  obtenerPacientes(): void {
    this.cargando = true;

    this.pacientesService.getAll().subscribe({
      next: (response) => {
        this.pacientes = [...response];
        this.aplicarFiltro();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al obtener pacientes', error);
        this.cargando = false;
        this.cdr.detectChanges();
      },
    });
  }

  aplicarFiltro(): void {
    const texto = this.filtroTexto.trim().toLowerCase();

    if (!texto) {
      this.pacientesFiltrados = [...this.pacientes];
      return;
    }

    this.pacientesFiltrados = this.pacientes.filter(
      (p) =>
        p.nombre.toLowerCase().includes(texto) ||
        p.telefono.toLowerCase().includes(texto) ||
        p.correoElectronico.toLowerCase().includes(texto),
    );
  }

  abrirNuevoPaciente(): void {
    this.modoEdicion = false;
    this.pacienteEditandoId = null;
    this.pacienteForm.reset();
    this.visibleNuevoPaciente = true;
  }

  editarPaciente(paciente: Paciente): void {
    this.modoEdicion = true;
    this.pacienteEditandoId = paciente.id;

    this.pacienteForm.patchValue({
      nombre: paciente.nombre,
      fechaNacimiento: paciente.fechaNacimiento?.substring(0, 10),
      telefono: paciente.telefono,
      correoElectronico: paciente.correoElectronico,
    });

    this.visibleNuevoPaciente = true;
  }

  guardarPaciente(): void {
    if (this.pacienteForm.invalid) {
      this.pacienteForm.markAllAsTouched();
      return;
    }

    const payload: PacienteCreate = {
      nombre: this.pacienteForm.value.nombre,
      fechaNacimiento: this.pacienteForm.value.fechaNacimiento,
      telefono: this.pacienteForm.value.telefono,
      correoElectronico: this.pacienteForm.value.correoElectronico,
    };

    if (this.modoEdicion && this.pacienteEditandoId !== null) {
      this.pacientesService.update(this.pacienteEditandoId, payload).subscribe({
        next: () => {
          this.obtenerPacientes();
          this.cerrarDialog();
          this.alertService.toastSuccess('Paciente actualizado correctamente');
        },
        error: (error) => {
          this.alertService.error(
            'Error',
            error?.error?.mensaje || 'No se pudo actualizar el paciente.',
          );
          console.error('Error al actualizar paciente', error);
        },
      });
      return;
    }

    this.pacientesService.create(payload).subscribe({
      next: () => {
        this.obtenerPacientes();
        this.cerrarDialog();
        this.alertService.toastSuccess('Paciente creado correctamente');
      },
      error: (error) => {
        this.alertService.error(
          'Error',
          error?.error?.mensaje || 'No se pudo guardar el paciente.',
        );
        console.error('Error al guardar paciente', error);
      },
    });
  }

  async eliminarPaciente(id: number): Promise<void> {
    const result = await this.alertService.confirm(
      '¿Eliminar paciente?',
      'Esta acción eliminará el paciente seleccionado.',
      'Sí, eliminar',
    );

    if (!result.isConfirmed) return;

    this.pacientesService.delete(id).subscribe({
      next: () => {
        this.pacientes = this.pacientes.filter((x) => x.id !== id);
        this.aplicarFiltro();
        this.cdr.detectChanges();

        this.alertService.toastSuccess('Paciente eliminado correctamente');
      },
      error: (error) => {
        console.error('Error al eliminar paciente', error);
        this.alertService.error(
          'Error',
          error?.error?.mensaje || 'No se pudo eliminar el paciente.',
        );
      },
    });
  }

  cerrarDialog(): void {
    this.visibleNuevoPaciente = false;
    this.modoEdicion = false;
    this.pacienteEditandoId = null;
    this.pacienteForm.reset();
    this.cdr.detectChanges();
  }

  campoInvalido(nombreCampo: string): boolean {
    const control = this.pacienteForm.get(nombreCampo);
    return !!control && control.invalid && (control.touched || control.dirty);
  }
}
