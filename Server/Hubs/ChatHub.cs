using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Server.Database;

namespace Bordico.Server.Hubs {
    public class ChatHub : Hub
    {
        private readonly BordicoContext _context;
        public static Dictionary<int, string> _userConnections = new();

        public ChatHub(BordicoContext context)
        {
            _context = context;
        }

    
        // public override Task OnConnectedAsync()
        // {
        //     var httpContext = Context.GetHttpContext();
        //     if (httpContext == null) return base.OnConnectedAsync();

        //     var id_user = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        //     if (id_user != null && int.TryParse(id_user.Value, out int id))
        //     {
        //         _userConnections[id] = Context.ConnectionId;
        //     }

        //     return base.OnConnectedAsync();
        // }


        public async Task SendMessage(int receiverId, int senderId, string senderNickname, string message)
        {

            var conversation = await _context.conversations
                .Where(c => (c.id_first_user == senderId && c.id_second_user == receiverId) ||
                            (c.id_first_user == receiverId && c.id_second_user == senderId))
                .Select(c => new Conversation { id_conversation = c.id_conversation })
                .FirstOrDefaultAsync();

            if (conversation == null) return;

            _context.messages.Add(new Message
            {
                id_conversation = conversation.id_conversation,
                id_sent_by = senderId,
                content = message,
                sent_at = DateTime.UtcNow,
                edited = false,
                delivered = false,
                is_read = false
            });
            await _context.SaveChangesAsync();

            if (_userConnections.TryGetValue(receiverId, out var receiverConnId))
                await Clients.Client(receiverConnId).SendAsync("ReceiveMessage", senderId, conversation.id_conversation, senderNickname, message, DateTime.UtcNow);

            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", senderId, conversation.id_conversation, senderNickname, message, DateTime.UtcNow);
        }

        // public override Task OnDisconnectedAsync(Exception? exception)
        // {
        //     var httpContext = Context.GetHttpContext();
        //     if (httpContext == null) return base.OnDisconnectedAsync(exception);

        //     var id_user = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        //     if (id_user != null && int.TryParse(id_user.Value, out int id))
        //     {
        //         _userConnections.Remove(id);
        //     }

        //     return base.OnDisconnectedAsync(exception);
        // }

    }
}