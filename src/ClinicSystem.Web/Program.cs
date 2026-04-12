using ClinicSystem.Application.Extensions;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using ClinicSystem.Infrastructure.Data;
using ClinicSystem.Infrastructure.Extensions;
using ClinicSystem.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/clinic-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Layer registrations
    builder.Services
        .AddInfrastructureServices(builder.Configuration)
        .AddApplicationServices();

    // Authentication
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Account/Login";
        });

    builder.Services.AddAuthorization();

    builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AuthorizeFolder("/");
        options.Conventions.AllowAnonymousToPage("/Account/Login");
    });

    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Migrate and seed on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            db.Database.Migrate();
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User { Username = "admin", FullName = "Administrator", Email = "admin@clinic.com", PasswordHash = PasswordHasher.HashPassword("Admin@123"), Role = UserRole.Admin, IsActive = true },
                    new User { Username = "doctor", FullName = "Dr. John Smith", Email = "doctor@clinic.com", PasswordHash = PasswordHasher.HashPassword("Doctor@123"), Role = UserRole.Doctor, IsActive = true },
                    new User { Username = "receptionist", FullName = "Sarah Johnson", Email = "reception@clinic.com", PasswordHash = PasswordHasher.HashPassword("Reception@123"), Role = UserRole.Receptionist, IsActive = true }
                );
                db.SaveChanges();
                Log.Information("Seeded default users: admin, doctor, receptionist");
            }
        }
        catch (Exception ex) { Log.Warning(ex, "Database migration/seed failed — continuing startup"); }
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapRazorPages();
    app.MapOpenApi();
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", at = DateTime.UtcNow })).AllowAnonymous();

    app.Run();
}
catch (Exception ex) { Log.Fatal(ex, "Application failed to start"); }
finally { Log.CloseAndFlush(); }
