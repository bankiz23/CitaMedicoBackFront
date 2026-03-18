# Agenda Cita

Sistema web para la gestión de médicos, pacientes, horarios y citas médicas, desarrollado con **.NET 8 Web API**, **Angular** y **SQL Server**.

## Descripción general

La solución permite administrar:

- Catálogo de médicos
- Catálogo de pacientes
- Catálogo de especialidades
- Horarios de atención por médico
- Agenda y cancelación de citas médicas
- Consulta de disponibilidad
- Dashboard con indicadores generales

Además, incluye validaciones de negocio importantes como control de traslapes, duración de consulta por especialidad, validación de horarios del médico y alertas por cancelaciones frecuentes.

---

## Tecnologías utilizadas

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Stored Procedures
- xUnit para pruebas unitarias

### Frontend
- Angular
- PrimeNG
- SweetAlert2
- Reactive Forms

---

## Instrucciones para ejecutar el proyecto

### Backend
1. Configurar la cadena de conexión en `appsettings.json`.
2. Restaurar paquetes con `dotnet restore`.
3. Compilar con `dotnet build`.
4. Ejecutar la API con `dotnet run --project API.AgendaCitas`.
5. Abrir Swagger en la URL configurada localmente, por ejemplo: `https://localhost:44351/swagger`.

### Base de datos
1. Crear la base de datos en SQL Server.
2. Ejecutar los scripts de creación de tablas.
3. Ejecutar los scripts de catálogo de especialidades y duración.
4. Ejecutar los procedimientos almacenados.
5. Si se desea, aplicar migraciones con Entity Framework Core.

### Frontend
1. Entrar al proyecto Angular.
2. Ejecutar `npm install`.
3. Configurar `environment.ts` con la URL de la API.
4. Ejecutar `ng serve -o`.

## Estructura de la solución

```text
API.AgendaCitas   -> Proyecto Web API
API.Models        -> Entidades, DbContext y modelos relacionados
API.Services      -> Lógica de negocio, interfaces y DTOs
API.Tests         -> Pruebas unitarias
agenda-medica-front -> Proyecto frontend en Angular
```

---

## Funcionalidades principales

### Médicos
- Alta de médicos
- Edición de médicos
- Eliminación lógica de médicos
- Consulta de listado de médicos
- Administración de horarios por día y rango de horas
- Consulta de citas por médico

### Pacientes
- Alta de pacientes
- Edición de pacientes
- Eliminación lógica de pacientes
- Consulta de listado de pacientes
- Consulta de historial de citas

### Citas
- Agendar cita
- Consultar cita por id
- Cancelar cita con motivo
- Sugerencia de próximos 5 horarios disponibles cuando el horario solicitado no está libre

### Dashboard
- Total de médicos activos
- Total de pacientes activos
- Total de citas del día
- Total de citas canceladas

---

## Reglas de negocio implementadas

- No se pueden agendar citas en fechas u horas pasadas.
- La cita debe estar dentro del horario configurado del médico.
- No se permiten traslapes de citas para un mismo médico.
- La duración de la consulta depende de la especialidad.
- Si un paciente tiene 3 o más cancelaciones en los últimos 30 días, el sistema genera una alerta.
- Si el horario solicitado no está disponible, el sistema sugiere los próximos 5 horarios disponibles.

---

## Estructura de la base de datos

### Descripción general
La base de datos se diseñó para separar catálogos, entidades principales y reglas de agenda.

### Tablas principales
- `Medicos`: almacena la información del médico.
- `Pacientes`: almacena la información del paciente.
- `Citas`: almacena las citas agendadas, canceladas o completadas.
- `HorariosMedico`: almacena los horarios disponibles del médico por día y rango de hora.
- `CatalogoEspecialidad`: catálogo de especialidades médicas.
- `DuracionEspecialidad`: duración en minutos asociada a cada especialidad.

### Relaciones principales
- Un médico pertenece a una especialidad.
- Una especialidad tiene una duración configurada.
- Un médico puede tener varios horarios.
- Un médico puede tener muchas citas.
- Un paciente puede tener muchas citas.
- Cada cita pertenece a un médico y a un paciente.

### Vista lógica simple

```text
CatalogoEspecialidad (1) ---- (N) Medicos
CatalogoEspecialidad (1) ---- (N) DuracionEspecialidad
Medicos (1) ---- (N) HorariosMedico
Medicos (1) ---- (N) Citas
Pacientes (1) ---- (N) Citas
```

## Catálogo de especialidades y duración

Se implementó una estructura basada en catálogo para evitar tener reglas quemadas en código.

### Tablas principales
- `CatalogoEspecialidad`
- `DuracionEspecialidad`

### Duraciones configuradas
- Medicina General = 20 min
- Cardiología = 30 min
- Cirugía = 45 min
- Pediatría = 20 min
- Ginecología = 30 min

---

## Procedimientos almacenados

Se implementaron procedimientos almacenados para consultas específicas.

### Resumen dashboard
- `sp_ResumenDashboard`

Este procedimiento devuelve:
- TotalMedicos
- TotalPacientes
- TotalCitasHoy
- TotalCitasCanceladas

