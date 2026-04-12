using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using ClinicSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default) =>
        await DbSet.Where(u => u.Role == role).ToListAsync(ct);

    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = DbSet.Where(u => u.Username == username);
        if (excludeId.HasValue) query = query.Where(u => u.Id != excludeId.Value);
        return await query.AnyAsync(ct);
    }
}
