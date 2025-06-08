using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Windows.Input;
using Bordico.Client.Model;
using Bordico.Client.Service;
using Microsoft.AspNetCore.SignalR.Client;

namespace Bordico.Client.ViewModel;

public class HomeViewModel
{
    // public ObservableCollection<Message> Chat { get; set; } = [];
    // private readonly HubConnection _conn;
    // public Message NewMessage {get; set;} = new();

    // public ICommand SendMessageCmd { get; }

    // private readonly Page _page;
    // string baseURL = "http://172.16.0.23";
    // public HomeViewModel(Page page){
    //     _page = page;
    //     _conn = new HubConnectionBuilder().WithUrl($"{baseURL}:5274/chathub").Build();

    //     TestConnection();

    //     _conn.On<string, string>("ReceiveMessage", (user, message) => {
    //         MainThread.BeginInvokeOnMainThread(() => {
    //             Chat.Add(new() {
    //                 Content = message,
    //                 Author = user,
    //                 IsMine = user == NewMessage.Author
    //             });
    //         });
    //     });

    //     MainThread.BeginInvokeOnMainThread(async() => {
    //         try {
    //             await _conn.StartAsync();
    //         } catch(Exception ex){
    //             await _page.DisplayAlert("Błąd", $"Nie można połączyć z serwerem: {ex.Message}", "OK");
    //         }
    //     });

    //     SendMessageCmd = new Command(async() => await SendMessage());
    // }

    // private async void TestConnection() {
    //     try {
    //         using (HttpClient client = new HttpClient()) {
    //             HttpResponseMessage response = await client.GetAsync(baseURL);

    //             if (response.IsSuccessStatusCode) {
    //                 await _page.DisplayAlert("Połączenie z serwerem", "Połączenie z serwerem zostało pomyślnie nawiązane.", "OK");
    //             } else {
    //                 await _page.DisplayAlert("Błąd", $"Nie udało się połączyć z serwerem: {response.StatusCode}", "OK");
    //             }
    //         }
    //     } catch (Exception ex) {
    //         await _page.DisplayAlert("Błąd", $"Wystąpił błąd połączenia: {ex.Message}", "OK");
    //     }
    // }

    // public async Task SendMessage(){
    //     try{
    //         await _conn.InvokeCoreAsync("SendMessage", args: [
    //             NewMessage.Author,
    //             NewMessage.Content
    //         ]);

    //         NewMessage.Content = "";
    //     } catch(Exception ex){
    //         await _page.DisplayAlert("Błąd", $"Nie udało się wysłać wiadomości: {ex.Message}", "OK");
    //     }
    // }
    private readonly RestService _api;
    private readonly Page _page;
    private readonly INavigation _navigation;

    public ObservableCollection<Conversations> ConversationList { get; set; } = [];
    public ICommand NewMessageCommand { get; }
    public ICommand EnterConversationCmd { get; }
    public string WelcomeLbl { get; set; } = "";

    public HomeViewModel(RestService api, Page page, INavigation navigation)
    {
        _api = api;
        _page = page;
        _navigation = navigation;

        NewMessageCommand = new Command(async () => await NewMessage());
        EnterConversationCmd = new Command<Conversations>(async (e) => await EnterConversation(e));

        Task.Run(LoadConversations);
        LoadData();
    }

    public async Task NewMessage()
    {
        await _navigation.PushAsync(new NewConversationPage(_api));
    }

    public async Task LoadConversations()
    {
        string? token = Preferences.Get("token", "");
        if (token != "")
        {
            string? json = await _api.GetConversations();
            if (json == null)
                return;

            var convos = JsonSerializer.Deserialize<List<Conversation>>(json);
            if (convos == null)
                return;

            foreach (var convo in convos)
            {
                int conversation_id = convo.id_conversation;
                if (Preferences.Get("user", "") != "")
                {
                    var user = JsonSerializer.Deserialize<User>(Preferences.Get("user", ""));
                    if (user == null) return;

                    int id_friend = convo.id_first_user == user.id_user ? convo.id_second_user : convo.id_first_user;

                    string? json_other = await _api.GetUserInfo(id_friend);
                    if (json_other == null) return;

                    var friend = JsonSerializer.Deserialize<User>(json_other);
                    if (friend == null) return;

                    ConversationList.Add(new Conversations() {
                        Id = conversation_id,
                        FriendName = friend.username,
                        FriendId = id_friend,
                        Description = friend.description ?? "<Brak opisu>"
                    });
                }
            }
        }
    }

    public void LoadData()
    {
        User user = JsonSerializer.Deserialize<User>(Preferences.Get("user", "{}")) ?? new User();
        WelcomeLbl = $"Witaj, {user.nickname}";
    }

    public async Task EnterConversation(Conversations conversation)
    {
        if (conversation != null)
            await _navigation.PushAsync(new ConversationPage(_api, conversation));
    }
}

public class Conversations {
    public int Id { get; set; }
    public string FriendName { get; set; } = "";
    public int FriendId { get; set; }
    public string Description { get; set; } = "";
}