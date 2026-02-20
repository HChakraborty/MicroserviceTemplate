using SampleAuthService.Application.Interfaces.Security;
using SampleAuthService.Application.Interfaces.Services;
using SampleAuthService.Application.Services;
using SampleAuthService.Infrastructure.Security;

namespace SampleAuthService.Api.Extensions.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
