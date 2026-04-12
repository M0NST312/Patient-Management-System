using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Domain.Entities;

public class Invoice : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(30)] public required string InvoiceNumber { get; set; }
    public Guid PatientId { get; set; }
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    public decimal DiscountAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public Patient Patient { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public decimal TotalAmount => Items.Sum(i => i.Total) - DiscountAmount;
    public decimal PaidAmount => Payments.Sum(p => p.Amount);
    public decimal Balance => TotalAmount - PaidAmount;
}
