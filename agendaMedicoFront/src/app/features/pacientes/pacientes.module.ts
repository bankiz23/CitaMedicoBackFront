import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PacientesRoutingModule } from './pacientes-routing.module';
import { PacientesListComponent } from './pages/pacientes-list/pacientes-list.component';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';

@NgModule({
  declarations: [PacientesListComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PacientesRoutingModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
  ],
})
export class PacientesModule {}
