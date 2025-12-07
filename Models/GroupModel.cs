using System.ComponentModel.DataAnnotations;

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
}

public class GroupCreate
{
    public string Name { get; set; } = string.Empty;

}

public class GroupEdit
{
    public string? Name { get; set; } = string.Empty;
    public string? AddUserEmail { get; set; } = string.Empty;
    public string? RemoveUserEmail { get; set; } = string.Empty;
}