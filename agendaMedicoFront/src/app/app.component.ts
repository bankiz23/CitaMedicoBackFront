import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  items: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-home', routerLink: '/dashboard' },
    { label: 'Médicos', icon: 'pi pi-user-md', routerLink: '/medicos' },
    { label: 'Pacientes', icon: 'pi pi-users', routerLink: '/pacientes' },
    { label: 'Citas', icon: 'pi pi-calendar', routerLink: '/citas' },
  ];
}
