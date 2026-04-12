using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default);
    Task<bool> UsernameExistsAsync(string username, Guid? excludeId = null, CancellationToken ct = default);
}
