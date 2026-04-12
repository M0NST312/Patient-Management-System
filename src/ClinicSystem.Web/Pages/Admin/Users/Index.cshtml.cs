using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class IndexModel(IUserService userService) : PageModel
{
    public IEnumerable<UserDto> Users { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Users = await userService.GetAllUsersAsync();
    }

    public async Task<IActionResult> OnPostToggleAsync(Guid id)
    {
        try
        {
            await userService.ToggleActiveAsync(id);
            TempData["Success"] = "User status updated.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        try
        {
            await userService.DeleteUserAsync(id);
            TempData["Success"] = "User deleted successfully.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }
}
