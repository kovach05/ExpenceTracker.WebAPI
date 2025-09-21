using ExpenseTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExpenceTracker.Infrastructure;

public class ExpenseTrackerDbContextFactory : IDesignTimeDbContextFactory<ExpenseTrackerDbContext>
{
    public ExpenseTrackerDbContext CreateDbContext(string[] args)
    {
        // Вказуємо шлях до WebAPI, де лежить appsettings.json
        var configuration = new ConfigurationBuilder()
            // З папки Infrastructure піднімаємось на один рівень, потім заходимо у WebAPI
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ExpenceTracker.WebAPI"))
            .AddJsonFile("appsettings.json")
            .Build();


        var optionsBuilder = new DbContextOptionsBuilder<ExpenseTrackerDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString); // Або UseSqlServer, якщо SQL Server

        return new ExpenseTrackerDbContext(optionsBuilder.Options);
    }
}