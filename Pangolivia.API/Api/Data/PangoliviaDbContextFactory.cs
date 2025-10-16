using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pangolivia.API.Data
{
    // Allows `dotnet ef ...` to create the DbContext without building the whole host.
    public class PangoliviaDbContextFactory : IDesignTimeDbContextFactory<PangoliviaDbContext>
    {
        public PangoliviaDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddUserSecrets<Program>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Prefer config, then environment, then a safe local default for dev
            var conn =
                config.GetConnectionString("DefaultConnection")
                ?? System.Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=localhost,1433;Database=Pangolivia;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

            var optionsBuilder = new DbContextOptionsBuilder<PangoliviaDbContext>();
            optionsBuilder.UseSqlServer(conn);

            return new PangoliviaDbContext(optionsBuilder.Options);
        }
    }
}
