using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StacklyBackend.Models;

public class Item
{
    [Key]
    [Required]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    [AllowNull]
    public string Description { get; set; } = string.Empty;
    [Required]
    public int Quantity { get; set; }
}

public class ItemCreate
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [AllowNull]
    public string Description { get; set; } = string.Empty;
    [Required]
    public int Quantity { get; set; } = 1;
}