using SampleAuthService.Domain.Entities;

namespace SampleAuthService.Application.Interfaces.Security;

public interface IJwtService
{
    string GenerateToken(User user);
}
