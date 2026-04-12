using System.ComponentModel.DataAnnotations;
using ClinicSystem.Domain.Common;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Domain.Entities;

public class Patient : AuditableEntity
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(200)] public required string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    [MaxLength(100)] public required string NationalId { get; set; }
    [MaxLength(10)] public string? BloodType { get; set; }
    public Address? Address { get; set; }
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
