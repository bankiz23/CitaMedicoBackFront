import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CitasRoutingModule } from './citas-routing.module';
import { CitasListComponent } from './pages/citas-list/citas-list.component';

import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';

@NgModule({
  declarations: [CitasListComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    CitasRoutingModule,
    ButtonModule,
    DialogModule,
  ],
})
export class CitasModule {}
