using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotnetApi.Enums;

namespace DotnetApi.Models.Financials
{
    /// <summary>
    /// The entity for tracking expenses and assets
    /// </summary>
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }

        // Transaction properties
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Recurring transaction properties
        public RecurrenceType? RecurrenceType { get; set; }
        public DateTime? RecurrenceStartDate { get; set; } = DateTime.UtcNow;
        public DateTime? RecurrenceEndDate { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
