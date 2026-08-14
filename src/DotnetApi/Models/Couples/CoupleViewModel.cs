namespace DotnetApi.Models.Couples
{
    public class CoupleViewModel
    {
        // Couple Id
        public Guid Id { get; set; }

        // Involved User Ids (nullable)
        public Guid? User1Id { get; set; }
        public Guid? User2Id { get; set; }

        // Couple Info
        public DateTime Anniversary { get; set; }
        public Streak Streak { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
