using ClinicSystem.Application.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Reports;

[Authorize(Roles = "Admin,Receptionist")]
public class IndexModel : PageModel
{
    private readonly IReportService _reportService;

    public IndexModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    [BindProperty]
    public DateTime? From { get; set; }

    [BindProperty]
    public DateTime? To { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostExportInvoicesAsync()
    {
        var invoices = await _reportService.GetInvoicesForReportAsync(From, To);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Invoices");
        var row = 1;
        ws.Cell(row, 1).Value = "InvoiceNumber";
        ws.Cell(row, 2).Value = "PatientName";
        ws.Cell(row, 3).Value = "CreatedAtUtc";
        ws.Cell(row, 4).Value = "TotalAmount";
        ws.Cell(row, 5).Value = "PaidAmount";
        ws.Cell(row, 6).Value = "Balance";
        ws.Cell(row, 7).Value = "Status";

        foreach (var inv in invoices)
        {
            row++;
            ws.Cell(row, 1).Value = inv.InvoiceNumber;
            ws.Cell(row, 2).Value = inv.PatientName;
            ws.Cell(row, 3).Value = inv.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm:ss");
            ws.Cell(row, 4).Value = inv.TotalAmount;
            ws.Cell(row, 5).Value = inv.PaidAmount;
            ws.Cell(row, 6).Value = inv.Balance;
            ws.Cell(row, 7).Value = inv.Status.ToString();
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        ms.Position = 0;
        var filename = $"invoices-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    public async Task<IActionResult> OnPostExportPatientsAsync()
    {
        var patients = await _reportService.GetPatientsForReportAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Patients");
        var row = 1;
        ws.Cell(row, 1).Value = "Id";
        ws.Cell(row, 2).Value = "FullName";
        ws.Cell(row, 3).Value = "DateOfBirth";
        ws.Cell(row, 4).Value = "Gender";
        ws.Cell(row, 5).Value = "NationalId";

        foreach (var p in patients)
        {
            row++;
            ws.Cell(row, 1).Value = p.Id.ToString();
            ws.Cell(row, 2).Value = p.FullName;
            ws.Cell(row, 3).Value = p.DateOfBirth.ToString("yyyy-MM-dd");
            ws.Cell(row, 4).Value = p.Gender.ToString();
            ws.Cell(row, 5).Value = p.NationalId;
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        ms.Position = 0;
        var filename = $"patients-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    public async Task<IActionResult> OnPostExportVisitsAsync()
    {
        var visits = await _reportService.GetVisitsForReportAsync(From, To);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Visits");
        var row = 1;
        ws.Cell(row, 1).Value = "VisitId";
        ws.Cell(row, 2).Value = "PatientName";
        ws.Cell(row, 3).Value = "VisitDate";
        ws.Cell(row, 4).Value = "DoctorName";
        ws.Cell(row, 5).Value = "Complaint";
        ws.Cell(row, 6).Value = "Status";

        foreach (var v in visits)
        {
            row++;
            ws.Cell(row, 1).Value = v.Id.ToString();
            ws.Cell(row, 2).Value = v.PatientName;
            ws.Cell(row, 3).Value = v.VisitDate.ToString("yyyy-MM-dd HH:mm:ss");
            ws.Cell(row, 4).Value = v.DoctorName;
            ws.Cell(row, 5).Value = v.Complaint;
            ws.Cell(row, 6).Value = v.Status.ToString();
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        ms.Position = 0;
        var filename = $"visits-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }
}
