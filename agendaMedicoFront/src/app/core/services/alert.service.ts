import { Injectable } from '@angular/core';
import Swal, { SweetAlertIcon } from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  success(title: string, text?: string): Promise<any> {
    return Swal.fire({
      icon: 'success',
      title,
      text,
      confirmButtonText: 'Aceptar',
      confirmButtonColor: '#2563eb',
    });
  }

  error(title: string, text?: string): Promise<any> {
    return Swal.fire({
      icon: 'error',
      title,
      text,
      confirmButtonText: 'Aceptar',
      confirmButtonColor: '#dc2626',
    });
  }

  warning(title: string, text?: string): Promise<any> {
    return Swal.fire({
      icon: 'warning',
      title,
      text,
      confirmButtonText: 'Aceptar',
      confirmButtonColor: '#f59e0b',
    });
  }

  info(title: string, text?: string): Promise<any> {
    return Swal.fire({
      icon: 'info',
      title,
      text,
      confirmButtonText: 'Aceptar',
      confirmButtonColor: '#0ea5e9',
    });
  }

  confirm(
    title: string,
    text?: string,
    confirmButtonText: string = 'Sí, continuar',
    cancelButtonText: string = 'Cancelar',
    icon: SweetAlertIcon = 'warning',
  ): Promise<any> {
    return Swal.fire({
      icon,
      title,
      text,
      showCancelButton: true,
      confirmButtonText,
      cancelButtonText,
      confirmButtonColor: '#dc2626',
      cancelButtonColor: '#6b7280',
      reverseButtons: true,
    });
  }

  toastSuccess(title: string): Promise<any> {
    return Swal.fire({
      toast: true,
      position: 'top-end',
      icon: 'success',
      title,
      showConfirmButton: false,
      timer: 2500,
      timerProgressBar: true,
    });
  }

  toastError(title: string): Promise<any> {
    return Swal.fire({
      toast: true,
      position: 'top-end',
      icon: 'error',
      title,
      showConfirmButton: false,
      timer: 2500,
      timerProgressBar: true,
    });
  }
}
