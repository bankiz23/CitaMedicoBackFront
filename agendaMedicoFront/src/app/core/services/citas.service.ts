import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AgendarCitaRequest,
  AgendarCitaResponse,
  CancelarCitaRequest,
  Cita,
} from '../interfaces/cita.interface';

@Injectable({
  providedIn: 'root',
})
export class CitasService {
  private readonly baseUrl = `${environment.apiUrl}/citas`;

  constructor(private http: HttpClient) {}

  getById(id: number): Observable<Cita> {
    return this.http.get<Cita>(`${this.baseUrl}/${id}`);
  }

  agendar(payload: AgendarCitaRequest): Observable<AgendarCitaResponse> {
    return this.http.post<AgendarCitaResponse>(`${this.baseUrl}/agendar`, payload);
  }

  cancelar(id: number, payload: CancelarCitaRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}/cancelar`, payload);
  }
}
