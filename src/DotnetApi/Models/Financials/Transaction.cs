using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotnetApi.Enums;
using DotnetApi.Models.Interfaces;

namespace DotnetApi.Models.Financials
{
    /// <summary>
    ///     The entity for tracking expenses and assets
    /// </summary>
    [Table("Transactions")]
    public class Transaction : ITimestampEntity
    {
        /// <summary>
        ///     Main guid identifier of a transaction
        /// </summary>
        [Key]
        public Guid Id { get; init; }
        
        /// <summary>
        ///     Main guid identifier for the related user
        /// </summary>
        [ForeignKey("UserId")]
        public Guid UserId { get; init; }

        /// <summary>
        ///     The name provided for the transaction
        /// </summary>
        // Transaction properties
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;


        /// <summary>
        ///     Sets a specific amount
        /// </summary>
        public decimal? Amount { get; set; }
        
        /// Below fields offer a range input choice (e.g., Gas: $230 - $300)
        
        /// <summary>
        ///     Handles the minimum value of a given range
        /// </summary>
        public decimal? MinAmount { get; set; }
        
        /// <summary>
        ///     Handles the maximum value of a given range
        /// </summary>
        public decimal? MaxAmount { get; set; }
        
        public decimal InterestRate { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Recurring transaction properties
        public RecurrenceType? RecurrenceType { get; set; }
        public DateTime? RecurrenceStartDate { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
    }
}
