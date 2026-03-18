export interface Paciente {
  id: number;
  nombre: string;
  fechaNacimiento: string;
  telefono: string;
  correoElectronico: string;
  activo: boolean;
}

export interface PacienteCreate {
  nombre: string;
  fechaNacimiento: string;
  telefono: string;
  correoElectronico: string;
}
