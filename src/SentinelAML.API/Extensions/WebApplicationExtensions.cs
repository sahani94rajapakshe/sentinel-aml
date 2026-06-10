using Microsoft.EntityFrameworkCore;
using SentinelAML.Persistence.Data;

namespace SentinelAML.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task VerifyDatabaseConnectionAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            if (!await context.Database.CanConnectAsync())
            {
                throw new InvalidOperationException(
                    $"PostgreSQL connection failed for database '{context.Database.GetDbConnection().Database}'. " +
                    "Ensure PostgreSQL is running, the database exists, and the password is correct.");
            }

            logger.LogInformation(
                "PostgreSQL connection verified for database '{Database}'.",
                context.Database.GetDbConnection().Database);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unable to connect to PostgreSQL. Verify PostgreSQL is running, the database exists, and the connection string password is correct. " +
                "Set your password with: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"Host=localhost;Port=5432;Database=SentinelAMLDb;Username=postgres;Password=YOUR_PASSWORD\" --project src/SentinelAML.API");
            throw;
        }
    }
}
