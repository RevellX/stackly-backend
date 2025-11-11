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
    // This is confusing. You need both these fields
    // for relation to be tracked properly
    [AllowNull]
    public Category? Category { get; set; }
    public string? CategoryName { get; set; } // ✅ Explicit FK Category.Name
}

public class ItemCreate
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [AllowNull]
    public string Description { get; set; } = string.Empty;
    [Required]
    public int Quantity { get; set; } = 1;
    [AllowNull]
    public string? CategoryName { get; set; }
}

public class ItemEdit
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public string? CategoryName { get; set; }

}

public class ItemAdd
{
    [Required]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    [Required]
    public int Quantity { get; set; }
}