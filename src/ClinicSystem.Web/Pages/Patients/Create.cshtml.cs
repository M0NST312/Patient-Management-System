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
public class CreateModel(IPatientService patientService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> GenderOptions => Enum.GetValues<Gender>()
        .Select(g => new SelectListItem(g.ToString(), g.ToString())).ToList();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            AddressDto? address = null;
            if (!string.IsNullOrWhiteSpace(Input.AddressLine1) && !string.IsNullOrWhiteSpace(Input.City) && !string.IsNullOrWhiteSpace(Input.Country))
                address = new AddressDto(Input.AddressLine1, Input.AddressLine2, Input.City, Input.State, Input.PostalCode, Input.Country);

            List<ContactDto>? contacts = null;
            if (!string.IsNullOrWhiteSpace(Input.ContactType) && !string.IsNullOrWhiteSpace(Input.ContactValue))
                contacts = [new ContactDto(Input.ContactType, Input.ContactValue, Input.IsEmergencyContact)];

            var dto = new PatientCreateDto(
                Input.FullName, Input.DateOfBirth, Input.Gender, Input.NationalId,
                string.IsNullOrWhiteSpace(Input.BloodType) ? null : Input.BloodType,
                address, contacts);

            var id = await patientService.CreatePatientAsync(dto);
            TempData["Success"] = $"Patient '{Input.FullName}' registered successfully.";
            return RedirectToPage("Details", new { id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(200)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "National ID is required.")]
        [MaxLength(100)]
        [Display(Name = "National ID")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "Date of Birth")]
        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));

        [Required]
        public Gender Gender { get; set; } = Gender.Unknown;

        [Display(Name = "Blood Type")]
        public string? BloodType { get; set; }

        [Display(Name = "Street Address")]
        public string? AddressLine1 { get; set; }
        [Display(Name = "Apartment / Suite")]
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        [Display(Name = "Contact Type")]
        public string? ContactType { get; set; }
        [Display(Name = "Contact Value")]
        public string? ContactValue { get; set; }
        [Display(Name = "Emergency Contact")]
        public bool IsEmergencyContact { get; set; }
    }
}
