using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Domain.Aggregates;
using ThatInterpretingAgency.Core.Domain.Entities;
using ThatInterpretingAgency.Core.Domain.ValueObjects;

namespace ThatInterpretingAgency.Infrastructure.Persistence;

public class SeedDataService
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public SeedDataService(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        try
        {
            // Check if data already exists
            if (await _context.Agencies.AnyAsync())
            {
                return; // Data already seeded
            }

            // Seed Agencies
            var agency1 = AgencyAggregate.Create(
                "That Interpreting Agency",
                "Professional interpreting services for healthcare, legal, and business needs"
            );

            var agency2 = AgencyAggregate.Create(
                "Global Language Solutions",
                "Comprehensive language services for international organizations"
            );

            _context.Agencies.AddRange(agency1, agency2);

            // Seed Clients
            var client1 = Client.Create(
                agency1.Id,
                Guid.NewGuid(),
                "City General Hospital",
                new Dictionary<string, string>
                {
                    { "PreferredLanguage", "Spanish" },
                    { "MedicalSpecialty", "Cardiology" },
                    { "ContactPerson", "Dr. Sarah Johnson" },
                    { "PhoneNumber", "+1-555-0101" },
                    { "Email", "dr.johnson@cityhospital.com" }
                }
            );

            var client2 = Client.Create(
                agency1.Id,
                Guid.NewGuid(),
                "Davis & Associates Law Firm",
                new Dictionary<string, string>
                {
                    { "PreferredLanguage", "French" },
                    { "LegalSpecialty", "Criminal Defense" },
                    { "ContactPerson", "Attorney Mike Davis" },
                    { "PhoneNumber", "+1-555-0102" },
                    { "Email", "mike.davis@davislaw.com" }
                }
            );

            var client3 = Client.Create(
                agency2.Id,
                Guid.NewGuid(),
                "International Business Corp",
                new Dictionary<string, string>
                {
                    { "PreferredLanguage", "Portuguese" },
                    { "BusinessType", "Import/Export" },
                    { "ContactPerson", "Maria Rodriguez" },
                    { "PhoneNumber", "+1-555-0103" },
                    { "Email", "m.rodriguez@ibcorp.com" }
                }
            );

            _context.Clients.AddRange(client1, client2, client3);

            // Seed Interpreters
            var interpreter1 = Interpreter.Create(
                agency1.Id,
                Guid.NewGuid(),
                "Carlos Mendez",
                new List<string> { "Spanish", "English" }
            );

            var interpreter2 = Interpreter.Create(
                agency1.Id,
                Guid.NewGuid(),
                "Marie Dubois",
                new List<string> { "French", "English" }
            );

            var interpreter3 = Interpreter.Create(
                agency2.Id,
                Guid.NewGuid(),
                "Ana Silva",
                new List<string> { "Portuguese", "Spanish", "English" }
            );

            _context.Interpreters.AddRange(interpreter1, interpreter2, interpreter3);

            // Seed Interpreter Requests
            var request1 = new InterpreterRequest(
                agency1.Id,
                client1.Id,
                "Virtual",
                DateTime.Today.AddDays(7),
                DateTime.Today.AddDays(7).AddHours(9),
                DateTime.Today.AddDays(7).AddHours(11),
                "Spanish",
                "Medical consultation with Spanish-speaking patient",
                "Consecutive",
                "Virtual Meeting",
                "https://meet.google.com/abc-defg-hij",
                "Medical terminology expertise required",
                "Healthcare",
                "Patient Care",
                "Dr. Johnson",
                "Dr. Sarah Johnson",
                "+1-555-0101",
                "MED001",
                "Dr. Johnson"
            );

            var request2 = new InterpreterRequest(
                agency1.Id,
                client2.Id,
                "In-Person",
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(3).AddHours(14),
                DateTime.Today.AddDays(3).AddHours(16),
                "French",
                "Legal deposition for French-speaking witness",
                "Simultaneous",
                "123 Main Street, Court Building",
                null,
                "Legal terminology expertise required",
                "Legal Services",
                "Court Proceedings",
                "Attorney Davis",
                "Attorney Mike Davis",
                "+1-555-0102",
                "LEG001",
                "Attorney Davis"
            );

            var request3 = new InterpreterRequest(
                agency2.Id,
                client3.Id,
                "Virtual",
                DateTime.Today.AddDays(5),
                DateTime.Today.AddDays(5).AddHours(10),
                DateTime.Today.AddDays(5).AddHours(12),
                "Portuguese",
                "Business meeting with Portuguese-speaking partners",
                "Consecutive",
                "Virtual Meeting",
                "https://zoom.us/j/123456789",
                "Business terminology expertise required",
                "International Business",
                "Partnership Development",
                "Ms. Rodriguez",
                "Maria Rodriguez",
                "+1-555-0103",
                "BUS001",
                "Ms. Rodriguez"
            );

            _context.InterpreterRequests.AddRange(request1, request2, request3);

            // Save all changes
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't throw - this is seed data
            Console.WriteLine($"Error seeding data: {ex.Message}");
        }
    }
}
