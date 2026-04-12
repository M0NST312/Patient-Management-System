using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Patients;

[Authorize]
public class DetailsModel(IPatientService patientService, IVisitService visitService, IInvoiceService invoiceService) : PageModel
{
    public PatientDetailsDto? Patient { get; private set; }
    public List<VisitDetailsDto> Visits { get; private set; } = [];
    public List<InvoiceDetailsDto> Invoices { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Patient = await patientService.GetPatientByIdAsync(id);
        if (Patient is null) return NotFound();

        Visits = (await visitService.GetVisitsByPatientIdAsync(id)).ToList();
        Invoices = (await invoiceService.GetInvoicesByPatientIdAsync(id)).ToList();
        return Page();
    }
}
