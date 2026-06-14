# ClinicSystem v2 — User Guide

**Overview**
- Purpose: Manage patients, visits, invoices, and payments for a small clinic.
- Audience: Receptionists, Administrators, and support staff who'll use the web UI.

**Quick Links**
- Project root: `ClinicSystem.sln`
- Web app: `src/ClinicSystem.Web`
- Application services: `src/ClinicSystem.Application`
- Database context / migrations: `src/ClinicSystem.Infrastructure`

**Prerequisites**
- .NET SDK 10 or later installed
- PostgreSQL (or the configured DB provider) running for production/dev data
- Connection string configured in `src/ClinicSystem.Web/appsettings.json`

**Getting Started (Developer / Local)**
1. Clone the repo and restore packages.
2. Configure DB connection string in `src/ClinicSystem.Web/appsettings.json` (or set `CLINIC_DB_CONNECTION`).
3. Build and run locally:

```bash
dotnet build ClinicSystem.sln
dotnet run --project src/ClinicSystem.Web
```

4. Useful tasks (VS Code tasks):
 - `build` — builds the solution
 - `watch` — runs `dotnet watch run` for the web project
 - `docker-build` / `docker-run` — build/run Docker images (see `src/ClinicSystem.Web/Dockerfile`)

**Logging In & Roles**
- Default roles used by the app: `Admin`, `Receptionist`, `Doctor`.
- Role permissions summary:
  - Admin: full access (manage invoices, delete, user management)
  - Receptionist: create patients, visits, invoices, record payments
  - Doctor: view patient records and visit history; create and update `Visit` notes; add `Diagnosis` entries; create `Prescription` records; view related invoices and payments. Doctors do not have user-management privileges and cannot delete invoices.

**Users & User Management**
- Overview: Only users with the `Admin` role can create, edit, or delete application users. This section explains the common user-management workflows and recommends filenames for screenshots you can add.

- Add a user (Admin):
  1. Navigate to `Admin` → `Users` in the web UI.
  2. Click **New User** (screenshot: `docs/images/user-create.png`).
  3. Fill the fields:
     - `Username`: unique identifier (3–50 chars).
     - `Email`: valid email address.
     - `Password`: follow the app's password policy (minimum length enforced server-side).
     - `Roles`: select one or more roles (`Admin`, `Receptionist`, `Doctor`).
     - Optional fields: first/last name, contact details, and any department/clinic assignment.
  4. Click **Create** to save the new user. The new user will appear in the users list.

- Edit a user:
  1. From `Admin` → `Users`, locate the user and click **Edit** (screenshot: `docs/images/user-edit.png`).
  2. Change fields or role assignments as needed and click **Save**.

- Delete a user:
  - Only `Admin` users can delete accounts. Deletions are irreversible; confirm before proceeding.

- Doctor-specific note:
  - When creating or editing a user who should act as a clinician, assign the `Doctor` role. Doctors can view patient histories, add `Visit` notes, add `Diagnosis` and `Prescription` records, and view invoices/payments related to their patients. They do not have user-management or invoice-deletion privileges.

- Screenshots (optional):
  - Place screenshots in `docs/images/` with the following recommended names:
    - `user-list.png` — users list view
    - `user-create.png` — create user form
    - `user-edit.png` — edit user form
  - Example Markdown to embed a screenshot:

```markdown
![Users list](docs/images/user-list.png)
```

- If you want me to add actual screenshots, provide image files or let me know if I should capture browser screenshots from a running instance; otherwise I can add the placeholders and you can attach images later.

**UI Overview**
- Dashboard: summary metrics (invoices outstanding, recent visits).
- Patients: create, view, and update patient records.
- Visits: record patient visits and attach diagnoses/prescriptions.
- Billing / Invoices: create invoices, add items, apply discounts, view invoice details.
- Payments: record payments on an invoice from the invoice details page.

