using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetApi.Models.Users
{
    /// <summary>
    /// The user view model for budget.jo users
    /// </summary>
    [Table("Users")]
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; } = null;
        [Required]
        [StringLength(50)]
        public string? LastName { get; set; } = null;
        [Required]
        [StringLength(50)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? ProfilePictureUrl { get; set; }

        // Notifications
        [StringLength(50)]
        public string? NotificationToken { get; set; }
        // Provided for future mobile home page badge value
        public int BadgeCount { get; set; } = 0;

        // Profile Completion Flags
        public bool? OnboardingCompleted { get; set; } = false;
        
        // TODO: if adding tutorial eventually, default to false.
        // initially defaulting to true to prevent experienced users from
        // receiving a tutorial when implemented later.
        public bool? TutorialCompleted { get; set; } = true;
        
        // Initialize null instead of 0 in order to tell the difference
        // between a provided value of 0 and no provided value at all
        public decimal? InitialAccountBalance { get; set; } = null;

        // Timestamps
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
