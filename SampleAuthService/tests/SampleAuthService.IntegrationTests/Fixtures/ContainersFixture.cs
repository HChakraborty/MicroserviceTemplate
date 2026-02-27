using SampleAuthService.IntegrationTests.Helpers;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace SampleAuthService.IntegrationTests.Fixtures;

public class ContainersFixture : IAsyncLifetime
{
    public MsSqlContainer Sql { get; } =
        new MsSqlBuilder(ContainerImages.Sql)
            .WithPassword("Your_strong_password123")
            .Build();

    public RedisContainer Redis { get; } =
        new RedisBuilder(ContainerImages.Redis)
            .Build();

    public RabbitMqContainer RabbitMq { get; } =
        new RabbitMqBuilder(ContainerImages.RabbitMq)
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();

    public async Task InitializeAsync()
    {
        await Sql.StartAsync();
        await Redis.StartAsync();
        await RabbitMq.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Sql.DisposeAsync();
        await Redis.DisposeAsync();
        await RabbitMq.DisposeAsync();
    }
}