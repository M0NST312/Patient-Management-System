using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class PatientRepository(ApplicationDbContext context) : Repository<Patient>(context), IPatientRepository
{
    // FIXED: override GetByIdAsync to include related entities
    public override async Task<Patient?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(p => p.Address).Include(p => p.Contacts).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Patient?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(p => p.Address).Include(p => p.Contacts).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Patient?> GetByNationalIdAsync(string nationalId, CancellationToken ct = default) =>
        await DbSet.Include(p => p.Address).Include(p => p.Contacts).FirstOrDefaultAsync(p => p.NationalId == nationalId, ct);

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name, CancellationToken ct = default) =>
        await DbSet.Where(p => p.FullName.Contains(name) || p.NationalId.Contains(name))
            .Include(p => p.Address).Include(p => p.Contacts).ToListAsync(ct);

    public async Task<bool> NationalIdExistsAsync(string nationalId, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = DbSet.Where(p => p.NationalId == nationalId);
        if (excludeId.HasValue) query = query.Where(p => p.Id != excludeId.Value);
        return await query.AnyAsync(ct);
    }
}
