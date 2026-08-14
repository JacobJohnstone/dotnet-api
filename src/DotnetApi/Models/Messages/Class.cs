namespace DotnetApi.Models.Messages
{
    public class ConversationViewModel
    {
        public Guid Id { get; set; }
        public Guid? User1Id { get; private set; }
        public Guid? User2Id { get; private set; }

        public bool User1HasContributed { get; private set; } = false;
        public bool User2HasContributed { get; private set; } = false;
    }
}
