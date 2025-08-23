using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ThatInterpretingAgency.Infrastructure.Persistence;
using ThatInterpretingAgency.Infrastructure.Services;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Infrastructure.Persistence.Repositories;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework Core
builder.Services.AddDbContext<ThatInterpretingAgencyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ThatInterpretingAgencyDbContext>()
    .AddDefaultTokenProviders();

// Add MediatR
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddMediatR(typeof(ThatInterpretingAgency.Core.Application.Commands.CreateAgency.CreateAgencyCommand).Assembly);

// Add Repository services
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IAgencyStaffRepository, AgencyStaffRepository>();
builder.Services.AddScoped<IInterpreterRepository, InterpreterRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInterpreterRequestRepository, InterpreterRequestRepository>();

// Add Application services
builder.Services.AddScoped<ThatInterpretingAgency.Core.Application.Common.IInterpreterRequestService, ThatInterpretingAgency.Core.Application.Services.InterpreterRequestService>();
builder.Services.AddScoped<IAgencyUniquenessService, AgencyUniquenessService>();

// Add Seed Data service
builder.Services.AddScoped<SeedDataService>();

// Add QuickBooks service
builder.Services.Configure<QuickBooksOptions>(
    builder.Configuration.GetSection("QuickBooks"));
builder.Services.AddScoped<IQuickBooksService, QuickBooksService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Try to ensure database is created and seed data, but continue if database is not available
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ThatInterpretingAgencyDbContext>();
        
        // Check if InterpreterRequests table exists, if not create it
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }
        else
        {
            // Check if InterpreterRequests table exists
            var tableExists = context.Database.SqlQueryRaw<int>($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'InterpreterRequests'").FirstOrDefault();
            if (tableExists == 0)
            {
                // Create just the InterpreterRequests table
                context.Database.ExecuteSqlRaw(@"
                    CREATE TABLE [InterpreterRequests] (
                        [Id] uniqueidentifier NOT NULL,
                        [AgencyId] uniqueidentifier NOT NULL,
                        [RequestorId] uniqueidentifier NOT NULL,
                        [AppointmentType] nvarchar(50) NOT NULL,
                        [VirtualMeetingLink] nvarchar(500) NULL,
                        [Location] nvarchar(500) NULL,
                        [Mode] nvarchar(50) NULL,
                        [Description] nvarchar(max) NULL,
                        [RequestedDate] datetime2 NOT NULL,
                        [StartTime] datetime2 NOT NULL,
                        [EndTime] datetime2 NOT NULL,
                        [Language] nvarchar(100) NOT NULL,
                        [SpecialInstructions] nvarchar(max) NULL,
                        [Status] nvarchar(50) NOT NULL,
                        [Division] nvarchar(100) NULL,
                        [Program] nvarchar(100) NULL,
                        [LniContact] nvarchar(100) NULL,
                        [DayOfEventContact] nvarchar(100) NULL,
                        [DayOfEventContactPhone] nvarchar(50) NULL,
                        [CostCode] nvarchar(50) NULL,
                        [InvoiceApprover] nvarchar(100) NULL,
                        [SupportingMaterials] bit NOT NULL,
                        [AppointmentId] uniqueidentifier NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_InterpreterRequests] PRIMARY KEY ([Id])
                    );
                    
                    CREATE INDEX [IX_InterpreterRequests_AgencyId] ON [InterpreterRequests] ([AgencyId]);
                    CREATE INDEX [IX_InterpreterRequests_RequestorId] ON [InterpreterRequests] ([RequestorId]);
                    CREATE INDEX [IX_InterpreterRequests_AgencyId_Status] ON [InterpreterRequests] ([AgencyId], [Status]);
                    CREATE INDEX [IX_InterpreterRequests_AgencyId_Language] ON [InterpreterRequests] ([AgencyId], [Language]);
                    CREATE INDEX [IX_InterpreterRequests_AgencyId_RequestedDate] ON [InterpreterRequests] ([AgencyId], [RequestedDate]);
                ");
            }
        }
        
        // Seed initial data
        var seedService = scope.ServiceProvider.GetRequiredService<SeedDataService>();
        await seedService.SeedDataAsync();
    }
}
catch (Exception ex)
{
    // Log the error but continue - the API will use mock data fallbacks
    Console.WriteLine($"Database initialization failed: {ex.Message}");
    Console.WriteLine("API will continue with mock data fallbacks for testing.");
}

app.Run();
