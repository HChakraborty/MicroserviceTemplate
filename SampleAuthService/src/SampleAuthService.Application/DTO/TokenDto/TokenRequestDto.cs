using System.ComponentModel.DataAnnotations;

namespace SampleAuthService.Application.DTO.TokenDto;

public class TokenRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}
