using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Common.Interfaces;

public interface IVisitRepository : IRepository<Visit>
{
    Task<IEnumerable<Visit>> GetAllWithPatientAsync(CancellationToken ct = default);
    Task<IEnumerable<Visit>> GetRecentWithPatientAsync(int count, CancellationToken ct = default);
    Task<IEnumerable<Visit>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default);
    Task<IEnumerable<Visit>> GetByStatusAsync(VisitStatus status, CancellationToken ct = default);
    Task<Visit?> GetWithDetailsAsync(Guid visitId, CancellationToken ct = default);
    Task<Diagnosis?> GetDiagnosisByIdAsync(Guid diagnosisId, CancellationToken ct = default);
    Task<Prescription?> GetPrescriptionByIdAsync(Guid prescriptionId, CancellationToken ct = default);
    Task AddDiagnosisAsync(Diagnosis diagnosis, CancellationToken ct = default);
    Task AddPrescriptionAsync(Prescription prescription, CancellationToken ct = default);
    void DeleteDiagnosis(Diagnosis diagnosis);
    void DeletePrescription(Prescription prescription);
}
