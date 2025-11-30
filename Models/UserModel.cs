using System.ComponentModel.DataAnnotations;

namespace StacklyBackend.Models;

public class User
{
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHashed { get; set; } = string.Empty;
    public string? DisplayName { get; set; } = string.Empty;
    public List<Group> Groups { get; set; } = [];
}

public class UserCredentials
{
    [MinLength(3)]
    [MaxLength(32)]
    public string Username { get; set; } = string.Empty;
    [MinLength(10)]
    public string Password { get; set; } = string.Empty;
}

public class UserCreate : UserCredentials
{
    public string? DisplayName { get; set; } = string.Empty;
}