using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotnetApi.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Models.Financials;

[Table("BankAccounts")]
[PrimaryKey("Id")]
public class BankAccount : ITimestampEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    
    [Required]
    [Column("Name")]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Required]
    [Column("AccountNumber")]
    [Range(0, double.MaxValue,  ErrorMessage = "Account number must be non-negative")]
    public int AccountNumber { get; set; }
    

    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}