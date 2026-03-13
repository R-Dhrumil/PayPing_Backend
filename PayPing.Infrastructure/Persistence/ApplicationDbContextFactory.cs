using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PayPing.Infrastructure.Persistence;

namespace PayPing.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Use a dummy connection string for migration creation if the real one isn't available or causing issues
            optionsBuilder.UseNpgsql("Host=localhost;Database=payping_db;Username=postgres;Password=postgres", 
                o => o.MigrationsAssembly("PayPing.Infrastructure")
                      .SetPostgresVersion(new Version(15, 0)));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
