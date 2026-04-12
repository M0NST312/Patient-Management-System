using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Visits;

[Authorize(Roles = "Admin,Doctor")]
public class CreateModel(IVisitService visitService, IPatientService patientService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    [BindProperty] public Guid? FromPatientId { get; set; }
    public string PatientsJson { get; private set; } = "[]";
    public string? PreselectedPatientName { get; private set; }

    public async Task OnGetAsync(Guid? patientId)
    {
        FromPatientId = patientId;
        await LoadPatientsAsync();

        if (patientId.HasValue)
        {
            Input.PatientId = patientId.Value;
            var patient = await patientService.GetPatientByIdAsync(patientId.Value);
            PreselectedPatientName = patient?.FullName;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) { await LoadPatientsAsync(); await SetPreselectedNameAsync(); return Page(); }

        try
        {
            var dto = new VisitCreateDto(
                Input.PatientId,
                Input.VisitDate.ToUniversalTime(),
                Input.DoctorName,
                Input.Complaint,
                string.IsNullOrWhiteSpace(Input.Notes) ? null : Input.Notes,
                Enum.Parse<VisitStatus>(Input.Status));

            await visitService.CreateVisitAsync(dto);
            TempData["Success"] = "Visit recorded successfully.";

            if (FromPatientId.HasValue)
                return RedirectToPage("/Patients/Details", new { id = FromPatientId.Value });

            return RedirectToPage("/Patients/Details", new { id = Input.PatientId });
        }
        catch (KeyNotFoundException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
        catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }

        await LoadPatientsAsync();
        await SetPreselectedNameAsync();
        return Page();
    }

    private async Task LoadPatientsAsync()
    {
        var patients = await patientService.GetAllPatientsAsync();
        var list = patients.OrderBy(p => p.FullName)
            .Select(p => new { id = p.Id, name = p.FullName, nationalId = p.NationalId });
        PatientsJson = JsonSerializer.Serialize(list);
    }

    private async Task SetPreselectedNameAsync()
    {
        if (Input.PatientId != Guid.Empty)
        {
            var p = await patientService.GetPatientByIdAsync(Input.PatientId);
            PreselectedPatientName = p?.FullName;
        }
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Doctor name is required.")]
        [Display(Name = "Doctor")]
        public string DoctorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Visit date is required.")]
        [Display(Name = "Visit Date & Time")]
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Complaint is required.")]
        [Display(Name = "Complaint / Reason")]
        public string Complaint { get; set; } = string.Empty;

        [Display(Name = "Clinical Notes")]
        public string? Notes { get; set; }

        public string Status { get; set; } = "Scheduled";
    }
}
