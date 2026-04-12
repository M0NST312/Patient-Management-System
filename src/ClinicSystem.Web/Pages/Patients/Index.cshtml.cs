using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Patients;

[Authorize]
public class IndexModel(IPatientService patientService) : PageModel
{
    public List<PatientDetailsDto> Patients { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync()
    {
        Patients = string.IsNullOrWhiteSpace(Search)
            ? (await patientService.GetAllPatientsAsync()).ToList()
            : (await patientService.SearchPatientsByNameAsync(Search)).ToList();
    }
}
