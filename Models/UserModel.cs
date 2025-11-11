using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StacklyBackend.Models;

public class User
{
    [Key]
    [Required]
    [StringLength(10)]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string PasswordHashed { get; set; } = string.Empty;
    [Required]
    [AllowNull]
    public string DisplayName { get; set; } = string.Empty;
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
    [AllowNull]
    public string DisplayName { get; set; } = string.Empty;
}