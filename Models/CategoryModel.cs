using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StacklyBackend.Models;

public class Category
{
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
}

public class CategoryCreate
{
    [Required]
    public string Name { get; set; } = string.Empty;
}