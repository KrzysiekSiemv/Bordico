using System.Net;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers;

[Route("Api/Auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly BordicoContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ILogger<AuthController> logger, BordicoContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] string username, [FromForm] string email_address, [FromForm] string password, [FromForm] string description, [FromForm] string nickname)
    {
        if (await _context.users.AnyAsync(u => u.username == username || u.email_address == email_address))
            return BadRequest("Użytkownik o takim adresie e-mail lub użytkowniku istnieje!");

        string hashPassword = HashMePassword(password);

        var entity = new User()
        {
            username = username,
            email_address = email_address,
            password = hashPassword,
            description = description,
            nickname = nickname,
        };

        _context.users.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Rejestracja zakończona sukcesem!" });
    }

    [HttpGet("Check")]
    public IActionResult Check() {
        return Ok(new { message = "Działam! Jest połączenie!" });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] LoginModel login)
    {
        string hashed = HashMePassword(login.Password);
        var user = await _context.users.FirstOrDefaultAsync(u => u.username == login.Username && u.password == hashed);

        if (user == null)
            return Unauthorized("Nieprawidłowe dane logowania");

        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"] ?? "chuj");

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.id_user.ToString()),
                new Claim(ClaimTypes.Name, user.nickname ?? "")
            ]),
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new { token = tokenHandler.WriteToken(token) });
    }

    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromForm] string password) {
        var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

        if (id_user == null)
            return Unauthorized("Brak ID użytkownika w tokenie");

        if (int.TryParse(id_user.Value, out int id))
        {
            var entity = await _context.users.FindAsync(id);
            if (entity == null)
                return NotFound("Wystąpił błąd! Zaloguj się ponownie do aplikacji");

            entity.password = HashMePassword(password);

            _context.users.Update(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pomyślnie zmieniono hasło!" });
        }
        else
            return Unauthorized("ID użytkownika ma nieprawidłowy format");   
    }

    private string HashMePassword(string password)
    {
        byte[] salt = Encoding.UTF8.GetBytes("092384djskh12890dfsnk12zx");

        return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 1000, 256 / 8));
    }
}

public class LoginModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}