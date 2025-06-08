using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Database;

namespace Server.Controllers;

[ApiController, Route("Api/User"), Authorize]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly BordicoContext _context;

    public UserController(ILogger<UserController> logger, BordicoContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("Data")]
    public async Task<IActionResult> GetUserInfo()
    {
        var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

        if (id_user == null)
            return Unauthorized("Brak ID użytkownika w tokenie");

        if (!int.TryParse(id_user.Value, out int id))
            return Unauthorized("ID użytkownika ma nieprawidłowy format");

        User? user = await _context.users.FindAsync(int.Parse(id_user.Value));

        if (user == null)
            return NotFound("Użytkownik nie istnieje!");

        return Ok(new
        {
            user.id_user,
            user.nickname,
            user.username,
            user.email_address,
            user.description
        });
    }

    [HttpPost("GetOther")]
    public async Task<IActionResult> GetOtherUserInfo([FromForm] int id_user)
    {
        // First: Identification!
        var user = User.FindFirst(ClaimTypes.NameIdentifier);

        if (user == null)
            return Unauthorized("Brak ID użytkownika w tokenie");

        if (int.TryParse(user.Value, out int id))
        {
            var entity = await _context.users.FindAsync(id_user);
            if (entity == null)
                return NotFound("Nie znaleziono użytkownika o takim ID");

            return Ok(new
            {
                entity.id_user,
                entity.nickname,
                entity.username,
                entity.email_address,
                entity.description
            });
        }
        else
            return Unauthorized("ID użytkownika ma nieprawidłowy format");
    }

    [HttpPost("Update")]
    public async Task<IActionResult> UpdateUserInfo([FromForm] string description, [FromForm] string email_address, [FromForm] string nickname, [FromForm] bool allow_messages)
    {
        var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

        if (id_user == null)
            return Unauthorized("Brak ID użytkownika w tokenie");

        if (int.TryParse(id_user.Value, out int id))
        {
            var entity = await _context.users.FindAsync(id);
            if (entity == null)
                return NotFound("Wystąpił błąd! Zaloguj się jeszcze raz do aplikacji");

            entity.allow_messages = allow_messages;
            entity.description = description;
            entity.email_address = email_address;
            entity.nickname = nickname;
            entity.updated_at = DateTime.UtcNow;

            _context.users.Update(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Zaktualizowano dane użytkownika pomyślnie!" });
        }
        else
            return Unauthorized("ID użytkownika ma nieprawidłowy format");
    }
}