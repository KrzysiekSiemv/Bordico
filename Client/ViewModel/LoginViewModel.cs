using System.Windows.Input;
using Bordico.Client.Service;
using Bordico.Client.View;

namespace Bordico.Client.ViewModel;

public class LoginViewModel
{
    private readonly RestService _api;
    private readonly Page _page;
    private readonly INavigation _navigation;

    public ICommand LoginCommand { get; }

    public string Username { get; set; } = "";
    public string Password { get; set; } = "";


    public LoginViewModel(RestService api, Page page, INavigation navigation)
    {
        _api = api;
        _page = page;
        _navigation = navigation;
        LoginCommand = new Command(async () => await Login());
    }

    public async Task Login()
    {
        string? token = await _api.Login(Username, Password);
        if (token != null)
        {
            Preferences.Set("token", token);

            string? userData = await _api.GetUserInfo();

            Preferences.Set("user", userData);
            Display(token);

            await _navigation.PushAsync(new MainPage(_api));
        }
        else
        {
            Display("Error!");
        }
    }

    private void Display(string message)
    {
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            _page.DisplayAlert("Komunikat", message, "OK");
        });
    }
}