import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

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

@Injectable({
  providedIn: 'root'
})
export class PacientesService {
  private readonly baseUrl = `${environment.apiUrl}/pacientes`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Paciente[]> {
    return this.http.get<Paciente[]>(this.baseUrl);
  }

  getById(id: number): Observable<Paciente> {
    return this.http.get<Paciente>(`${this.baseUrl}/${id}`);
  }

  create(payload: PacienteCreate): Observable<Paciente> {
    return this.http.post<Paciente>(this.baseUrl, payload);
  }

  update(id: number, payload: PacienteCreate): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  getHistorial(id: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/${id}/historial`);
  }
}