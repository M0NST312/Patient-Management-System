using System.ComponentModel.DataAnnotations;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class EditModel(IUserService userService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public string Username { get; private set; } = string.Empty;
    public bool NotFound { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        if (user is null) { NotFound = true; return Page(); }

        Username = user.Username;
        Input = new InputModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive
        };
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync(Guid id)
    {
        // Validate password only if provided
        if (!string.IsNullOrEmpty(Input.NewPassword))
        {
            if (Input.NewPassword.Length < 6)
                ModelState.AddModelError(nameof(Input.NewPassword), "Password must be at least 6 characters.");
            if (Input.NewPassword != Input.ConfirmPassword)
                ModelState.AddModelError(nameof(Input.ConfirmPassword), "Passwords do not match.");
        }
        else
        {
            ModelState.Remove(nameof(Input.NewPassword));
            ModelState.Remove(nameof(Input.ConfirmPassword));
        }

        if (!ModelState.IsValid)
        {
            var u = await userService.GetUserByIdAsync(id);
            if (u != null) Username = u.Username;
            return Page();
        }

        try
        {
            var role = Enum.Parse<UserRole>(Input.Role);
            await userService.UpdateUserAsync(id, new UserUpdateDto(Input.FullName.Trim(), Input.Email?.Trim(), role, Input.IsActive));

            if (!string.IsNullOrWhiteSpace(Input.NewPassword))
                await userService.ResetPasswordAsync(id, Input.NewPassword);

            TempData["Success"] = "User updated successfully.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var u = await userService.GetUserByIdAsync(id);
            if (u != null) Username = u.Username;
            return Page();
        }
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        public string Role { get; set; } = "Receptionist";

        [Display(Name = "Account Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
