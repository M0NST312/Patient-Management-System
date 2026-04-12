using System.Security.Cryptography;
using System.Text;
using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Infrastructure.Security;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static bool Verify(string password, string hash) =>
        HashPassword(password) == hash;
}

public class PasswordHasherService : IPasswordHasher
{
    public string HashPassword(string password) => PasswordHasher.HashPassword(password);
    public bool Verify(string password, string hash) => PasswordHasher.Verify(password, hash);
}

public class AuthService(IUserRepository repository) : IAuthService
{
    public async Task<User?> AuthenticateAsync(string username, string password, CancellationToken ct = default)
    {
        var user = await repository.GetByUsernameAsync(username, ct);
        if (user == null || !user.IsActive || !PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        user.LastLoginAtUtc = DateTime.UtcNow;
        repository.Update(user);
        await repository.SaveChangesAsync(ct);
        return user;
    }

    public async Task<Guid> CreateUserAsync(string username, string password, string fullName, string? email, UserRole role, CancellationToken ct = default)
    {
        if (await repository.UsernameExistsAsync(username, null, ct))
            throw new ArgumentException($"Username '{username}' already exists.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");

        var user = new User
        {
            Username = username,
            PasswordHash = PasswordHasher.HashPassword(password),
            FullName = fullName,
            Email = email,
            Role = role,
            IsActive = true
        };
        await repository.AddAsync(user, ct);
        await repository.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(userId, ct) ?? throw new KeyNotFoundException("User not found.");
        if (!PasswordHasher.Verify(currentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            throw new ArgumentException("New password must be at least 6 characters.");

        user.PasswordHash = PasswordHasher.HashPassword(newPassword);
        repository.Update(user);
        await repository.SaveChangesAsync(ct);
    }
}
