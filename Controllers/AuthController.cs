using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StacklyBackend.Models;
using Utils;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static AppDbContext _context = new AppDbContext();
    private readonly IConfiguration Configuration;

    public AuthController(IConfiguration configuration)
    {
        _context = new AppDbContext();
        Configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserCredentials userLogin)
    {
        var dbUser = _context.Users.SingleOrDefault(
            p => p.Username == userLogin.Username);

        if (dbUser is not null &&
                Hasher.VerifyPassword(userLogin.Password, dbUser.PasswordHashed))
        {
            var token = GenerateJwtToken(userLogin.Username);
            return Ok(new { token });
        }

        return NotFound();
    }

    private string GenerateJwtToken(string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Configuration["Auth:SecretKey"]
            ?? throw new ArgumentNullException("Auth:SecretKey is null")
        ));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Configuration["Auth:TokenIssuer"] ?? throw new ArgumentNullException("Auth:TokenIssuer is null"),
            audience: Configuration["Auth:TokenAudience"] ?? throw new ArgumentNullException("Auth:TokenAudience is null"),
            claims: claims,
            expires: DateTime.Now.AddMinutes(Double.Parse(Configuration["Auth:TokenExpireMinutes"] ?? throw new ArgumentNullException("Auth:TokenExpireMinutes is null"))),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}