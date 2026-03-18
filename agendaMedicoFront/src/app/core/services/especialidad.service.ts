export interface Especialidad {
  id: number;
  nombreEspecialidad: string;
}
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EspecialidadesService {
  private readonly baseUrl = `${environment.apiUrl}/especialidades`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Especialidad[]> {
    return this.http.get<Especialidad[]>(this.baseUrl);
  }

  getById(id: number): Observable<Especialidad> {
    return this.http.get<Especialidad>(`${this.baseUrl}/${id}`);
  }
}