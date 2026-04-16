using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using ClinicSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

#pragma warning disable CS9107
public class VisitRepository(ApplicationDbContext context) : Repository<Visit>(context), IVisitRepository
#pragma warning restore CS9107
{
    public async Task<IEnumerable<Visit>> GetAllWithPatientAsync(CancellationToken ct = default) =>
        await DbSet.Include(v => v.Patient).Include(v => v.Diagnoses).Include(v => v.Prescriptions)
            .OrderByDescending(v => v.VisitDateUtc).ToListAsync(ct);

    public async Task<IEnumerable<Visit>> GetRecentWithPatientAsync(int count, CancellationToken ct = default) =>
        await DbSet.Include(v => v.Patient).Include(v => v.Diagnoses).Include(v => v.Prescriptions)
            .OrderByDescending(v => v.VisitDateUtc).Take(count).ToListAsync(ct);

    public async Task<IEnumerable<Visit>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default) =>
        await DbSet.Where(v => v.PatientId == patientId)
            .Include(v => v.Diagnoses).Include(v => v.Prescriptions)
            .OrderByDescending(v => v.VisitDateUtc).ToListAsync(ct);

    public async Task<IEnumerable<Visit>> GetByStatusAsync(VisitStatus status, CancellationToken ct = default) =>
        await DbSet.Where(v => v.Status == status).Include(v => v.Patient)
            .Include(v => v.Diagnoses).Include(v => v.Prescriptions)
            .OrderByDescending(v => v.VisitDateUtc).ToListAsync(ct);

    public async Task<Visit?> GetWithDetailsAsync(Guid visitId, CancellationToken ct = default) =>
        await DbSet.Where(v => v.Id == visitId).Include(v => v.Patient)
            .Include(v => v.Diagnoses).Include(v => v.Prescriptions)
            .FirstOrDefaultAsync(ct);

    public async Task<Diagnosis?> GetDiagnosisByIdAsync(Guid diagnosisId, CancellationToken ct = default) =>
        await context.Diagnoses.FindAsync([diagnosisId], ct);

    public async Task<Prescription?> GetPrescriptionByIdAsync(Guid prescriptionId, CancellationToken ct = default) =>
        await context.Prescriptions.FindAsync([prescriptionId], ct);

    public async Task AddDiagnosisAsync(Diagnosis diagnosis, CancellationToken ct = default) =>
        await context.Diagnoses.AddAsync(diagnosis, ct);

    public async Task AddPrescriptionAsync(Prescription prescription, CancellationToken ct = default) =>
        await context.Prescriptions.AddAsync(prescription, ct);

    public void DeleteDiagnosis(Diagnosis diagnosis) => context.Diagnoses.Remove(diagnosis);
    public void DeletePrescription(Prescription prescription) => context.Prescriptions.Remove(prescription);
}
