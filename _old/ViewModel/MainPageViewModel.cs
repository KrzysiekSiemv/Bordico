using Server.Database;

namespace Bordico.Server.ViewModel;

public class MainPageViewModel
{
    public int LoggedUserId { get; set; }
    public User UserData { get; set; }
    public List<Conversations> ConversationList { get; set; } = [];
}

public class Conversations
{
    public int ConversationId { get; set; }
    public int UserId { get; set; }     // Tutaj jest podawane ID z kim jest prowadzona konwersacja
    public string UserName { get; set; } = "";
    public List<Message> Messages { get; set; } = [];
}