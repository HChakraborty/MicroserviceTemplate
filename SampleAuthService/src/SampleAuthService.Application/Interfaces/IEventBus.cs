namespace SampleAuthService.Application.Interfaces.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T message);
    Task Subscribe<T>(Func<T, Task> handler);
}
