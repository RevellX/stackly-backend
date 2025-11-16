using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StacklyBackend.Models;

public class Category
{
    [Key]
    public string Name { get; set; } = string.Empty;
    [AllowNull]
    public string DisplayName { get; set; } = string.Empty;
}