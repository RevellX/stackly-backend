using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StacklyBackend.Models;

public class Category
{
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public Group Group { get; set; } = null!;
    
    [NotMapped]
    public int DisplayCount { get; set; }

    public static Category? GetCategoryById(AppDbContext _context, string categoryId, string userId)
    {
        return _context.Categories.FirstOrDefault(c =>
            c.Id == categoryId &&
            (c.Group.OwnerId == userId || c.Group.Users.Any(u => u.Id == userId)));
    }

    public static List<Category> GetCategoriesByGroupId(AppDbContext _context, string groupId, string userId)
    {
        return _context.Categories.Where(c =>
            c.GroupId == groupId &&
                (c.Group.OwnerId == userId ||
                c.Group.Users.Any(u => u.Id == userId))
            ).ToList();
    }

    public static bool IsCategoryInGroup(AppDbContext _context, string groupId, string name)
    {
        return _context.Categories.Any(c => c.Name == name && c.GroupId == groupId);
    }

    public static bool CategoryExistsById(AppDbContext _context, string categoryId)
    {
        return _context.Categories.Any(c => c.Id == categoryId);
    }

    public static bool UserCanAccessCategory(AppDbContext _context, string categoryId, string userId)
    {
        return _context.Categories.Any(c =>
            c.Id == categoryId &&
            (
                c.Group.OwnerId == userId ||
                c.Group.Users.Any(u => u.Id == userId)
            )
        );
    }
}

public class CategoryCreate
{
    public string Name { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
}

public class CategoryEdit
{
    public string? Name { get; set; } = string.Empty;
    public string? GroupId { get; set; } = string.Empty;
}