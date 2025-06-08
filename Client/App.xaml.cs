using Bordico.Client.Service;
using Bordico.Client.View;

namespace Bordico.Client;

public partial class App : Application
{
	private readonly RestService _api;
	public App(RestService api)
	{
		InitializeComponent();
		_api = api;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		if (Preferences.Get("token", null) != null)
			return new Window(new NavigationPage(new MainPage(_api)));
		else
			return new Window(new NavigationPage(new LoginPagePage(_api)));
	}
}