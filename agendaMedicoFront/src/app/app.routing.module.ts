import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./features/dashboard/dashboard.module').then((m) => m.DashboardModule),
  },
  {
    path: 'medicos',
    loadChildren: () => import('./features/medicos/medicos.module').then((m) => m.MedicosModule),
  },
  {
    path: 'pacientes',
    loadChildren: () =>
      import('./features/pacientes/pacientes.module').then((m) => m.PacientesModule),
  },
  {
    path: 'citas',
    loadChildren: () => import('./features/citas/citas.module').then((m) => m.CitasModule),
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
