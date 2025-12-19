using System.ComponentModel.DataAnnotations;

namespace StacklyBackend.Models;

public class Item
{
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public Category Category { get; set; } = null!;
}

public class ItemCreate
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string CategoryId { get; set; } = null!;
}

public class ItemQuery
{
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public string? Search { get; set; }
    public string? Category { get; set; }
}