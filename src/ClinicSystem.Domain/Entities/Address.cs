using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class Address : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    [MaxLength(200)] public required string Line1 { get; set; }
    [MaxLength(200)] public string? Line2 { get; set; }
    [MaxLength(100)] public required string City { get; set; }
    [MaxLength(100)] public string? State { get; set; }
    [MaxLength(20)] public string? PostalCode { get; set; }
    [MaxLength(100)] public required string Country { get; set; }
    public Patient Patient { get; set; } = null!;
}
