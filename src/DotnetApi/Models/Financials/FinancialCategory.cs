using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using DotnetApi.Models.Interfaces;

namespace DotnetApi.Models.Financials;

[Table("FinancialCategories")]
public class FinancialCategory : ITimestampEntity
{
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public Color Color { get; set; }
    
    // icon
    
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    
}