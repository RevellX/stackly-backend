using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StacklyBackend.Models;

public class User : IdentityUser
{
    public List<Group> Groups { get; set; } = [];
}