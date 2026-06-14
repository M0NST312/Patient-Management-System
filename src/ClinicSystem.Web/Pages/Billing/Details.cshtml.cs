using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Application.Validators;
using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Billing;

[Authorize(Roles = "Admin,Receptionist")]
public class DetailsModel(IInvoiceService invoiceService, ILogger<DetailsModel> logger) : PageModel
{
    public InvoiceDetailsDto? Invoice { get; private set; }
    public string? PaymentError { get; private set; }
    private readonly ILogger<DetailsModel> _logger = logger;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Invoice = await invoiceService.GetInvoiceByIdAsync(id);
        return Invoice is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAddPaymentAsync(Guid id, decimal amount, string method)
    {
        try
        {
            var dto = new PaymentCreateDto(amount, method);
            var validator = new PaymentCreateValidator();
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                Invoice = await invoiceService.GetInvoiceByIdAsync(id);
                PaymentError = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return Page();
            }

            await invoiceService.AddPaymentAsync(id, dto);
            TempData["Success"] = $"Payment of E{amount:F2} recorded successfully.";
            return RedirectToPage(new { id });
        }
        catch (KeyNotFoundException ex)
        {
            Invoice = await invoiceService.GetInvoiceByIdAsync(id);
            PaymentError = ex.Message;
            return Page();
        }
        catch (InvalidOperationException ex)
        {
            Invoice = await invoiceService.GetInvoiceByIdAsync(id);
            PaymentError = ex.Message;
            return Page();
        }
        catch (ArgumentException ex)
        {
            Invoice = await invoiceService.GetInvoiceByIdAsync(id);
            PaymentError = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            Invoice = await invoiceService.GetInvoiceByIdAsync(id);
            _logger.LogError(ex, "Unexpected error while adding payment for InvoiceId={InvoiceId}", id);
            PaymentError = "An unexpected error occurred while recording the payment. Please try again.";
            return Page();
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        try
        {
            await invoiceService.DeleteInvoiceAsync(id);
            TempData["Success"] = "Invoice deleted.";
            return RedirectToPage("Index");
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToPage(new { id });
        }
    }
}
