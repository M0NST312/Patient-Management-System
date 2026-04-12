using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Domain.Entities;

public class Visit : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public DateTime VisitDateUtc { get; set; }
    [MaxLength(200)] public required string DoctorName { get; set; }
    [MaxLength(300)] public required string Complaint { get; set; }
    [MaxLength(3000)] public string? Notes { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;
    public Patient Patient { get; set; } = null!;
    public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