**Managing Invoices**
- Create invoice: from Billing → New Invoice. Provide patient, items, and optional discount.
- Invoice fields:
  - `InvoiceNumber`: system-generated (format: `INV-<year>-XXXX`).
  - `Items`: description, unit price, quantity. Total is computed.
  - `DiscountAmount`: applied to invoice total.
  - `Status`: `Unpaid`, `Partial`, `Paid` (set automatically when payments are recorded).
- Update invoice: allowed when status is not `Paid`.
- Delete invoice: allowed only when `Unpaid` (Admin only).

**Recording Payments**
- Navigate to Billing → Invoice Details → Record Payment.
- Required fields: `Amount` (positive decimal), `Payment Method` (select or text). The UI enforces a maximum equal to the invoice `Balance`.
- Validation rules (client + server):
  - `Amount` must be > 0 and cannot exceed the invoice balance.
  - `Payment.Method` must be non-empty and at most 30 characters.
- Status transition logic:
  - After recording a payment the app recalculates `PaidAmount` and `Balance`.
  - If total paid >= total amount → `Status = Paid`.
  - If 0 < total paid < total amount → `Status = Partial`.

**Common Issues & Troubleshooting**
- Payment not saved / invoice state not updated:
  1. Validation error: ensure `Method` string length ≤ 30 characters. Long methods can be rejected by DB.
  2. DB constraint errors: check server logs for `DbUpdateException` and the inner exception; the web UI surfaces friendly messages in `PaymentError`.
  3. If payments appear not to persist, run the following DB checks (Postgres example):

```sql
SELECT id, amount, method, paidatutc FROM payments WHERE invoiceid = '<invoice-id>';
SELECT id, status FROM invoices WHERE id = '<invoice-id>';
```

- Concurrency conflicts: if two users update the same invoice concurrently you may see a concurrency error — refresh the page and try again.

**Configuration**
- Primary configuration file: `src/ClinicSystem.Web/appsettings.json`.
  - Update the `ConnectionStrings:Default` value with your DB connection.

**Database & Migrations**
- Migrations live in `src/ClinicSystem.Infrastructure/Migrations`.
- To add a migration locally (development):

```bash
cd src/ClinicSystem.Infrastructure
dotnet ef migrations add <Name> --project ./ --startup-project ../ClinicSystem.Web
dotnet ef database update --project ./ --startup-project ../ClinicSystem.Web
```

**Development Notes & Tips**
- Repository pattern: repositories are in `src/ClinicSystem.Infrastructure/Repositories` and application services live in `src/ClinicSystem.Application/Services`.
- `InvoiceService.AddPaymentAsync` performs payment validation and updates `Invoice.Status` and `Payments`.
- If you change EF entities (annotations such as `[MaxLength]`), add and apply a migration to update the DB schema.

**Testing**
- Run unit/integration tests (if present):

```bash
dotnet test
```

**Deployment**
- Docker: `src/ClinicSystem.Web/Dockerfile` exists and workspace contains `compose.yaml`.
- Build image (example):

```bash
docker build -t clinicsystemv2:latest -f src/ClinicSystem.Web/Dockerfile .
docker run -e "ConnectionStrings__Default=<your-conn>" -p 5000:80 clinicsystemv2:latest
```

**Support & Contribution**
- For issues, open a ticket in the project tracker or contact the maintainer listed in README.md.
- To contribute: fork, create a branch, open a PR with tests if applicable.

**Appendix — Useful SQL & Debugging Queries**
- List invoices and balances:

```sql
SELECT i.id, i.invoicenumber, i.status, (
  (SELECT COALESCE(SUM(ii.unitprice * ii.quantity),0) FROM invoiceitems ii WHERE ii.invoiceid = i.id)
  - i.discountamount
) AS total_amount,
(
  (SELECT COALESCE(SUM(p.amount),0) FROM payments p WHERE p.invoiceid = i.id)
) AS paid_amount
FROM invoices i ORDER BY i.createdatutc desc;
```

---
This guide is a living document — if you want additional sections (API reference, screenshots, or step-by-step troubleshooting flows), tell me where to add them.

*** End Patch