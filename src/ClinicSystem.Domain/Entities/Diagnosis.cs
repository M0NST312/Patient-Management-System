using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class Diagnosis : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VisitId { get; set; }
    [MaxLength(300)] public required string Description { get; set; }
    [MaxLength(20)] public string? Icd10Code { get; set; }
    public Visit Visit { get; set; } = null!;
}
