using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Entities;

namespace ClinicSystem.Application.Services;

public class PatientService(IPatientRepository repository) : IPatientService
{
    public async Task<PatientDetailsDto?> GetPatientByIdAsync(Guid id, CancellationToken ct = default)
    {
        var patient = await repository.GetByIdWithDetailsAsync(id, ct);
        return patient == null ? null : MapToDetails(patient);
    }

    public async Task<PatientDetailsDto?> GetPatientByNationalIdAsync(string nationalId, CancellationToken ct = default)
    {
        var patient = await repository.GetByNationalIdAsync(nationalId, ct);
        return patient == null ? null : MapToDetails(patient);
    }

    public async Task<IEnumerable<PatientDetailsDto>> GetAllPatientsAsync(CancellationToken ct = default)
    {
        var patients = await repository.GetAllAsync(ct);
        return patients.Select(MapToDetails).ToList();
    }

    public async Task<IEnumerable<PatientDetailsDto>> SearchPatientsByNameAsync(string name, CancellationToken ct = default)
    {
        var patients = await repository.SearchByNameAsync(name, ct);
        return patients.Select(MapToDetails).ToList();
    }

    public async Task<Guid> CreatePatientAsync(PatientCreateDto dto, CancellationToken ct = default)
    {
        if (await repository.NationalIdExistsAsync(dto.NationalId, null, ct))
            throw new ArgumentException($"Patient with National ID '{dto.NationalId}' already exists.");

        var patient = new Patient
        {
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            NationalId = dto.NationalId,
            BloodType = dto.BloodType
        };

        if (dto.Address != null)
            patient.Address = new Address
            {
                Line1 = dto.Address.Line1, Line2 = dto.Address.Line2, City = dto.Address.City,
                State = dto.Address.State, PostalCode = dto.Address.PostalCode, Country = dto.Address.Country
            };

        if (dto.Contacts != null)
            patient.Contacts = dto.Contacts.Select(c => new Contact { Type = c.Type, Value = c.Value, IsEmergency = c.IsEmergency }).ToList();

        await repository.AddAsync(patient, ct);
        await repository.SaveChangesAsync(ct);
        return patient.Id;
    }

    public async Task UpdatePatientAsync(Guid id, PatientUpdateDto dto, CancellationToken ct = default)
    {
        var patient = await repository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new KeyNotFoundException($"Patient with ID '{id}' not found.");

        if (patient.NationalId != dto.NationalId && await repository.NationalIdExistsAsync(dto.NationalId, id, ct))
            throw new ArgumentException($"National ID '{dto.NationalId}' is already in use.");

        patient.FullName = dto.FullName;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.NationalId = dto.NationalId;
        patient.BloodType = dto.BloodType;

        if (dto.Address != null)
        {
            if (patient.Address == null)
                patient.Address = new Address { PatientId = id, Line1 = dto.Address.Line1, City = dto.Address.City, Country = dto.Address.Country };
            patient.Address.Line1 = dto.Address.Line1;
            patient.Address.Line2 = dto.Address.Line2;
            patient.Address.City = dto.Address.City;
            patient.Address.State = dto.Address.State;
            patient.Address.PostalCode = dto.Address.PostalCode;
            patient.Address.Country = dto.Address.Country;
        }
        else
        {
            patient.Address = null;
        }

        if (dto.Contacts != null)
            patient.Contacts = dto.Contacts.Select(c => new Contact { Type = c.Type, Value = c.Value, IsEmergency = c.IsEmergency, PatientId = id }).ToList();

        repository.Update(patient);
        await repository.SaveChangesAsync(ct);
    }

    public async Task DeletePatientAsync(Guid id, CancellationToken ct = default)
    {
        var patient = await repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Patient with ID '{id}' not found.");
        repository.Delete(patient);
        await repository.SaveChangesAsync(ct);
    }

    public async Task<int> GetTotalPatientsCountAsync(CancellationToken ct = default) =>
        await repository.CountAsync(null, ct);

    private static PatientDetailsDto MapToDetails(Patient patient) => new(
        patient.Id, patient.FullName, patient.DateOfBirth, patient.Gender, patient.NationalId, patient.BloodType,
        patient.Address == null ? null : new AddressDto(patient.Address.Line1, patient.Address.Line2, patient.Address.City, patient.Address.State, patient.Address.PostalCode, patient.Address.Country),
        patient.Contacts.Select(c => new ContactDto(c.Type, c.Value, c.IsEmergency)).ToList()
    );
}
