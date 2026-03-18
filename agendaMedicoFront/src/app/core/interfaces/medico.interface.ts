export interface Medico {
  id: number;
  nombre: string;
  fk_IdEspecialidad: number;
  especialidad: string;
  activo: boolean;
}

export interface MedicoCreate {
  nombre: string;
  fk_IdEspecialidad: number;
}
export interface HorarioMedico {
  id: number|null;
  medicoId: number;
  diaSemana: number;
  horaInicio: string;
  horaFin: string;
}
export interface HorarioMedicoCreate {
  diaSemana: number;
  horaInicio: string;
  horaFin: string;
}