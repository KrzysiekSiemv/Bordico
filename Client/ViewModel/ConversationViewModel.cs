using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Claims;
using System.Text.Json;
using System.Windows.Input;
using Bordico.Client.Model;
using Bordico.Client.Service;
using Microsoft.AspNetCore.SignalR.Client;

namespace Bordico.Client.ViewModel;

public class ConversationViewModel : INotifyPropertyChanged
{
    private readonly RestService _api;
    private Conversations _conversation;
    private readonly Page _page;
    private readonly HubConnection _conn;
    private readonly User _user;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? ScrollToEndRequested;


    public Conversations Conversation
    {
        get => _conversation;
        set
        {
            _conversation = value;
            OnPropertyChanged(nameof(Conversation));
        }
    }

    public ObservableCollection<Message> Messages { get; set; } = [];
    public Message NewMessage { get; set; } = new();
    public ICommand SendMessageCmd { get; }

    public ConversationViewModel(RestService api, Conversations conversation, Page page, INavigation navigation)
    {
        _api = api;
        Conversation = conversation;
        _page = page;

        string url = $"{Constants.Server}/Chat?conversationId={Conversation.Id}";
        _conn = new HubConnectionBuilder().WithUrl(url).Build();

        var user_json = Preferences.Get("user", "");
        _user = JsonSerializer.Deserialize<User>(user_json) ?? new();

        Task.Run(GetMessages);

        SendMessageCmd = new Command(async () => await SendMessage());
        _conn.On<int, int, string, string, DateTime>("ReceiveMessage", (senderId, conversationId, senderNickname, message, sentAt) =>
        {
            if (conversationId == Conversation.Id)
            {
                Messages.Add(new Message()
                {
                    Id = senderId,
                    Author = senderNickname,
                    Content = message,
                    SentAt = sentAt.ToString(),
                    IsMine = senderId.ToString() == ClaimTypes.NameIdentifier
                });

                OnPropertyChanged(nameof(Messages));
                ScrollToEndRequested?.Invoke();
            }
        });

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await _conn.StartAsync();
                await _conn.InvokeAsync("JoinConversation", Conversation.Id.ToString());
            }
            catch (Exception ex)
            {
                await _page.DisplayAlert("Błąd", $"Nie można połączyć z serwerem: {ex.Message}", "OK");
            }
        });
    }

    public async Task GetMessages()
    {
        string? token = Preferences.Get("token", "");
        if (token != "")
        {
            string? json = await _api.GetMessages(Conversation.Id);
            if (json == null) return;

            var messages = JsonSerializer.Deserialize<List<TheMessage>>(json);
            if (messages == null) return;

            foreach (var message in messages)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Messages.Add(new Message()
                    {
                        Id = message.senderId,
                        Author = message.senderNickname,
                        Content = message.content,
                        SentAt = message.sentAt.ToString(),
                        IsMine = message.senderId != _user.id_user
                    });

                    OnPropertyChanged(nameof(Messages));
                    ScrollToEndRequested?.Invoke();
                });
            }
        }
    }

    private async void TestConnection() {
        try {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(Constants.Server + "/Api/Auth/Check");

            if (response.IsSuccessStatusCode)
            {
                await _page.DisplayAlert("Połączenie z serwerem", "Połączenie z serwerem zostało pomyślnie nawiązane.", "OK");
            }
            else
            {
                await _page.DisplayAlert("Błąd", $"Nie udało się połączyć z serwerem: {response.StatusCode}", "OK");
            }
        } catch (Exception ex) {
            await _page.DisplayAlert("Błąd", $"Wystąpił błąd połączenia: {ex.Message}", "OK");
        }
    }

    private async Task SendMessage()
    {
        try
        {
            await _conn.InvokeCoreAsync("SendMessage", args: [
                _conversation.FriendId,
                _user.id_user,
                _user.nickname,
                NewMessage.Content
            ]);

            NewMessage.Content = "";
            OnPropertyChanged(nameof(NewMessage));

            await Task.Delay(100);
            ScrollToEndRequested?.Invoke();
        }
        catch (Exception ex)
        {
            await _page.DisplayAlert("Błąd", $"Nie udało się wysłać wiadomości: {ex.Message}", "OK");
        }
    }

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

public class TheMessage
{
    public int senderId { get; set; }
    public string senderNickname { get; set; } = "";
    public string content { get; set; } = "";
    public DateTime sentAt { get; set; }
}