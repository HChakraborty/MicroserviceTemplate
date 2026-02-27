using SampleAuthService.Domain.Enums;

namespace SampleAuthService.Application.DTO.UserDto;

public class GetUserRequestDto
{
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.ReadUser;
}
