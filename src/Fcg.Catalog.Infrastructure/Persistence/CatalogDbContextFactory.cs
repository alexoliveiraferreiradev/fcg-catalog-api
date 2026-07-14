using Fcg.Core.Abstractions.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Fcg.Catalog.Infrastructure.Persistence
{
    public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

            var host = configuration["DatabaseSettings:Host"] ?? "localhost";
            var port = configuration["DatabaseSettings:Port"] ?? "1433";
            var dbName = configuration["DatabaseSettings:DatabaseName"];
            var user = configuration["DatabaseSettings:Username"];
            var password = configuration["DatabaseSettings:Password"];

            if (string.IsNullOrEmpty(host))
                throw new DomainException("Faltam variáveis de ambiente de Banco de Dados para rodar a migração.");

            var connectionString = $"Data Source={host},{port};Initial Catalog={dbName};User ID={user};Password={password};TrustServerCertificate=True";

            var builder = new DbContextOptionsBuilder<CatalogDbContext>();
            builder.UseSqlServer(connectionString);

            return new CatalogDbContext(builder.Options);
        }
    }
}
