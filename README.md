# Clinic Patient Management System

ASP.NET 10 Razor Pages clinic management app restructured into **Clean Architecture**.

## Architecture

```
ClinicSystem/
└── src/
    ├── ClinicSystem.Domain/          ← Entities, Enums (no deps)
    │   ├── Entities/
    │   │   ├── Patient.cs
    │   │   ├── Visit.cs, Diagnosis.cs, Prescription.cs
    │   │   ├── Invoice.cs, InvoiceItem.cs, Payment.cs
    │   │   ├── User.cs, Address.cs, Contact.cs
    │   └── Enums/Enums.cs
    │
    ├── ClinicSystem.Application/     ← Interfaces, Services, DTOs (→ Domain)
    │   ├── Common/Interfaces/        ← IRepository<T>, IPatientRepository…
    │   ├── Dtos/                     ← All request/response records
    │   ├── Services/                 ← PatientService, VisitService, InvoiceService
    │   └── Extensions/               ← AddApplicationServices()
    │
    ├── ClinicSystem.Infrastructure/  ← EF Core, Repos, Security (→ Application)
    │   ├── Data/ApplicationDbContext.cs
    │   ├── Repositories/             ← Repository<T>, PatientRepo, VisitRepo…
    │   ├── Security/AuthService.cs, PasswordHasher.cs
    │   └── Extensions/               ← AddInfrastructureServices()
    │
    └── ClinicSystem.Web/             ← Razor Pages, Program.cs (→ Application + Infrastructure)
        ├── Pages/
        │   ├── Account/Login, Logout
        │   ├── Patients/Index, Create, Edit, Details
        │   ├── Visits/Index, Create, Details
        │   └── Billing/Index, Create, Details
        └── Program.cs
```

## Layer Dependency Rule
`Domain` ← `Application` ← `Infrastructure` ← `Web`

## Setup

1. Install prerequisites:
   ```bash
   # .NET 10 SDK + PostgreSQL
   dotnet tool install --global dotnet-ef
   ```

2. Configure connection string in `src/ClinicSystem.Web/appsettings.json`:
   ```json
   { "ConnectionStrings": { "Default": "Host=localhost;Port=5432;Database=clinic;Username=postgres;Password=..." } }
   ```
   Or set env var: `CLINIC_DB_CONNECTION`

3. Apply migrations (run from `src/ClinicSystem.Web`):
   ```bash
   dotnet ef database update --project ../ClinicSystem.Infrastructure
   ```

4. Run:
   ```bash
   cd src/ClinicSystem.Web
   dotnet run
   ```

## Default Credentials

| Role          | Username       | Password          |
|---------------|---------------|-------------------|
| Admin         | admin         | Admin@123         |
| Doctor        | doctor        | Doctor@123        |
| Receptionist  | receptionist  | Reception@123     |

## Role Permissions

| Feature           | Admin | Doctor | Receptionist |
|-------------------|-------|--------|--------------|
| View patients     | ✓     | ✓      | ✓            |
| Create/edit patient | ✓   | –      | ✓            |
| Create/view visits | ✓   | ✓      | –            |
| Add diagnosis/Rx  | ✓     | ✓      | –            |
| Billing / invoices | ✓   | –      | ✓            |
| Record payments   | ✓     | –      | ✓            |
| Delete records    | ✓     | –      | –            |

