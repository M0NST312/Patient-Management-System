using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class Contact : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    [MaxLength(20)] public required string Type { get; set; }
    [MaxLength(200)] public required string Value { get; set; }
    public bool IsEmergency { get; set; }
    public Patient Patient { get; set; } = null!;
}
