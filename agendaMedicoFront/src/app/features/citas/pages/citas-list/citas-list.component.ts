import { ChangeDetectorRef, Component, OnInit, NgZone, ApplicationRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MedicosService } from '../../../../core/services/medicos.service';
import { PacientesService } from '../../../../core/services/pacientes.service';
import { CitasService } from '../../../../core/services/citas.service';
import { Medico } from '../../../../core/interfaces/medico.interface';
import { Paciente } from '../../../../core/interfaces/paciente.interface';
import {
  AgendarCitaRequest,
  Cita,
  CancelarCitaRequest,
} from '../../../../core/interfaces/cita.interface';
import { AlertService } from '../../../../core/services/alert.service';
@Component({
  selector: 'app-citas-list',
  standalone: false,
  templateUrl: './citas-list.component.html',
  styleUrls: ['./citas-list.component.css'],
})
export class CitasListComponent implements OnInit {
  medicos: Medico[] = [];
  pacientes: Paciente[] = [];

  citaForm: FormGroup;
  cancelarForm: FormGroup;
  buscarForm: FormGroup;

  citaConsultada: Cita | null = null;
  cargando = false;

  visibleCancelarDialog = false;
  mensajeResultado = '';
  sugerencias: string[] = [];
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
    private pacientesService: PacientesService,
    private citasService: CitasService,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone,
    private appRef: ApplicationRef,
    private alertService: AlertService,
  ) {
    this.citaForm = this.fb.group({
      medicoId: [null, Validators.required],
      pacienteId: [null, Validators.required],
      fecha: ['', Validators.required],
      hora: ['', Validators.required],
      motivo: ['', [Validators.required, Validators.minLength(3)]],
    });

    this.cancelarForm = this.fb.group({
      motivoCancelacion: ['', [Validators.required, Validators.minLength(3)]],
    });

    this.buscarForm = this.fb.group({
      citaId: [null, Validators.required],
    });
  }

  ngOnInit(): void {
    this.obtenerMedicos();
    this.obtenerPacientes();
  }

  obtenerMedicos(): void {
    this.medicosService.getAll().subscribe({
      next: (response) => {
        this.medicos = response;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al obtener médicos', error);
      },
    });
  }

  obtenerPacientes(): void {
    this.pacientesService.getAll().subscribe({
      next: (response) => {
        this.pacientes = response;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al obtener pacientes', error);
      },
    });
  }

  agendarCita(): void {
    if (this.citaForm.invalid) {
      this.citaForm.markAllAsTouched();
      return;
    }

    const fecha = this.citaForm.value.fecha;
    const hora = this.citaForm.value.hora;

    const payload: AgendarCitaRequest = {
      medicoId: Number(this.citaForm.value.medicoId),
      pacienteId: Number(this.citaForm.value.pacienteId),
      fechaHoraInicio: `${fecha}T${hora}:00`,
      motivo: this.citaForm.value.motivo,
    };

    this.cargando = true;
    this.mensajeResultado = '';
    this.sugerencias = [];

    this.citasService.agendar(payload).subscribe({
      next: (response) => {
        this.ngZone.run(() => {
          this.cargando = false;
          this.mensajeResultado = response.mensaje;
          this.sugerencias = response.sugerencias ?? [];

          if (response.citaId) {
            this.consultarCita(response.citaId);
          }

          this.citaForm.reset();
          this.refrescarVista();
          this.cdr.detectChanges();

          this.alertService.toastSuccess('Cita agendada correctamente');
        });
      },
      error: (error) => {
        this.ngZone.run(() => {
          this.cargando = false;

          const body = error?.error ?? {};
          this.alertService.warning('Horario no disponible', this.mensajeResultado);
          this.mensajeResultado =
            body.mensaje ||
            body.Mensaje ||
            error?.message ||
            'Ocurrió un error al agendar la cita.';

          this.sugerencias = body.sugerencias || body.Sugerencias || [];
          this.refrescarVista();
          this.cdr.detectChanges();
        });
      },
    });
  }

  buscarCita(): void {
    if (this.buscarForm.invalid) {
      this.buscarForm.markAllAsTouched();
      return;
    }

    const id = Number(this.buscarForm.value.citaId);
    this.consultarCita(id);
  }

  consultarCita(id: number): void {
    this.cargando = true;
    // this.citaConsultada = null;

    this.citasService.getById(id).subscribe({
      next: (response) => {
        this.citaConsultada = response;
        console.log('Cita consultada:', response);
        this.cargando = false;
        this.refrescarVista();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al consultar cita', error);
        this.citaConsultada = null;
        this.cargando = false;
        this.refrescarVista();

        this.cdr.detectChanges();
      },
    });
  }

  abrirCancelarDialog(): void {
    if (!this.citaConsultada) return;

    this.cancelarForm.reset();
    this.visibleCancelarDialog = true;
  }

  cancelarCita(): void {
    if (!this.citaConsultada) return;

    if (this.cancelarForm.invalid) {
      this.cancelarForm.markAllAsTouched();
      return;
    }

    const payload: CancelarCitaRequest = {
      motivoCancelacion: this.cancelarForm.value.motivoCancelacion,
    };

    this.citasService.cancelar(this.citaConsultada.id, payload).subscribe({
      next: () => {
        this.visibleCancelarDialog = false;
        this.mensajeResultado = 'Cita cancelada correctamente.';
        this.consultarCita(this.citaConsultada!.id);
        this.refrescarVista();

        this.alertService.toastSuccess('Cita cancelada correctamente');
      },
      error: (error) => {
        console.error('Error al cancelar cita', error);
        this.alertService.error('Error', error?.error?.mensaje || 'No se pudo cancelar la cita.');
      },
    });
  }

  cerrarCancelarDialog(): void {
    this.visibleCancelarDialog = false;
    this.cancelarForm.reset();
  }

  campoInvalido(form: FormGroup, campo: string): boolean {
    const control = form.get(campo);
    return !!control && control.invalid && (control.touched || control.dirty);
  }
}
