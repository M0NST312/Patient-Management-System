using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Billing;

[Authorize(Roles = "Admin,Receptionist")]
public class CreateModel(IInvoiceService invoiceService, IPatientService patientService) : PageModel
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
            var items = Input.Items
                .Where(i => !string.IsNullOrWhiteSpace(i.Description) && i.UnitPrice > 0 && i.Quantity > 0)
                .Select(i => new InvoiceItemDto(i.Description!, i.UnitPrice, i.Quantity))
                .ToList();

            if (!items.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one valid line item is required.");
                await LoadPatientsAsync();
                await SetPreselectedNameAsync();
                return Page();
            }

            var dto = new InvoiceCreateDto(Input.PatientId, Input.DiscountAmount, items);
            await invoiceService.CreateInvoiceAsync(dto);
            TempData["Success"] = "Invoice created successfully.";

            if (FromPatientId.HasValue)
                return RedirectToPage("/Patients/Details", new { id = FromPatientId.Value });

            return RedirectToPage("/Patients/Details", new { id = Input.PatientId });
        }
        catch (Exception ex) { ModelState.AddModelError(string.Empty, ex.Message); }

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

    public class LineItemInput
    {
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public Guid PatientId { get; set; }

        [Display(Name = "Discount Amount")]
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        public List<LineItemInput> Items { get; set; } = [new()];
    }
}
