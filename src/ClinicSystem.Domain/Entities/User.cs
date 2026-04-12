using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Domain.Entities;

public class User : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(100)] public required string Username { get; set; }
    [MaxLength(500)] public required string PasswordHash { get; set; }
    [MaxLength(200)] public required string FullName { get; set; }
    [MaxLength(100)] public string? Email { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAtUtc { get; set; }
}
