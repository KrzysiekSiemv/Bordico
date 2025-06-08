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
    
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var query = httpContext?.Request?.Query;

            if (query != null && query.TryGetValue("conversationId", out var conversationId))
            {
                Console.WriteLine("Połączono do rozmowy: " + conversationId);
                Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            }

            return base.OnConnectedAsync();
        }


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

            Console.WriteLine("Wysłano!");

            // if (_userConnections.TryGetValue(receiverId, out var receiverConnId))
            // {
            //     await Clients.Group(receiverConnId).SendAsync("ReceiveMessage", senderId, conversation.id_conversation, senderNickname, message, DateTime.UtcNow);
            //     Console.WriteLine(receiverConnId);
            // }
            await Clients.Group($"conversation_{conversation.id_conversation}").SendAsync("ReceiveMessage", senderId, conversation.id_conversation, senderNickname, message, DateTime.UtcNow);
            Console.WriteLine(Context.ConnectionId);
        }

        public Task JoinConversation(string conversationId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var query = httpContext?.Request?.Query;

            if (query != null && query.TryGetValue("conversationId", out var conversationId))
            {
                Console.WriteLine("Połączono do rozmowy: " + conversationId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            }

            return base.OnDisconnectedAsync(exception);
        }

    }
}