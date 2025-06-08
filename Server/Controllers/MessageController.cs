using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Bordico.Server.Hubs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Bordico.Server.Controllers
{
    [ApiController, Route("Api/Messages"), Authorize]
    public class MessageController : Controller
    {
        private readonly BordicoContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(BordicoContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("Conversations")]
        public async Task<IActionResult> Conversations()
        {
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                var entities = await _context.conversations.Where(c => c.id_first_user == id || c.id_second_user == id).ToListAsync();
                if (entities == null)
                    return NotFound("Nie znaleziono żadnych konwersacji!");

                return Json(entities);
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }

        [HttpGet("Chat")]
        public async Task<IActionResult> Chat(int conversationId)
        {
            var messages = await _context.messages
                .Where(m => m.id_conversation == conversationId)
                .OrderBy(m => m.sent_at)
                .ToListAsync();

            return View(messages);
        }

        [HttpPost("GetConversation")]
        public async Task<IActionResult> GetConversation([FromForm] int userId)
        {
            
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                var conversation = await _context.conversations.Where(c => (c.id_first_user == id && c.id_second_user == userId) || (c.id_first_user == userId && c.id_second_user == id)).FirstOrDefaultAsync();
                if (conversation == null)
                {
                    var entity = new Conversation()
                    {
                        id_first_user = id,
                        id_second_user = userId
                    };

                    _context.conversations.Add(entity);
                    await _context.SaveChangesAsync();

                    if (ChatHub._userConnections.TryGetValue(userId, out var connection))
                    {
                        await _hubContext.Clients.Client(connection).SendAsync("NewConversation", new
                        {
                            userId = id,
                            userName = User.FindFirst(ClaimTypes.Name)?.Value,
                            conversationId = entity.id_conversation
                        });
                    }

                    return Ok(entity.id_conversation);
                }
                else
                    return Ok(conversation.id_conversation);
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }

        [HttpPost("GetMessages")]
        public async Task<IActionResult> GetMessages([FromForm] int conversation_id)
        {
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                var entities = await (from m in _context.messages
                                      join u in _context.users on m.id_sent_by equals u.id_user
                                      where m.id_conversation == conversation_id
                                      orderby m.sent_at ascending
                                      select new
                                      {
                                          senderId = m.id_sent_by,
                                          senderNickname = u.nickname,
                                          content = m.content,
                                          sentAt = m.sent_at
                                      }).ToListAsync();
                                      
                if (entities == null)
                    return NotFound("Nie znaleziono żadnych wiadomości!");

                return Json(entities);
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }

        [HttpPost("Send")]
        public async Task<IActionResult> SendMessage([FromForm] int conversationId, [FromForm] string content)
        {
            var id_user = User.FindFirst(ClaimTypes.NameIdentifier);

            if (id_user == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            if (int.TryParse(id_user.Value, out int id))
            {
                var entity = new Message
                {
                    id_conversation = conversationId,
                    id_sent_by = id,
                    content = content,
                    sent_at = DateTime.UtcNow
                };

                _context.messages.Add(entity);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
                return Unauthorized("ID użytkownika ma nieprawidłowy format");
        }
    }
}