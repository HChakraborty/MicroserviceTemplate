namespace SampleAuthService.Application.DTO;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = default!;

    public int ExpiresAt { get; set; }
}
