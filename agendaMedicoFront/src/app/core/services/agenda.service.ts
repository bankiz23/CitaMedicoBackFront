import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cita } from '../interfaces/cita.interface';

export interface AgendaMedicoItem {
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
@Injectable({
  providedIn: 'root',
})
export class AgendaService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAgenda(medicoId: number, fecha: string): Observable<Cita[]> {
    const params = new HttpParams().set('medicoId', medicoId).set('fecha', fecha);

    return this.http.get<Cita[]>(`${this.apiUrl}/agenda`, { params });
  }

  getDisponibilidad(medicoId: number, fecha: string): Observable<string[]> {
    const params = new HttpParams().set('medicoId', medicoId).set('fecha', fecha);

    return this.http.get<string[]>(`${this.apiUrl}/disponibilidad`, { params });
  }
}
