using BCrypt.Net;
using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Infrastructure.Security;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool Verify(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (SaltParseException)
        {
            return false;
        }
    }
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
        if (user is null || !user.IsActive || !PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        // Update last login time, but don't fail authentication if update fails
        try
        {
            user.LastLoginAtUtc = DateTime.UtcNow;
            repository.Update(user);
            await repository.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException or Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            // Log error but don't break authentication - user can still login if timestamp update fails
            // In production, you might want to log this: Logger?.LogWarning(ex, "Failed to update last login time for user {Username}", username);
        }
        return user;
    }

    public async Task<Guid> CreateUserAsync(string username, string password, string fullName, string? email, UserRole role, CancellationToken ct = default)
    {
        if (await repository.UsernameExistsAsync(username, null, ct))
            throw new ArgumentException($"Username '{username}' already exists.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.", nameof(password));

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
            throw new ArgumentException("New password must be at least 6 characters.", nameof(newPassword));

        user.PasswordHash = PasswordHasher.HashPassword(newPassword);
        repository.Update(user);
        await repository.SaveChangesAsync(ct);
    }
}
