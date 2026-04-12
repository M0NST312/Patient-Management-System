using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Visits;

[Authorize]
public class IndexModel(IVisitService visitService) : PageModel
{
    public List<VisitDetailsDto> Visits { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(StatusFilter) && Enum.TryParse<VisitStatus>(StatusFilter, out var status))
            Visits = (await visitService.GetVisitsByStatusAsync(status)).ToList();
        else
            Visits = (await visitService.GetAllVisitsAsync()).OrderByDescending(v => v.VisitDate).ToList();
    }
}
