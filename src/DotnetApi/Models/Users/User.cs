using DotnetApi.Enums.Users;

namespace DotnetApi.Models.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public string? DisplayName { get; set; } = null;
        public string? Email { get; set; }
        public int PresetImageIndex { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Premium status
        public bool? HasPremium { get; set; }
        public PremiumSource? PremiumSource { get; set; }

        // Notifications
        public string? NotificationToken { get; set; }  
        public int BadgeCount { get; set; }

        // Profile Completion Flags
        public bool? OnboardingCompleted { get; set; }
        public bool? ProfileCompleted { get; set; }

        // Timestamps
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
