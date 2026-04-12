using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class Payment : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    [MaxLength(30)] public required string Method { get; set; }
    public Invoice Invoice { get; set; } = null!;
}
