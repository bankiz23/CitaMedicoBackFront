import { ChangeDetectorRef, Component, OnInit, NgZone, ApplicationRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MedicosService } from '../../../../core/services/medicos.service';
import {
  HorarioMedico,
  HorarioMedicoCreate,
  Medico,
  MedicoCreate,
} from '../../../../core/interfaces/medico.interface';
import { EspecialidadesService } from '../../../../core/services/especialidad.service';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { AgendaService, AgendaMedicoItem } from '../../../../core/services/agenda.service';
import { AlertService } from '../../../../core/services/alert.service';
export const rangoHorarioValidoValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const horaInicio = control.get('horaInicio')?.value;
  const horaFin = control.get('horaFin')?.value;

  if (!horaInicio || !horaFin) {
    return null;
  }

  return horaInicio < horaFin ? null : { rangoHorarioInvalido: true };
};
@Component({
  selector: 'app-medicos-list',
  standalone: false,
  templateUrl: './medicos-list.component.html',
  styleUrls: ['./medicos-list.component.css'],
})
export class MedicosListComponent implements OnInit {
  citasMedicoDialogVisible = false;
  citasMedico: AgendaMedicoItem[] = [];
  fechaAgendaMedico = '';
  cargandoCitasMedico = false;

  visibleNuevoMedico = false;
  cargando = false;
  horariosDialogVisible = false;
  medicoSeleccionado: Medico | null = null;
  horariosMedico: HorarioMedico[] = [];

  horarioForm: FormGroup;
  modoEdicionHorario = false;
  horarioEditandoId: number | null = null;
  medicos: Medico[] = [];
  medicosFiltrados: Medico[] = [];

  filtroNombre = '';

