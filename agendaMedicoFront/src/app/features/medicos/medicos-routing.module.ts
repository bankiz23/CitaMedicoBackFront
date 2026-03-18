import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MedicosListComponent } from './pages/medicos-list/medicos-list.component';

const routes: Routes = [
  {
    path: '',
    component: MedicosListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MedicosRoutingModule {}
