using DotnetApi.Enums;

namespace DotnetApi.Models.Financials
{
    public class Transaction
    {
        // Primary key + foreign key to the User entity
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // Transaction properties
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Recurring transaction properties
        public RecurrenceType? RecurrenceType { get; set; }
        public DateTime? RecurrenceStartDate { get; set; } = DateTime.UtcNow;
        public DateTime? RecurrenceEndDate { get; set; }
    }
}
