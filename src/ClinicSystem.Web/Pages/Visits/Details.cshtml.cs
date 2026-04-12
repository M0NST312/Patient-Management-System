using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Visits;

[Authorize]
public class DetailsModel(IVisitService visitService) : PageModel
{
    public VisitDetailsDto? Visit { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Visit = await visitService.GetVisitByIdAsync(id);
        return Visit is null ? NotFound() : Page();
    }

    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> OnPostAddDiagnosisAsync(Guid id, string diagnosisDescription, string? icd10Code)
    {
        if (string.IsNullOrWhiteSpace(diagnosisDescription))
        {
            TempData["Error"] = "Diagnosis description is required.";
            return RedirectToPage(new { id });
        }
        await visitService.AddDiagnosisAsync(id, new DiagnosisCreateDto(diagnosisDescription, icd10Code));
        TempData["Success"] = "Diagnosis added.";
        return RedirectToPage(new { id });
    }

    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> OnPostRemoveDiagnosisAsync(Guid id, Guid diagnosisId)
    {
        await visitService.RemoveDiagnosisAsync(diagnosisId);
        TempData["Success"] = "Diagnosis removed.";
        return RedirectToPage(new { id });
    }

    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> OnPostAddPrescriptionAsync(Guid id, string medicationName, string? dosage, string? instructions)
    {
        if (string.IsNullOrWhiteSpace(medicationName))
        {
            TempData["Error"] = "Medication name is required.";
            return RedirectToPage(new { id });
        }
        await visitService.AddPrescriptionAsync(id, new PrescriptionCreateDto(medicationName, dosage, instructions));
        TempData["Success"] = "Prescription added.";
        return RedirectToPage(new { id });
    }

    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> OnPostRemovePrescriptionAsync(Guid id, Guid prescriptionId)
    {
        await visitService.RemovePrescriptionAsync(prescriptionId);
        TempData["Success"] = "Prescription removed.";
        return RedirectToPage(new { id });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await visitService.DeleteVisitAsync(id);
        TempData["Success"] = "Visit deleted.";
        return RedirectToPage("/Visits/Index");
    }
}
