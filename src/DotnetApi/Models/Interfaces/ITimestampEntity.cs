using System.ComponentModel.DataAnnotations;

namespace DotnetApi.Models.Interfaces;

/// <summary>
///     Adds standard createdAt and updatedAt timestamps to models
/// </summary>

public interface ITimestampEntity
{
    [Required]
    public DateTime CreatedAt { get; init; }
    [Required]
    public DateTime UpdatedAt { get; set; }
}