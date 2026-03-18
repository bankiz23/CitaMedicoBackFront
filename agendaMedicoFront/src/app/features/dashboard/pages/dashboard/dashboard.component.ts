import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DashboardService } from '../../../../core/services/dashboard.service';
import { DashboardResumen } from '../../../../core/interfaces/dashboard-resumen.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  resumen: DashboardResumen = {
    totalMedicos: 0,
    totalPacientes: 0,
    totalCitasHoy: 0,
    totalCitasCanceladas: 0,
  };

  constructor(
    private dashboardService: DashboardService,
    private cdr: ChangeDetectorRef,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.dashboardService.getResumen().subscribe({
      next: (response) => {
        console.log('Resumen del dashboard:', response);
        this.resumen.totalMedicos = response.totalMedicos;
        this.resumen.totalPacientes = response.totalPacientes;
        this.resumen.totalCitasHoy = response.totalCitasHoy;
        this.resumen.totalCitasCanceladas = response.totalCitasCanceladas;
        console.log('Resumen actualizado en el componente:', this.resumen);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al obtener resumen del dashboard', error);
      },
    });
  }
  irA(ruta: string): void {
    this.router.navigate([ruta]);
  }
}
