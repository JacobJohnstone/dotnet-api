namespace DotnetApi.Models.Users;

/// <summary>
///     Dto model for a standard user
///     Implemented/populated through the primary constructor
/// </summary>
/// <param name="user">Associated user for dto model</param>
public class UserDto(User user)
{
    public Guid Id { get; set; } = user.Id;
    public string FirstName { get; set; } = user.FirstName;
    public string LastName { get; set; } = user.LastName;
    public string Email { get; set; } = user.Email;
    public string? ProfilePictureUrl { get; set; } = user.ProfilePictureUrl;
    public bool IsActive { get; set; } = user.IsActive;
}