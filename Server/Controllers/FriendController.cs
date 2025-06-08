using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Database;

namespace Bordico.Server.Controllers
{
    [ApiController, Route("Api/Friends"), Authorize]
    public class FriendsController : Controller
    {
        private readonly BordicoContext _context;

        public FriendsController(BordicoContext context)
        {
            _context = context;
        }
        
        [HttpGet("List")]
        public async Task<IActionResult> GetFriends()
        {
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                List<Friend>? friends = await _context.friends.Where(f => f.id_first_user == id || f.id_second_user == id).ToListAsync();

                if (friends == null)
                    return NotFound("Użytkownik nie ma przjaciół");

                return Ok(friends);
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddFriend([FromForm] int id_friend)
        {
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                var entity = new Friend()
                {
                    id_first_user = id,
                    id_second_user = id_friend,
                    accepted = false
                };

                _context.friends.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Wysłano zaproszenie do znajomych " });
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }

        [HttpPost("Accept")]
        public async Task<IActionResult> AcceptFriend([FromForm] int id_friend)
        {
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                var entity = await _context.friends.Where(f => f.id_first_user == id_friend && f.id_second_user == id && f.accepted == false).FirstOrDefaultAsync();
                if (entity == null)
                    return NotFound("Nie znaleziono relacji miedzy wami");

                entity.accepted = true;

                _context.friends.Update(entity);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Zaakceptowano zaproszenie do grona znajomych" });
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }

        [HttpPost("Remove")]
        public async Task<IActionResult> DeleteFriend([FromForm] int id_friend)
        {
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                var entity = await _context.friends.Where(f => (f.id_first_user == id_friend && f.id_second_user == id) || (f.id_first_user == id && f.id_second_user == id_friend)).FirstOrDefaultAsync();
                if (entity == null)
                    return NotFound("Nie znaleziono relacji między wami");

                _context.friends.Remove(entity);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Usunięto z listy znajomych" });
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }
    }
}
