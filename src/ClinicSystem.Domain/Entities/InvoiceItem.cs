using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;

namespace ClinicSystem.Domain.Entities;

public class InvoiceItem : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    [MaxLength(200)] public required string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total => UnitPrice * Quantity;
    public Invoice Invoice { get; set; } = null!;
}
