using ClinicSystem.Application.Services;
using ClinicSystem.Application.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicSystem.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IVisitService, VisitService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);
        return services;
    }
}
