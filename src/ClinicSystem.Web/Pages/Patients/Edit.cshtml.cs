using System.ComponentModel.DataAnnotations;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicSystem.Web.Pages.Patients;

[Authorize(Roles = "Admin,Receptionist")]
public class EditModel(IPatientService patientService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public Guid PatientId { get; private set; }

    public List<SelectListItem> GenderOptions => Enum.GetValues<Gender>()
        .Select(g => new SelectListItem(g.ToString(), g.ToString())).ToList();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var patient = await patientService.GetPatientByIdAsync(id);
        if (patient is null) return NotFound();

        PatientId = id;
        Input = new InputModel
        {
            FullName = patient.FullName,
            NationalId = patient.NationalId,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            BloodType = patient.BloodType,
            AddressLine1 = patient.Address?.Line1,
            AddressLine2 = patient.Address?.Line2,
            City = patient.Address?.City,
            State = patient.Address?.State,
            PostalCode = patient.Address?.PostalCode,
            Country = patient.Address?.Country,
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        PatientId = id;
        if (!ModelState.IsValid) return Page();

        try
        {
            AddressDto? address = null;
            if (!string.IsNullOrWhiteSpace(Input.AddressLine1) && !string.IsNullOrWhiteSpace(Input.City) && !string.IsNullOrWhiteSpace(Input.Country))
                address = new AddressDto(Input.AddressLine1!, Input.AddressLine2, Input.City!, Input.State, Input.PostalCode, Input.Country!);

            var dto = new PatientUpdateDto(Input.FullName, Input.DateOfBirth, Input.Gender, Input.NationalId,
                string.IsNullOrWhiteSpace(Input.BloodType) ? null : Input.BloodType, address, null);

            await patientService.UpdatePatientAsync(id, dto);
            TempData["Success"] = "Patient information updated successfully.";
            return RedirectToPage("Details", new { id });
        }
        catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); return Page(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")] public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "National ID is required.")]
        [Display(Name = "National ID")] public string NationalId { get; set; } = string.Empty;
        [Required][Display(Name = "Date of Birth")] public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        [Required] public Gender Gender { get; set; } = Gender.Unknown;
        [Display(Name = "Blood Type")] public string? BloodType { get; set; }
        [Display(Name = "Street Address")] public string? AddressLine1 { get; set; }
        [Display(Name = "Apt / Suite")] public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Postal Code")] public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
