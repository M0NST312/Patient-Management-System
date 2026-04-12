using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Dtos;

// User DTOs
public record UserDto(Guid Id, string Username, string FullName, string? Email, UserRole Role, bool IsActive, DateTime? LastLoginAtUtc);
public record UserCreateDto(string Username, string Password, string FullName, string? Email, UserRole Role);
public record UserUpdateDto(string FullName, string? Email, UserRole Role, bool IsActive);

// Patient DTOs
public record ContactDto(string Type, string Value, bool IsEmergency);
public record AddressDto(string Line1, string? Line2, string City, string? State, string? PostalCode, string Country);

public record PatientCreateDto(
    string FullName, DateOnly DateOfBirth, Gender Gender,
    string NationalId, string? BloodType, AddressDto? Address, List<ContactDto>? Contacts);

public record PatientUpdateDto(
    string FullName, DateOnly DateOfBirth, Gender Gender,
    string NationalId, string? BloodType, AddressDto? Address, List<ContactDto>? Contacts);

public record PatientSummaryDto(Guid Id, string FullName, DateOnly DateOfBirth, Gender Gender, string NationalId, string? BloodType);

public record PatientDetailsDto(
    Guid Id, string FullName, DateOnly DateOfBirth, Gender Gender,
    string NationalId, string? BloodType, AddressDto? Address, List<ContactDto>? Contacts);

// Visit DTOs
public record VisitCreateDto(Guid PatientId, DateTime VisitDateUtc, string DoctorName, string Complaint, string? Notes, VisitStatus Status);
public record VisitUpdateDto(DateTime VisitDateUtc, string DoctorName, string Complaint, string? Notes, VisitStatus Status);
public record DiagnosisCreateDto(string Description, string? Icd10Code);
public record PrescriptionCreateDto(string MedicationName, string? Dosage, string? Instructions);

public record DiagnosisDto(Guid Id, string Description, string? Icd10Code);
public record PrescriptionDto(Guid Id, string MedicationName, string? Dosage, string? Instructions);

public record VisitDetailsDto(
    Guid Id, Guid PatientId, DateTime VisitDate, string DoctorName,
    string Complaint, string? Notes, VisitStatus Status, string? PatientName,
    List<DiagnosisDto> Diagnoses, List<PrescriptionDto> Prescriptions);

// Invoice DTOs
public record InvoiceItemDto(string Description, decimal UnitPrice, int Quantity);
public record InvoiceCreateDto(Guid PatientId, decimal DiscountAmount, List<InvoiceItemDto> Items);
public record InvoiceUpdateDto(decimal DiscountAmount, List<InvoiceItemDto> Items);
public record PaymentCreateDto(decimal Amount, string Method);

public record PaymentDto(Guid Id, decimal Amount, string Method, DateTime PaidAtUtc);
public record InvoiceItemDetailsDto(Guid Id, string Description, decimal UnitPrice, int Quantity, decimal Total);

public record InvoiceDetailsDto(
    Guid Id, Guid PatientId, string InvoiceNumber, decimal DiscountAmount,
    InvoiceStatus Status, List<InvoiceItemDetailsDto> Items, List<PaymentDto> Payments,
    DateTime CreatedAtUtc, string? PatientName, decimal TotalAmount, decimal PaidAmount, decimal Balance);
