using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotnetApi.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Models.Users
{
    /// <summary>
    /// The user view model for budget.jo users
    /// </summary>
    [Table("Users")]
    [PrimaryKey("Id")]
    public class User : ITimestampEntity
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        // String composite formatting sets the following within the error message for string length attributes:
        // 0 - the field name
        // 1 - the maximum value
        // 2 - the minimum value
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string FirstName { get; set; }
        
        [StringLength(50,  ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string LastName { get; set; }
        
        [StringLength(50,  ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        
        [StringLength(50, ErrorMessage =  "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        [StringLength(50)]
        public string? ProfilePictureUrl { get; set; }

        // Notifications
        [StringLength(50)] 
        public string? NotificationToken { get; set; } = null;
        // Provided for future mobile home page badge value
        public int BadgeCount { get; set; } = 0;

        // Profile Completion Flags
        public bool? OnboardingCompleted { get; set; } = false;
        
        // TODO: if/when adding tutorial eventually, default to false.
        // initially defaulting to true to prevent experienced users from
        // receiving a tutorial when implemented later.
        public bool? TutorialCompleted { get; set; } = true;
        
        // Initialize null instead of 0 in order to tell the difference
        // between a provided value of 0 and no provided value at all
        public decimal? BaseAccountBalance { get; set; } = null;
        
        public bool IsActive { get; set; } = true;
        
        // Timestamps
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
