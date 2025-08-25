using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ThatInterpretingAgency.Infrastructure.Persistence;

public class ThatInterpretingAgencyDbContextFactory : IDesignTimeDbContextFactory<ThatInterpretingAgencyDbContext>
{
    public ThatInterpretingAgencyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ThatInterpretingAgencyDbContext>();
        
        // Use the same connection string as the application
        var connectionString = "Server=localhost;Database=ThatInterpretingAgency;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;Encrypt=false";
        
        optionsBuilder.UseSqlServer(connectionString);

        return new ThatInterpretingAgencyDbContext(optionsBuilder.Options);
    }
}
