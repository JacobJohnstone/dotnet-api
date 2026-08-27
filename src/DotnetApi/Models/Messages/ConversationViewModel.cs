using DotnetApi.Enums.Messages;

namespace DotnetApi.Models.Messages
{
    public class ConversationViewModel
    {
        public Guid Id { get; set; }
        public Guid CoupleId { get; set; }

        // Blur and list styling statuses
        public bool User1HasContributed { get; private set; } = false;
        public bool User2HasContributed { get; private set; } = false;

        // Conversation identifiers
        public ActivityType ActivityType { get; set; }
        public required string CategoryId { get; set; }
        public required string ItemId { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
