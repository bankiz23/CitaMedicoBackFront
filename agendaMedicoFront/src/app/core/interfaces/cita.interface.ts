export interface Cita {
  id: number;
  medicoId: number;
  medico: string;
  pacienteId: number;
  paciente: string;
  fechaHoraInicio: string;
  fechaHoraFin: string;
  motivo: string;
  estado: string;
}

export interface AgendarCitaRequest {
  medicoId: number;
  pacienteId: number;
  fechaHoraInicio: string;
  motivo: string;
}

export interface CancelarCitaRequest {
  motivoCancelacion: string;
}

export interface AgendarCitaResponse {
  exito: boolean;
  mensaje: string;
  citaId?: number;
  alertaCancelaciones: boolean;
  sugerencias: string[];
}
