namespace SampleAuthService.Application.DTO.TokenDto;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = default!;

    public int ExpiresAt { get; set; }
}
