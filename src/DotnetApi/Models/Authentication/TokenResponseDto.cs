namespace DotnetApi.Models.Users;

public class TokenResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshExpiresAt { get; set; }
}