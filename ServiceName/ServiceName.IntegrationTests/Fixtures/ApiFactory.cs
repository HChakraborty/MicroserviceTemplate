using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ServiceName.Application.Interfaces;
using ServiceName.Infrastructure.Persistence;
using ServiceName.IntegrationTests.Helpers;

namespace ServiceName.IntegrationTests.Fixtures;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly ContainersFixture _containers;
    private bool _databaseInitialized;

    public ApiFactory(ContainersFixture containers)
    {
        _containers = containers;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        // Override configuration
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                // Redis config is present for startup compatibility,
                // but cache implementation is replaced with in-memory for tests
                ["Redis:ConnectionString"] =
                    $"{_containers.Redis.Hostname}:{_containers.Redis.GetMappedPublicPort(6379)}",

                ["RabbitMq:HostName"] = _containers.RabbitMq.Hostname,
                ["RabbitMq:Port"] =
                    _containers.RabbitMq.GetMappedPublicPort(5672).ToString(),
                ["RabbitMq:UserName"] = "guest",
                ["RabbitMq:Password"] = "guest",

                ["Jwt:Key"] = "IntegrationTestKey_123456789012345",
                ["Jwt:Issuer"] = "SampleAuthService",
                ["Jwt:Audience"] = "SampleServices",
                ["Jwt:ExpireMinutes"] = "30"
            };

            config.AddInMemoryCollection(settings);
        });

        // Replace infrastructure services for tests
        builder.ConfigureServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(d =>
                    d.ServiceType ==
                    typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            var csBuilder = new SqlConnectionStringBuilder(
                _containers.Sql.GetConnectionString());

            csBuilder.InitialCatalog = "SampleAuthTestDb";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    csBuilder.ConnectionString,
                    o => o.EnableRetryOnFailure()));
        });

        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(IConfigureOptions<JwtBearerOptions>))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            // Remove default policy evaluator to ensure fake one is used
            services.RemoveAll<IPolicyEvaluator>();
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

            // Replace cache service with in-memory implementation
            services.RemoveAll<ICacheService>();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        if (_databaseInitialized) return;

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await db.Database.EnsureDeletedAsync();

        await db.Database.MigrateAsync();

        _databaseInitialized = true;
    }

    // Reset DB because tests run in parallel
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await db.Database.ExecuteSqlRawAsync(
            "EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

        await db.Database.ExecuteSqlRawAsync(
            "EXEC sp_msforeachtable 'DELETE FROM ?'");

        await db.Database.ExecuteSqlRawAsync(
            "EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
    }
}