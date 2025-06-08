using System.Text.Json;
using System.Net.Http.Json;
using Bordico.Client.Model;
using System.Net.Http.Headers;

namespace Bordico.Client.Service;

public class RestService
{
    readonly JsonSerializerOptions _options;

    public RestService()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    private HttpClient _client(bool authorize = false)
    {
        var client = new HttpClient();
        if (authorize)
        {
            string token = Preferences.Get("token", "");

            if (string.IsNullOrEmpty(token))
                throw new Exception("Brak tokena uwierzytelniającego. Zaloguj się jeszcze raz!");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<string?> Login(string username, string password)
    {
        Uri uri = new($"{Constants.Server}/Api/Auth/Login");

        FormUrlEncodedContent data = new([
            new KeyValuePair<string, string>("Username", username),
            new KeyValuePair<string, string>("Password", password)
        ]);

        var response = await _client().PostAsync(uri, data);

        if (!response.IsSuccessStatusCode)
            return null;

        string json = await response.Content.ReadAsStringAsync();
        var doc = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        return doc != null ? doc["token"] : "no_token";
    }

    public async Task<Boolean> Register(User user)
    {
        Uri uri = new($"{Constants.Server}/Api/Auth/Register");

        FormUrlEncodedContent data = new([
            new KeyValuePair<string, string>("username", user.username),
            new KeyValuePair<string, string>("password", user.password),
            new KeyValuePair<string, string>("email_address", user.email_address),
            new KeyValuePair<string, string>("description", user.description ?? ""),
            new KeyValuePair<string, string>("nickname", user.nickname ?? "")
        ]);

        var response = await _client().PostAsync(uri, data);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetUserInfo(int id_user = -1)
    {
        var response = new HttpResponseMessage();
        using var client = _client(true);
        if (id_user == -1)
        {
            Uri uri = new($"{Constants.Server}/Api/User/Data");

            response = await client.GetAsync(uri);
        }
        else
        {
            Uri uri = new($"{Constants.Server}/Api/User/GetOther");
            FormUrlEncodedContent data = new([
                new KeyValuePair<string, string>("id_user", id_user.ToString())
            ]);

            response = await client.PostAsync(uri, data);
        }
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("Błąd pobierania danych użytkownika");

        string json = await response.Content.ReadAsStringAsync();
        return json;
    }

    public async Task<string?> GetConversations()
    {
        Uri uri = new($"{Constants.Server}/Api/Messages/Conversations");
        using var client = _client(true);

        var response = await client.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Błąd przy pobieraniu konwersacji");

        string json = await response.Content.ReadAsStringAsync();
        return json;
    }

    public async Task<string?> GetMessages(int conversation_id)
    {
        Uri uri = new($"{Constants.Server}/Api/Messages/GetMessages");
        FormUrlEncodedContent data = new([
            new KeyValuePair<string, string>("conversation_id", conversation_id.ToString())
        ]);

        using var client = _client(true);
        var response = await client.PostAsync(uri, data);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Nie można pobrać wiadomości z tej konwersacji!");

        string json = await response.Content.ReadAsStringAsync();
        return json;
    }

    public async Task<string?> GetConversation(int conversation_id)
    {
        Uri uri = new($"{Constants.Server}/Api/Messages/GetMessages");
        FormUrlEncodedContent data = new([
            new KeyValuePair<string, string>("conversation_id", conversation_id.ToString())
        ]);

        using var client = _client(true);
        var response = await client.PostAsync(uri, data);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Nie można pobrać wiadomości z tej konwersacji!");

        string json = await response.Content.ReadAsStringAsync();
        return json;
    }
}