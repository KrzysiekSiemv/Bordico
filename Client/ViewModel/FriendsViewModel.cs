using System.Collections.ObjectModel;
using Bordico.Client.Model;
using Bordico.Client.Service;

namespace Bordico.Client.ViewModel;

public class FriendsViewModel
{
    private readonly RestService _api;
    private readonly Page _page;
    private readonly INavigation _navigation;

    public ObservableCollection<User> Friends { get; set; } = [];
    public ObservableCollection<User> AllUsers { get; set; } = [];

    public FriendsViewModel(RestService api, Page page, INavigation navigation)
    {
        _api = api;
        _page = page;
        _navigation = navigation;
    }

    

}