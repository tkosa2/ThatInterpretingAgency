using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ThatInterpretingAgency.Infrastructure.Persistence;

public class ThatInterpretingAgencyDbContextFactory : IDesignTimeDbContextFactory<ThatInterpretingAgencyDbContext>
{
    public ThatInterpretingAgencyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ThatInterpretingAgencyDbContext>();
        
        // Use a connection string that can be resolved at design time
        // You can modify this connection string as needed for your environment
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ThatInterpretingAgency;Trusted_Connection=true;MultipleActiveResultSets=true";
        
        optionsBuilder.UseSqlServer(connectionString);

        return new ThatInterpretingAgencyDbContext(optionsBuilder.Options);
    }
}
