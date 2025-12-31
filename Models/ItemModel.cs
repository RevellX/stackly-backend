using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
    public ICollection<ItemFile> Files { get; set; } = null!;
    public static IQueryable<Item> GetItemsByGroupId(AppDbContext _context, string groupId, string userId)
    {
        return _context.Items
            .Include(i => i.Category)
            .Include(i => i.Files)
            .Where(i =>
                i.Category.GroupId == groupId &&
                (i.Category.Group.OwnerId == userId ||
                i.Category.Group.Users.Any(u => u.Id == userId))
            );
    }

    public static Item? GetItemById(AppDbContext _context, string itemId, string userId)
    {
        return _context.Items.Include(i => i.Files).FirstOrDefault(i =>
            i.Id == itemId &&
            (i.Category.Group.OwnerId == userId || i.Category.Group.Users.Any(u => u.Id == userId)));
    }


}

public class ItemCreate
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string CategoryId { get; set; } = null!;
    public List<IFormFile>? Files { get; set; }
}

public class ItemEdit
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public string? CategoryId { get; set; }
    public List<IFormFile>? Files { get; set; }
}

public class ItemQuery
{
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public string? Search { get; set; }
    public string? Category { get; set; }
}