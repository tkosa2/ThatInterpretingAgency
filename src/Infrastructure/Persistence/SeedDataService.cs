using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Domain.Aggregates;
using ThatInterpretingAgency.Core.Domain.Entities;
using ThatInterpretingAgency.Core.Domain.ValueObjects;
using ThatInterpretingAgency.Core.Domain.Common;

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

            // Seed UserProfiles
            var userProfile1 = UserProfile.Create(
                "user1@example.com", // This would be the actual AspNetUsers.Id
                "Dr. Sarah",
                "Johnson",
                mailingAddress: "123 Medical Center Dr",
                city: "Springfield",
                state: "IL",
                zipCode: "62701"
            );

            var userProfile2 = UserProfile.Create(
                "user2@example.com",
                "Attorney Mike",
                "Davis",
                mailingAddress: "456 Legal Ave",
                city: "Chicago",
                state: "IL",
                zipCode: "60601"
            );

            var userProfile3 = UserProfile.Create(
                "user3@example.com",
                "Maria",
                "Rodriguez",
                mailingAddress: "789 Business Blvd",
                city: "Miami",
                state: "FL",
                zipCode: "33101"
            );

            var userProfile4 = UserProfile.Create(
                "user4@example.com",
                "Carlos",
                "Mendez",
                mailingAddress: "321 Interpreter St",
                city: "Los Angeles",
                state: "CA",
                zipCode: "90001"
            );

            var userProfile5 = UserProfile.Create(
                "user5@example.com",
                "Marie",
                "Dubois",
                mailingAddress: "654 Language Ln",
                city: "New Orleans",
                state: "LA",
                zipCode: "70112"
            );

            var userProfile6 = UserProfile.Create(
                "user6@example.com",
                "Ana",
                "Silva",
                mailingAddress: "987 Translation Tr",
                city: "San Antonio",
                state: "TX",
                zipCode: "78201"
            );

            _context.UserProfiles.AddRange(userProfile1, userProfile2, userProfile3, userProfile4, userProfile5, userProfile6);

            // Seed Clients
            var client1 = Client.Create(
                agency1.Id,
                "user1@example.com",
                "City General Hospital",
                new Dictionary<string, string>
                {
                    { "PreferredLanguage", "Spanish" },
                    { "MedicalSpecialty", "Cardiology" },
                    { "PhoneNumber", "+1-555-0101" },
                    { "Email", "dr.johnson@cityhospital.com" }
                }
            );

            var client2 = Client.Create(
                agency1.Id,
                "user2@example.com",
                "Davis & Associates Law Firm",
                new Dictionary<string, string>
                {
                    { "PreferredLanguage", "French" },
                    { "LegalSpecialty", "Criminal Defense" },
                    { "PhoneNumber", "+1-555-0102" },
                    { "Email", "mike.davis@davislaw.com" }
                }
            );

            var client3 = Client.Create(
                agency2.Id,
                "user3@example.com",
                "International Business Corp",
                new Dictionary<string, string>
                {
                    { "PreferredLanguage", "Portuguese" },
                    { "BusinessType", "Import/Export" },
                    { "PhoneNumber", "+1-555-0103" },
                    { "Email", "m.rodriguez@ibcorp.com" }
                }
            );

            _context.Clients.AddRange(client1, client2, client3);

            // Seed Interpreters
            var interpreter1 = Interpreter.Create(
                agency1.Id,
                "user4@example.com",
                new List<string> { "Spanish", "English" }
            );

            var interpreter2 = Interpreter.Create(
                agency1.Id,
                "user5@example.com",
                new List<string> { "French", "English" }
            );

            var interpreter3 = Interpreter.Create(
                agency2.Id,
                "user6@example.com",
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
