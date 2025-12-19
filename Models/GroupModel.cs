using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StacklyBackend.Models;

public class Group
{
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public string OwnerId { get; set; } = string.Empty;
    public User Owner { get; set; } = null!;

    public static Group? GetGroupById(AppDbContext _context, string groupId, string userId)
    {
        return _context.Groups.Include(g => g.Users).FirstOrDefault(g =>
            g.Id == groupId &&
            (g.OwnerId == userId || g.Users.Any(u => u.Id == userId))
        );
    }

    public static List<Group> GetGroupsForUser(AppDbContext _context, string userId)
    {
        return _context.Groups.Include(g => g.Users).Where(g =>
            g.OwnerId == userId ||
            g.Users.Any(u => u.Id == userId))
        .ToList();
    }

    public static bool IsUserGroupOwner(AppDbContext _context, string groupId, string userId)
    {
        return _context.Groups.Any(g =>
            g.Id == groupId &&
            g.OwnerId == userId);
    }

    public static bool IsUserGroupMember(AppDbContext _context, string groupId, string userId)
    {
        return _context.Groups.Any(g =>
                    g.Id == groupId &&
                    (g.OwnerId == userId ||
                    g.Users.Any(u => u.Id == userId)));
    }

    public static bool GroupExistsById(AppDbContext _context, string groupId)
    {
        return _context.Groups.Any(g => g.Id == groupId);
    }
}

public class GroupCreate
{
    public string Name { get; set; } = string.Empty;

}

public class GroupEdit
{
    public string? Name { get; set; } = string.Empty;
    public string? GroupId { get; set; } = string.Empty;
    public string? AddUserEmail { get; set; } = string.Empty;
    public string? RemoveUserEmail { get; set; } = string.Empty;
}