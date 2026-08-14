using DotnetApi.Enums.Messages;

namespace DotnetApi.Models.Messages
{
    public class MessageViewModel
    {
        // Identifiers
        public required Guid Id { get; set; }
        public required Guid ConversationId { get; set; }
        public required Guid SenderId { get; set; }
        public required Guid ReceiverId { get; set; }

        public required string Content { get; set; }

        // Status state
        public Status Status { get; set; }
        public bool IsEdited { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
