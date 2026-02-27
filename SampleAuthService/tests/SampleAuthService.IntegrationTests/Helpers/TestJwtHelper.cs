using Microsoft.Extensions.Options;
using SampleAuthService.Domain.Entities;
using SampleAuthService.Domain.Enums;
using SampleAuthService.Infrastructure.Security;

namespace SampleAuthService.IntegrationTests.Helpers;

// Generates a valid JWT for integration tests to simulate authenticated requests with specific roles.
public static class TestJwtHelper
{
    public static string GenerateToken(
        string email,
        UserRole role)
    {
        var options = Options.Create(new JwtOptions
        {
            Key = "IntegrationTestKey_123456789012345",
            Issuer = "SampleAuthService",
            Audience = "SampleServices",
            ExpireMinutes = 30
        });

        var jwtService = new JwtService(options);

        var user = new User(email, "dummy-hash", role);

        return jwtService.GenerateToken(user);
    }
}
