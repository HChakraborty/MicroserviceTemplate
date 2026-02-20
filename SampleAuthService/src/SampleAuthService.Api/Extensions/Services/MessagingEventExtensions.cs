using SampleAuthService.Infrastructure.BackgroundServices;

namespace SampleAuthService.Api.Extensions.Services;

public static class MessagingEventExtensions
{
    public static IServiceCollection AddMessagingEvent(this IServiceCollection services)
    {
        services.AddHostedService<UserCreatedConsumer>();
        services.AddHostedService<UserDeletedConsumer>();
        services.AddHostedService<UserPasswordResetConsumer>();

        return services;
    }
}
