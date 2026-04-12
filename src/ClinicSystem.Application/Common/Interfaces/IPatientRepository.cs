using ClinicSystem.Domain.Entities;

namespace ClinicSystem.Application.Common.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<Patient?> GetByNationalIdAsync(string nationalId, CancellationToken ct = default);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name, CancellationToken ct = default);
    Task<bool> NationalIdExistsAsync(string nationalId, Guid? excludeId = null, CancellationToken ct = default);
}
