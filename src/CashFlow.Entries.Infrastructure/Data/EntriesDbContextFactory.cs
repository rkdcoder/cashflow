using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Entries.Infrastructure.Data
{
    public class EntriesDbContextFactory : IDesignTimeDbContextFactory<EntriesDbContext>
    {
        public EntriesDbContext CreateDbContext(string[] args)
        {
            // Use o caminho do diretório do projeto de infraestrutura
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EntriesDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("CashFlowDb"));

            return new EntriesDbContext(optionsBuilder.Options);
        }
    }
}