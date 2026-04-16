using System.ComponentModel.DataAnnotations;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class CreateModel(IUserService userService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.Password != Input.ConfirmPassword)
            ModelState.AddModelError(nameof(Input.ConfirmPassword), "Passwords do not match.");

        if (!ModelState.IsValid) return Page();

        try
        {
            var role = Enum.Parse<UserRole>(Input.Role);
            var dto = new UserCreateDto(Input.Username.Trim(), Input.Password, Input.FullName.Trim(), Input.Email?.Trim(), role);
            await userService.CreateUserAsync(dto);
            TempData["Success"] = $"User '{Input.Username}' created successfully.";
            return RedirectToPage("Index");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        public string Role { get; set; } = "Receptionist";

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
