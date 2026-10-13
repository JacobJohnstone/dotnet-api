using System.ComponentModel.DataAnnotations;

namespace DotnetApi.Models.Users;

public class LoginDto
{
    [Required]
    [EmailAddress]
    [StringLength(50,  MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
    public string email { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
    public string password { get; set; }
}