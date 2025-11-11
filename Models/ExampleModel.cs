using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StacklyBackend.Models;

public class Example
{
    [Key]
    [Required]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [Range(0.0, 999999.9)]
    [Precision(8, 2)]
    public decimal Price { get; set; }

    public override string ToString()
    {
        return $"{Id}, {Name}, {Price}";
    }
}

public class ExampleCreate
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
}