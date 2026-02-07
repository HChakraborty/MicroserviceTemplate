using Microsoft.EntityFrameworkCore;
using ServiceName.Application.Interfaces;
using ServiceName.Application.Services;
using ServiceName.Domain.Entities;
using ServiceName.Domain.Interfaces;
using ServiceName.Infrastructure.Persistence;
using ServiceName.Infrastructure.Repositories;

namespace ServiceName.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISampleService, SampleService>();

        return services;
    }
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("ServiceNameDb"));

        services.AddScoped<IRepository<SampleEntity>, SampleRepository>();

        return services;
    }
}
