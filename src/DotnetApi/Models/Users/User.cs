namespace DotnetApi.Models.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Notifications
        public string? NotificationToken { get; set; }  
        public int BadgeCount { get; set; }

        // Profile Completion Flags
        public bool? OnboardingCompleted { get; set; }

        public decimal InitialAmount { get; set; } = 0;

        // Timestamps
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
