using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Domain.Aggregates;
using ThatInterpretingAgency.Core.Domain.Entities;
using ThatInterpretingAgency.Core.Domain.ValueObjects;
using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Infrastructure.Persistence;

public class ThatInterpretingAgencyDbContext : IdentityDbContext
{
    public ThatInterpretingAgencyDbContext(DbContextOptions<ThatInterpretingAgencyDbContext> options) : base(options)
    {
    }

    public DbSet<AgencyAggregate> Agencies { get; set; }
    public DbSet<AgencyStaff> AgencyStaff { get; set; }
    public DbSet<Interpreter> Interpreters { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<AppointmentAggregate> Appointments { get; set; }
    public DbSet<InvoiceAggregate> Invoices { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<InterpreterRequest> InterpreterRequests { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Agency configuration
        modelBuilder.Entity<AgencyAggregate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // UserProfile configuration
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.MailingAddress).HasMaxLength(500);
            entity.Property(e => e.PhysicalAddress).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // AgencyStaff configuration
        modelBuilder.Entity<AgencyStaff>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.HasIndex(e => new { e.AgencyId, e.UserId, e.Role }).IsUnique();
            
            // Configure relationship with UserProfile
            entity.HasOne(d => d.UserProfile)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasPrincipalKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Interpreter configuration
        modelBuilder.Entity<Interpreter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Skills).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.Property(e => e.Availability).HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<AvailabilitySlot>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<AvailabilitySlot>());
            entity.Property(e => e.Status).HasConversion<string>();
            entity.HasIndex(e => new { e.AgencyId, e.UserId }).IsUnique();
            
            // Configure relationship with UserProfile
            entity.HasOne(d => d.UserProfile)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasPrincipalKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Client configuration
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.OrganizationName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Preferences).HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());
            entity.Property(e => e.Status).HasConversion<string>();
            entity.HasIndex(e => new { e.AgencyId, e.UserId }).IsUnique();
            
            // Configure relationship with UserProfile
            entity.HasOne(d => d.UserProfile)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasPrincipalKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Appointment configuration
        modelBuilder.Entity<AppointmentAggregate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TimeZone).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Rate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.HasIndex(e => new { e.AgencyId, e.InterpreterId, e.StartTime });
            entity.HasIndex(e => new { e.AgencyId, e.ClientId, e.StartTime });
        });

        // Invoice configuration
        modelBuilder.Entity<InvoiceAggregate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuickBooksInvoiceId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasIndex(e => new { e.AgencyId, e.QuickBooksInvoiceId }).IsUnique();
            entity.HasIndex(e => new { e.AgencyId, e.ClientId });
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.ExternalReference).HasMaxLength(100);
            entity.Property(e => e.Metadata).HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());
            entity.HasIndex(e => new { e.AgencyId, e.UserId, e.Status });
        });



        // Configure relationships
        modelBuilder.Entity<AgencyAggregate>()
            .HasMany(e => e.Staff)
            .WithOne()
            .HasForeignKey(e => e.AgencyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AgencyAggregate>()
            .HasMany(e => e.Interpreters)
            .WithOne()
            .HasForeignKey(e => e.AgencyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AgencyAggregate>()
            .HasMany(e => e.Clients)
            .WithOne()
            .HasForeignKey(e => e.AgencyId)
            .OnDelete(DeleteBehavior.Cascade);



        // InterpreterRequest configuration
        modelBuilder.Entity<InterpreterRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AppointmentType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.VirtualMeetingLink).HasMaxLength(500);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Mode).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(int.MaxValue);
            entity.Property(e => e.Language).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SpecialInstructions).HasMaxLength(int.MaxValue);
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Division).HasMaxLength(100);
            entity.Property(e => e.Program).HasMaxLength(100);
            entity.Property(e => e.LniContact).HasMaxLength(100);
            entity.Property(e => e.DayOfEventContact).HasMaxLength(100);
            entity.Property(e => e.DayOfEventContactPhone).HasMaxLength(50);
            entity.Property(e => e.CostCode).HasMaxLength(50);
            entity.Property(e => e.InvoiceApprover).HasMaxLength(100);
            
            // Constraints
            entity.HasCheckConstraint("CHK_InterpreterRequests_Mode", "Mode IN ('Consecutive', 'Simultaneous') OR Mode IS NULL");
            entity.HasCheckConstraint("CHK_InterpreterRequests_AppointmentType", "AppointmentType IN ('In-Person', 'Virtual')");
            entity.HasCheckConstraint("CHK_InterpreterRequests_Status", "Status IN ('Pending', 'Approved', 'Rejected', 'Fulfilled', 'Cancelled')");
            
            // Indexes
            entity.HasIndex(e => e.AgencyId);
            entity.HasIndex(e => e.RequestorId);
            entity.HasIndex(e => new { e.AgencyId, e.Status });
            entity.HasIndex(e => new { e.AgencyId, e.Language });
            entity.HasIndex(e => new { e.AgencyId, e.RequestedDate });
            
            // Relationships
            entity.HasOne(e => e.Agency)
                .WithMany()
                .HasForeignKey(e => e.AgencyId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Requestor)
                .WithMany()
                .HasForeignKey(e => e.RequestorId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Appointment)
                .WithMany()
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
