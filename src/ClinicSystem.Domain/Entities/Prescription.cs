using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class Prescription : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VisitId { get; set; }
    [MaxLength(200)] public required string MedicationName { get; set; }
    [MaxLength(100)] public string? Dosage { get; set; }
    [MaxLength(500)] public string? Instructions { get; set; }
    public Visit Visit { get; set; } = null!;
}
