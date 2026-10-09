using System.ComponentModel.DataAnnotations;

namespace DotnetApi.Models.Users;

public class CreateUserDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }
    
    [Required(AllowEmptyStrings =  false, ErrorMessage = "Last name is required.")] 
    public string LastName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")] 
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "{0} must be at least {2} characters long.")]
    public string Password { get; set; }
    
    public string? ProfilePictureUrl { get; set; } = null;

    public bool IsActive { get; set; } = true;
}