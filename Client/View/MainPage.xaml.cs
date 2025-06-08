using System.Text.Json;
using Bordico.Client.Model;
using Bordico.Client.Service;
using Bordico.Client.ViewModel;

namespace Bordico.Client.View;

public partial class MainPage : TabbedPage
{
	public MainPage(RestService api)
	{
		InitializeComponent();

		Children.Add(new HomePage(api)
		{
			Title = "Strona główna"
		});
		Children.Add(new FriendsPage()
		{
			Title = "Znajomi"
		});
		Children.Add(new SettingsPage()
		{
			Title = "Ustawienia"
		});
	}
}