  medicoForm: FormGroup;
  modoEdicion = false;
  medicoEditandoId: number | null = null;
  especialidades: any;
  diasSemana = [
    { id: 1, nombre: 'Lunes' },
    { id: 2, nombre: 'Martes' },
    { id: 3, nombre: 'Miércoles' },
    { id: 4, nombre: 'Jueves' },
    { id: 5, nombre: 'Viernes' },
    { id: 6, nombre: 'Sábado' },
    { id: 0, nombre: 'Domingo' },
  ];
  private refrescarVista(): void {
    this.ngZone.run(() => {
      this.cdr.markForCheck();
      this.cdr.detectChanges();
      this.appRef.tick();
    });
  }
  constructor(
    private fb: FormBuilder,
    private medicosService: MedicosService,
    private cdr: ChangeDetectorRef,
    private especialidadesService: EspecialidadesService,
    private agendaService: AgendaService,
    private ngZone: NgZone,
    private appRef: ApplicationRef,
    private alertService: AlertService,
  ) {
    this.medicoForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      fk_IdEspecialidad: [null, Validators.required],
    });
    this.horarioForm = this.fb.group(
      {
        diaSemana: [null, Validators.required],
        horaInicio: ['', Validators.required],
        horaFin: ['', Validators.required],
      },
      { validators: rangoHorarioValidoValidator },
    );
  }

  ngOnInit(): void {
    this.obtenerEspecialidades();
    this.obtenerMedicos();
  }
  obtenerEspecialidades(): void {
    this.especialidadesService.getAll().subscribe({
      next: (response) => {
        this.especialidades = response;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al obtener especialidades', error);
      },
    });
  }
  obtenerMedicos(): void {
    this.cargando = true;

    this.medicosService.getAll().subscribe({
      next: (response) => {
        this.medicos = [...response];
        this.aplicarFiltro();
        this.cargando = false;
        this.cdr.detectChanges();
        this.refrescarVista();
      },
      error: (error) => {
        console.error('Error al obtener médicos', error);
        this.cargando = false;
        this.cdr.detectChanges();
        this.refrescarVista();
      },
    });
  }

  aplicarFiltro(): void {
    const texto = this.filtroNombre.trim().toLowerCase();

    if (!texto) {
      this.medicosFiltrados = [...this.medicos];
      return;
    }

    this.medicosFiltrados = this.medicos.filter(
      (m) => m.nombre.toLowerCase().includes(texto) || m.especialidad.toLowerCase().includes(texto),
    );
  }

  abrirNuevoMedico(): void {
    this.modoEdicion = false;
    this.medicoEditandoId = null;
    this.medicoForm.reset();
    this.visibleNuevoMedico = true;
  }

  editarMedico(medico: Medico): void {
    this.modoEdicion = true;
    this.medicoEditandoId = medico.id;

    this.medicoForm.patchValue({
      nombre: medico.nombre,
      especialidad: medico.especialidad,
    });

    this.visibleNuevoMedico = true;
  }

  guardarMedico(): void {
    if (this.medicoForm.invalid) {
      this.medicoForm.markAllAsTouched();
      return;
    }

    const payload: MedicoCreate = {
      nombre: this.medicoForm.value.nombre,
      fk_IdEspecialidad: this.medicoForm.value.fk_IdEspecialidad,
    };

    if (this.modoEdicion && this.medicoEditandoId !== null) {
      this.medicosService.update(this.medicoEditandoId, payload).subscribe({
        next: () => {
          const index = this.medicos.findIndex((x) => x.id === this.medicoEditandoId);

          if (index !== -1) {
            this.medicos[index] = {
              ...this.medicos[index],
              nombre: payload.nombre,
              fk_IdEspecialidad: payload.fk_IdEspecialidad,
            };
            this.medicos = [...this.medicos];
            this.aplicarFiltro();
          }
          this.obtenerMedicos();
          this.cerrarDialog();
          this.refrescarVista();
          this.cdr.detectChanges();
          this.alertService.success(
            'Médico actualizado',
            'Los datos se actualizaron correctamente.',
          );
        },
        error: (error) => {
          this.alertService.error(
            'Error',
            error?.error?.mensaje || 'No se pudo actualizar el médico.',
          );
          console.error('Error al actualizar médico', error);
        },
      });

      return;
    }

    this.medicosService.create(payload).subscribe({
      next: (nuevoMedico) => {
        this.medicos = [...this.medicos, nuevoMedico];
        this.aplicarFiltro();
        this.obtenerMedicos();
        this.cerrarDialog();
        this.refrescarVista();
        this.cdr.detectChanges();
        this.alertService.success('Médico creado', 'El médico se registró correctamente.');
      },
      error: (error) => {
        this.alertService.error('Error', error?.error?.mensaje || 'No se pudo guardar el médico.');
        console.error('Error al guardar médico', error);
      },
    });
  }

  async eliminarMedico(id: number): Promise<void> {
    const result = await this.alertService.confirm(
      '¿Eliminar médico?',
      'Esta acción desactivará el médico.',
      'Sí, eliminar',
    );

    if (!result.isConfirmed) return;

    this.medicosService.delete(id).subscribe({
      next: () => {
        this.medicos = this.medicos.filter((x) => x.id !== id);
        this.aplicarFiltro();
        this.obtenerMedicos();
        this.cdr.detectChanges();

        this.alertService.success('Médico eliminado', 'El médico fue eliminado correctamente.');
      },
      error: (error) => {
        console.error('Error al eliminar médico', error);
        this.alertService.error('Error', error?.error?.mensaje || 'No se pudo eliminar el médico.');
      },
    });
  }
  cerrarDialog(): void {
    this.visibleNuevoMedico = false;
    this.modoEdicion = false;
    this.medicoEditandoId = null;
    this.medicoForm.reset();
    this.cdr.detectChanges();
  }

  campoInvalido(nombreCampo: string): boolean {
    const control = this.medicoForm.get(nombreCampo);
    return !!control && control.invalid && (control.touched || control.dirty);
  }

  abrirHorarios(medico: Medico): void {
    this.medicoSeleccionado = medico;
    this.horariosDialogVisible = true;
    this.horarioForm.reset();
    this.modoEdicionHorario = false;
    this.horarioEditandoId = null;
    this.obtenerHorarios(medico.id);
  }

  obtenerHorarios(medicoId: number): void {
    this.medicosService.getHorarios(medicoId).subscribe({
      next: (response) => {
        this.horariosMedico = response;
        this.refrescarVista();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al obtener horarios', error);
      },
    });
  }

  guardarHorario(): void {
    if (!this.medicoSeleccionado) return;

    if (this.horarioForm.invalid) {
      this.horarioForm.markAllAsTouched();
      return;
    }

    const payload: HorarioMedicoCreate = {
      diaSemana: Number(this.horarioForm.value.diaSemana),
      horaInicio: `${this.horarioForm.value.horaInicio}:00`,
      horaFin: `${this.horarioForm.value.horaFin}:00`,
    };

    if (this.modoEdicionHorario && this.horarioEditandoId !== null) {
      this.medicosService
        .updateHorario(this.medicoSeleccionado.id, this.horarioEditandoId, payload)
        .subscribe({
          next: () => {
            this.obtenerHorarios(this.medicoSeleccionado!.id);
            this.horarioForm.reset();
            this.refrescarVista();
            this.modoEdicionHorario = false;
            this.horarioEditandoId = null;
            this.alertService.toastSuccess('Horario actualizado correctamente');
          },
          error: (error) => {
            this.alertService.error(
              'Error',
              error?.error?.mensaje || 'No se pudo actualizar el horario.',
            );
            console.error('Error al actualizar horario', error);
          },
        });
      return;
    }

    this.medicosService.addHorario(this.medicoSeleccionado.id, payload).subscribe({
      next: () => {
        this.obtenerHorarios(this.medicoSeleccionado!.id);
        this.horarioForm.reset();
        this.alertService.toastSuccess('Horario agregado correctamente');
      },
      error: (error) => {
        this.alertService.error('Error', error?.error?.mensaje || 'No se pudo guardar el horario.');
        console.error('Error al guardar horario', error);
      },
    });
  }

  editarHorario(horario: HorarioMedico): void {
    this.modoEdicionHorario = true;
    this.horarioEditandoId = horario.id;

    this.horarioForm.patchValue({
      diaSemana: horario.diaSemana,
      horaInicio: horario.horaInicio?.substring(0, 5),
      horaFin: horario.horaFin?.substring(0, 5),
    });
  }

  async eliminarHorario(horarioId: number): Promise<void> {
    if (!this.medicoSeleccionado) return;

    const result = await this.alertService.confirm(
      '¿Eliminar horario?',
      'Esta acción eliminará el horario seleccionado.',
      'Sí, eliminar',
    );

    if (!result.isConfirmed) return;

    this.medicosService.deleteHorario(this.medicoSeleccionado.id, horarioId).subscribe({
      next: () => {
        this.obtenerHorarios(this.medicoSeleccionado!.id);
        this.alertService.toastSuccess('Horario eliminado correctamente');
      },
      error: (error) => {
        console.error('Error al eliminar horario', error);
        this.alertService.error(
          'Error',
          error?.error?.mensaje || 'No se pudo eliminar el horario.',
        );
      },
    });
  }

  obtenerNombreDia(dia: number): string {
    const encontrado = this.diasSemana.find((x) => x.id === dia);
    return encontrado ? encontrado.nombre : 'N/D';
  }

  cerrarDialogHorarios(): void {
    this.horariosDialogVisible = false;
    this.medicoSeleccionado = null;
    this.horariosMedico = [];
    this.horarioForm.reset();
    this.modoEdicionHorario = false;
    this.horarioEditandoId = null;
  }
  //#region Citas médico
  abrirCitasMedico(medico: Medico): void {
    this.medicoSeleccionado = medico;
    this.citasMedicoDialogVisible = true;
    this.citasMedico = [];

    const hoy = new Date();
    this.fechaAgendaMedico = hoy.toISOString().split('T')[0];

    this.obtenerCitasMedico();
  }

  obtenerCitasMedico(): void {
    if (!this.medicoSeleccionado || !this.fechaAgendaMedico) return;

    this.cargandoCitasMedico = true;

    this.agendaService.getAgenda(this.medicoSeleccionado.id, this.fechaAgendaMedico).subscribe({
      next: (response) => {
        this.citasMedico = response;
        this.cargandoCitasMedico = false;
        this.refrescarVista();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al obtener citas del médico', error);
        this.citasMedico = [];
        this.cargandoCitasMedico = false;
        this.refrescarVista();
        this.cdr.detectChanges();
      },
    });
  }

  cerrarDialogCitasMedico(): void {
    this.citasMedicoDialogVisible = false;
    this.citasMedico = [];
  }

  //#endregion
}
