using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StacklyBackend.Models;

public class User : IdentityUser
{
    // List of groups a user is assigned to
    public List<Group> Groups { get; set; } = [];
}