### Agenda del médico por rango de fechas
- `sp_ObtenerAgendaMedicoPorRango`

---

## API REST

### Médicos
- `GET /api/medicos`
- `GET /api/medicos/{id}`
- `POST /api/medicos`
- `PUT /api/medicos/{id}`
- `DELETE /api/medicos/{id}`
- `GET /api/medicos/{medicoId}/horarios`
- `POST /api/medicos/{medicoId}/horarios`
- `PUT /api/medicos/{medicoId}/horarios/{horarioId}`
- `DELETE /api/medicos/{medicoId}/horarios/{horarioId}`

### Pacientes
- `GET /api/pacientes`
- `GET /api/pacientes/{id}`
- `POST /api/pacientes`
- `PUT /api/pacientes/{id}`
- `DELETE /api/pacientes/{id}`
- `GET /api/pacientes/{id}/historial`

### Citas
- `GET /api/citas/{id}`
- `POST /api/citas/agendar`
- `PUT /api/citas/{id}/cancelar`

### Agenda y disponibilidad
- `GET /api/agenda`
- `GET /api/disponibilidad`

### Dashboard
- `GET /api/dashboard/resumen`

---

## Configuración del backend

### 1. Configurar cadena de conexión
En el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=CitaSalud;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Restaurar paquetes
```bash
dotnet restore
```

### 3. Compilar solución
```bash
dotnet build
```

### 4. Ejecutar API
```bash
dotnet run --project API.AgendaCitas
```

### 5. Swagger
La documentación Swagger se encuentra disponible en:

```text
https://localhost:44351/swagger
```

> El puerto puede variar según la configuración local.

---

## Configuración de base de datos

### 1. Crear base de datos en SQL Server
Crear una base de datos para el sistema, por ejemplo:

```sql
CREATE DATABASE CitaSalud;
```

### 2. Ejecutar scripts SQL
Ejecutar:
- creación de tablas
- inserción de catálogo de especialidades
- inserción de duración por especialidad
- creación de procedimientos almacenados

### 3. Migraciones
Si se usan migraciones de EF Core:

```bash
dotnet ef migrations add InitialCreate --project API.Models/API.Models.csproj --startup-project API.AgendaCitas/API.AgendaCitas.csproj
dotnet ef database update --project API.Models/API.Models.csproj --startup-project API.AgendaCitas/API.AgendaCitas.csproj
```

---

## Configuración del frontend

### 1. Instalar dependencias
Dentro del proyecto Angular:

```bash
npm install
```

### 2. Ejecutar frontend
```bash
ng serve -o
```

### 3. Configurar URL de la API
En `src/environments/environment.ts`:

```ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:44351/api'
};
```

---

## Alertas y experiencia de usuario

Se implementó SweetAlert2 para mostrar:

- Confirmaciones antes de eliminar
- Mensajes de éxito al crear/editar/eliminar
- Errores de validación o fallos de operación
- Alertas al agendar citas con conflicto de horario

---

## Pruebas unitarias

Se implementaron pruebas unitarias con **xUnit** y **EF Core InMemory**.

### Casos cubiertos
- No permitir agendar citas en el pasado
- No permitir citas fuera del horario del médico
- Calcular la duración correctamente según la especialidad
- Detectar conflicto de horarios
- Generar sugerencias cuando el horario no está disponible
- Activar alerta por 3 cancelaciones en 30 días
- Cambiar estado a cancelada al cancelar una cita

### Ejecutar pruebas
```bash
dotnet test
```

---

## Evidencia de pruebas

Resultado esperado:

```text
Resumen de pruebas: total: 7, con errores: 0, correcto: 7, omitido: 0
```

---

## Decisiones de diseño

- Se separó la solución por capas para mantener responsabilidades claras.
- La lógica de negocio se implementó en servicios para evitar duplicarla en controladores.
- Se normalizó la especialidad en catálogo para evitar guardar texto libre en médicos.
- La duración de la consulta se configuró por especialidad para hacer el sistema flexible.
- Los horarios del médico se modelaron en una tabla independiente para soportar varios días y rangos.
- La validación de conflictos de cita se realiza en backend para proteger la regla de negocio.
- El frontend usa servicios desacoplados para consumir la API.
- Se usaron procedimientos almacenados en consultas específicas de dashboard y agenda.
- Se implementaron pruebas unitarias sobre reglas críticas de negocio.

## Consideraciones de diseño

- Se separó la solución por capas para mantener mejor organización.
- La lógica de negocio se concentra en servicios.
- Se normalizó el manejo de especialidades y duración de consulta.
- Se implementaron procedimientos almacenados para consultas específicas del dashboard y agenda.
- El frontend consume la API mediante servicios desacoplados.

---

## Posibles mejoras futuras

- Autenticación y autorización por roles
- Bitácora de auditoría
- Agenda semanal/mensual visual
- Exportación de citas a PDF o Excel
- Notificaciones por correo o WhatsApp
- Filtros avanzados por médico, paciente y estado de cita

---

## Autor

Ulises Flores

Desarrollado como solución para la prueba técnica **Agenda Cita**.

