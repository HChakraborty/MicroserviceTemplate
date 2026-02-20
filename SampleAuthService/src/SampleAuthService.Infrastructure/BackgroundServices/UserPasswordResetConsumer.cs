using SampleAuthService.Application.Events;
using Microsoft.Extensions.Hosting;
using SampleAuthService.Application.Interfaces;

namespace SampleAuthService.Infrastructure.BackgroundServices;

public class UserPasswordResetConsumer : BackgroundService { 
    private readonly IEventBus _eventBus; 
    
    public UserPasswordResetConsumer(IEventBus eventBus) { 
        _eventBus = eventBus; 
    } 
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) { 
        await _eventBus.Subscribe<UserPasswordResetEvent>(Handle); 
    } 
    
    private Task Handle(UserPasswordResetEvent evt) {
        Console.WriteLine($"User password reset event received: {evt.Email}"); 
        return Task.CompletedTask; 
    } 
}