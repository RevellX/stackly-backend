using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;

public class ItemFile
{
    public string Id { get; set; } = string.Empty;

    public string ItemId { get; set; } = string.Empty;
    public Item Item { get; set; } = null!;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public static ItemFile? GetFileById(AppDbContext _context, string fileId, string userId)
    {
        return _context.ItemFiles.Include(i => i.Item).Where(i =>
            i.Id == fileId &&
            (
                i.Item.Category.Group.OwnerId == userId ||
                i.Item.Category.Group.Users.Any(u => u.Id == userId)
            )
        ).FirstOrDefault();
    }
}