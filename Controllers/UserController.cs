using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StacklyBackend.Models;
using StacklyBackend.Utils.DataGenerator;
using Utils;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private static AppDbContext _context = new AppDbContext();

    public UserController()
    {
        _context = new AppDbContext();
    }

    [HttpPost("create")]
    [Authorize]
    public ActionResult<IEnumerable<User>> CreateNew([FromBody] UserCreate userLogin)
    {
        bool usernameExists = _context.Users
            .Any(u => u.Username.ToLower() == userLogin.Username.ToLower());

        if (usernameExists)
            return Conflict(new { message = "Username already exists" });

        string id;
        do
        {
            id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
        } while (_context.Users.FirstOrDefault(p => p.Id.Equals(id)) is not null);

        string hashedPassword = Hasher.HashPassword(userLogin.Password);

        var newUser = new User
        {
            Id = id,
            Username = userLogin.Username,
            PasswordHashed = hashedPassword,
            DisplayName = userLogin.DisplayName
        };
        var user = _context.Users.Add(newUser);

        _context.SaveChanges();
        return CreatedAtAction(nameof(CreateNew), new { id = newUser.Id },
            new { Id = newUser.Id, Username = newUser.Username, DisplayName = newUser.DisplayName });
    }
}