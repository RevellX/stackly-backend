using System.ComponentModel.DataAnnotations;

namespace StacklyBackend.Models;

public class Category
{
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public Group Group { get; set; } = null!;
}

public class CategoryCreate
{
    public string Name { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
}