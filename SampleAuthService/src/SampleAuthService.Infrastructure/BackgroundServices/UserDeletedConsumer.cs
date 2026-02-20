using SampleAuthService.Application.Events;
using Microsoft.Extensions.Hosting;
using SampleAuthService.Application.Interfaces;

namespace SampleAuthService.Infrastructure.BackgroundServices;

public class UserDeletedConsumer : BackgroundService { 
    private readonly IEventBus _eventBus; 
    
    public UserDeletedConsumer(IEventBus eventBus) { 
        _eventBus = eventBus; 
    } 
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) { 
        await _eventBus.Subscribe<UserDeletedEvent>(Handle); 
    } 
    
    private Task Handle(UserDeletedEvent evt) {
        Console.WriteLine($"User deleted event received: {evt.Email}"); 
        return Task.CompletedTask; 
    } 
}