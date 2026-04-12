using ClinicSystem.Application.Dtos;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Services.Interfaces;

public interface IPatientService
{
    Task<PatientDetailsDto?> GetPatientByIdAsync(Guid id, CancellationToken ct = default);
    Task<PatientDetailsDto?> GetPatientByNationalIdAsync(string nationalId, CancellationToken ct = default);
    Task<IEnumerable<PatientDetailsDto>> GetAllPatientsAsync(CancellationToken ct = default);
    Task<IEnumerable<PatientDetailsDto>> SearchPatientsByNameAsync(string name, CancellationToken ct = default);
    Task<Guid> CreatePatientAsync(PatientCreateDto dto, CancellationToken ct = default);
    Task UpdatePatientAsync(Guid id, PatientUpdateDto dto, CancellationToken ct = default);
    Task DeletePatientAsync(Guid id, CancellationToken ct = default);
    Task<int> GetTotalPatientsCountAsync(CancellationToken ct = default);
}

public interface IVisitService
{
    Task<VisitDetailsDto?> GetVisitByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<VisitDetailsDto>> GetAllVisitsAsync(CancellationToken ct = default);
    Task<IEnumerable<VisitDetailsDto>> GetRecentVisitsAsync(int count, CancellationToken ct = default);
    Task<IEnumerable<VisitDetailsDto>> GetVisitsByPatientIdAsync(Guid patientId, CancellationToken ct = default);
    Task<IEnumerable<VisitDetailsDto>> GetVisitsByStatusAsync(VisitStatus status, CancellationToken ct = default);
    Task<Guid> CreateVisitAsync(VisitCreateDto dto, CancellationToken ct = default);
    Task UpdateVisitAsync(Guid id, VisitUpdateDto dto, CancellationToken ct = default);
    Task DeleteVisitAsync(Guid id, CancellationToken ct = default);
    Task AddDiagnosisAsync(Guid visitId, DiagnosisCreateDto dto, CancellationToken ct = default);
    Task RemoveDiagnosisAsync(Guid diagnosisId, CancellationToken ct = default);
    Task AddPrescriptionAsync(Guid visitId, PrescriptionCreateDto dto, CancellationToken ct = default);
    Task RemovePrescriptionAsync(Guid prescriptionId, CancellationToken ct = default);
    Task<int> GetTotalVisitsCountAsync(CancellationToken ct = default);
}

public interface IInvoiceService
{
    Task<InvoiceDetailsDto?> GetInvoiceByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<InvoiceDetailsDto>> GetAllInvoicesAsync(CancellationToken ct = default);
    Task<IEnumerable<InvoiceDetailsDto>> GetInvoicesByPatientIdAsync(Guid patientId, CancellationToken ct = default);
    Task<IEnumerable<InvoiceDetailsDto>> GetInvoicesByStatusAsync(InvoiceStatus status, CancellationToken ct = default);
    Task<Guid> CreateInvoiceAsync(InvoiceCreateDto dto, CancellationToken ct = default);
    Task UpdateInvoiceAsync(Guid id, InvoiceUpdateDto dto, CancellationToken ct = default);
    Task DeleteInvoiceAsync(Guid id, CancellationToken ct = default);
    Task AddPaymentAsync(Guid invoiceId, PaymentCreateDto dto, CancellationToken ct = default);
    Task<decimal> GetTotalOutstandingBalanceAsync(CancellationToken ct = default);
    Task<int> GetTotalInvoicesCountAsync(CancellationToken ct = default);
}

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken ct = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateUserAsync(UserCreateDto dto, CancellationToken ct = default);
    Task UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken ct = default);
    Task ResetPasswordAsync(Guid id, string newPassword, CancellationToken ct = default);
    Task ToggleActiveAsync(Guid id, CancellationToken ct = default);
    Task DeleteUserAsync(Guid id, CancellationToken ct = default);
}

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string username, string password, CancellationToken ct = default);
    Task<Guid> CreateUserAsync(string username, string password, string fullName, string? email, UserRole role, CancellationToken ct = default);
    Task UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default);
}
