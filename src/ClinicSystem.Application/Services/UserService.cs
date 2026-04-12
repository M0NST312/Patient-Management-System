using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Entities;

namespace ClinicSystem.Application.Services;

public class UserService(IUserRepository repository, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await repository.GetAllAsync(ct);
        return users.Select(ToDto).OrderBy(u => u.FullName);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct);
        return user is null ? null : ToDto(user);
    }

    public async Task<Guid> CreateUserAsync(UserCreateDto dto, CancellationToken ct = default)
    {
        if (await repository.UsernameExistsAsync(dto.Username, null, ct))
            throw new ArgumentException($"Username '{dto.Username}' is already taken.");
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");

        var user = new User
        {
            Username = dto.Username.Trim(),
            PasswordHash = passwordHasher.HashPassword(dto.Password),
            FullName = dto.FullName.Trim(),
            Email = dto.Email?.Trim(),
            Role = dto.Role,
            IsActive = true
        };
        await repository.AddAsync(user, ct);
        await repository.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("User not found.");
        user.FullName = dto.FullName.Trim();
        user.Email = dto.Email?.Trim();
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;
        repository.Update(user);
        await repository.SaveChangesAsync(ct);
    }

    public async Task ResetPasswordAsync(Guid id, string newPassword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");
        var user = await repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("User not found.");
        user.PasswordHash = passwordHasher.HashPassword(newPassword);
        repository.Update(user);
        await repository.SaveChangesAsync(ct);
    }

    public async Task ToggleActiveAsync(Guid id, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("User not found.");
        user.IsActive = !user.IsActive;
        repository.Update(user);
        await repository.SaveChangesAsync(ct);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("User not found.");
        repository.Delete(user);
        await repository.SaveChangesAsync(ct);
    }

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Username, u.FullName, u.Email, u.Role, u.IsActive, u.LastLoginAtUtc);
}
