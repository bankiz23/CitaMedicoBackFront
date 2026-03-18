import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HorarioMedico, HorarioMedicoCreate, Medico, MedicoCreate } from '../interfaces/medico.interface';


@Injectable({
  providedIn: 'root',
})
export class MedicosService {
  private baseUrl = environment.apiUrl + '/medicos';

  constructor(private http: HttpClient) {}


  getAll(): Observable<Medico[]> {
    return this.http.get<Medico[]>(this.baseUrl);
  }

  getById(id: number): Observable<Medico> {
    return this.http.get<Medico>(`${this.baseUrl}/${id}`);
  }

  create(payload: MedicoCreate): Observable<Medico> {
    return this.http.post<Medico>(this.baseUrl, payload);
  }

  update(id: number, payload: MedicoCreate): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  getHorarios(medicoId: number): Observable<HorarioMedico[]> {
    return this.http.get<HorarioMedico[]>(`${this.baseUrl}/${medicoId}/horarios`);
  }

  addHorario(medicoId: number, payload: HorarioMedicoCreate): Observable<HorarioMedico> {
    return this.http.post<HorarioMedico>(`${this.baseUrl}/${medicoId}/horarios`, payload);
  }

  updateHorario(medicoId: number, horarioId: number, payload: HorarioMedicoCreate): Observable<any> {
    return this.http.put(`${this.baseUrl}/${medicoId}/horarios/${horarioId}`, payload);
  }

  deleteHorario(medicoId: number, horarioId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${medicoId}/horarios/${horarioId}`);
  }
}
