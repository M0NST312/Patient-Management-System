using ClinicSystem.Domain.Common;
using ClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Patient>().HasIndex(p => p.NationalId).IsUnique();

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.Address).WithOne(a => a.Patient)
            .HasForeignKey<Address>(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Contacts).WithOne(c => c.Patient)
            .HasForeignKey(c => c.PatientId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Visits).WithOne(v => v.Patient)
            .HasForeignKey(v => v.PatientId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Invoices).WithOne(i => i.Patient)
            .HasForeignKey(i => i.PatientId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Visit>()
            .HasMany(v => v.Diagnoses).WithOne(d => d.Visit)
            .HasForeignKey(d => d.VisitId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Visit>()
            .HasMany(v => v.Prescriptions).WithOne(p => p.Visit)
            .HasForeignKey(p => p.VisitId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>().HasIndex(i => i.InvoiceNumber).IsUnique();

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.Items).WithOne(it => it.Invoice)
            .HasForeignKey(it => it.InvoiceId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.Payments).WithOne(p => p.Invoice)
            .HasForeignKey(p => p.InvoiceId).OnDelete(DeleteBehavior.Cascade);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        var now = DateTime.UtcNow;
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added) entry.Entity.CreatedAtUtc = now;
            if (entry.State is EntityState.Added or EntityState.Modified) entry.Entity.UpdatedAtUtc = now;
        }
    }
}
