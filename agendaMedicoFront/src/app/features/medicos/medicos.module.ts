import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MedicosRoutingModule } from './medicos-routing.module';
import { MedicosListComponent } from './pages/medicos-list/medicos-list.component';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { DialogModule } from 'primeng/dialog';

@NgModule({
  declarations: [
    MedicosListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MedicosRoutingModule,
    TableModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    ToolbarModule,
    DialogModule
  ]
})
export class MedicosModule { }