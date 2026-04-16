using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Services;

public class VisitService(IVisitRepository repository, IPatientRepository patientRepository) : IVisitService
{
    public async Task<VisitDetailsDto?> GetVisitByIdAsync(Guid id, CancellationToken ct = default)
    {
        var visit = await repository.GetWithDetailsAsync(id, ct);
        return visit is null ? null : MapToDetails(visit);
    }

    public async Task<IEnumerable<VisitDetailsDto>> GetAllVisitsAsync(CancellationToken ct = default)
    {
        var visits = await repository.GetAllWithPatientAsync(ct);
        return visits.Select(MapToDetails).ToList();
    }

    public async Task<IEnumerable<VisitDetailsDto>> GetRecentVisitsAsync(int count, CancellationToken ct = default)
    {
        var visits = await repository.GetRecentWithPatientAsync(count, ct);
        return visits.Select(MapToDetails).ToList();
    }

    public async Task<IEnumerable<VisitDetailsDto>> GetVisitsByPatientIdAsync(Guid patientId, CancellationToken ct = default)
    {
        var visits = await repository.GetByPatientIdAsync(patientId, ct);
        return visits.Select(MapToDetails).ToList();
    }

    public async Task<IEnumerable<VisitDetailsDto>> GetVisitsByStatusAsync(VisitStatus status, CancellationToken ct = default)
    {
        var visits = await repository.GetByStatusAsync(status, ct);
        return visits.Select(MapToDetails).ToList();
    }

    public async Task<Guid> CreateVisitAsync(VisitCreateDto dto, CancellationToken ct = default)
    {
        var patient = await patientRepository.GetByIdAsync(dto.PatientId, ct)
            ?? throw new KeyNotFoundException($"Patient with ID '{dto.PatientId}' not found.");

        var visit = new Visit
        {
            PatientId = dto.PatientId,
            VisitDateUtc = dto.VisitDateUtc,
            DoctorName = dto.DoctorName,
            Complaint = dto.Complaint,
            Notes = dto.Notes,
            Status = dto.Status
        };

        await repository.AddAsync(visit, ct);
        await repository.SaveChangesAsync(ct);
        return visit.Id;
    }

    public async Task UpdateVisitAsync(Guid id, VisitUpdateDto dto, CancellationToken ct = default)
    {
        var visit = await repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Visit with ID '{id}' not found.");
        visit.VisitDateUtc = dto.VisitDateUtc;
        visit.DoctorName = dto.DoctorName;
        visit.Complaint = dto.Complaint;
        visit.Notes = dto.Notes;
        visit.Status = dto.Status;
        repository.Update(visit);
        await repository.SaveChangesAsync(ct);
    }

    public async Task DeleteVisitAsync(Guid id, CancellationToken ct = default)
    {
        var visit = await repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Visit with ID '{id}' not found.");
        repository.Delete(visit);
        await repository.SaveChangesAsync(ct);
    }

    public async Task AddDiagnosisAsync(Guid visitId, DiagnosisCreateDto dto, CancellationToken ct = default)
    {
        _ = await repository.GetByIdAsync(visitId, ct) ?? throw new KeyNotFoundException($"Visit '{visitId}' not found.");
        var diagnosis = new Diagnosis { VisitId = visitId, Description = dto.Description, Icd10Code = dto.Icd10Code };
        await repository.AddDiagnosisAsync(diagnosis, ct);
        await repository.SaveChangesAsync(ct);
    }

    public async Task RemoveDiagnosisAsync(Guid diagnosisId, CancellationToken ct = default)
    {
        var diagnosis = await repository.GetDiagnosisByIdAsync(diagnosisId, ct)
            ?? throw new KeyNotFoundException($"Diagnosis '{diagnosisId}' not found.");
        repository.DeleteDiagnosis(diagnosis);
        await repository.SaveChangesAsync(ct);
    }

    public async Task AddPrescriptionAsync(Guid visitId, PrescriptionCreateDto dto, CancellationToken ct = default)
    {
        _ = await repository.GetByIdAsync(visitId, ct) ?? throw new KeyNotFoundException($"Visit '{visitId}' not found.");
        var prescription = new Prescription { VisitId = visitId, MedicationName = dto.MedicationName, Dosage = dto.Dosage, Instructions = dto.Instructions };
        await repository.AddPrescriptionAsync(prescription, ct);
        await repository.SaveChangesAsync(ct);
    }

    public async Task RemovePrescriptionAsync(Guid prescriptionId, CancellationToken ct = default)
    {
        var prescription = await repository.GetPrescriptionByIdAsync(prescriptionId, ct)
            ?? throw new KeyNotFoundException($"Prescription '{prescriptionId}' not found.");
        repository.DeletePrescription(prescription);
        await repository.SaveChangesAsync(ct);
    }

    public async Task<int> GetTotalVisitsCountAsync(CancellationToken ct = default) =>
        await repository.CountAsync(null, ct);

    private static VisitDetailsDto MapToDetails(Visit visit) => new(
        visit.Id, visit.PatientId, visit.VisitDateUtc, visit.DoctorName, visit.Complaint,
        visit.Notes, visit.Status, visit.Patient?.FullName,
        visit.Diagnoses.Select(d => new DiagnosisDto(d.Id, d.Description, d.Icd10Code)).ToList(),
        visit.Prescriptions.Select(p => new PrescriptionDto(p.Id, p.MedicationName, p.Dosage, p.Instructions)).ToList()
    );
}
