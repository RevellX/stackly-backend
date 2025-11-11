using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace stackly.Models;

public class Category
{
    [Key]
    public string Name { get; set; } = string.Empty;
    [AllowNull]
    public string DisplayName { get; set; } = string.Empty;
}

public class CategoryEdit
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;
}