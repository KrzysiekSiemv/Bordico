using System.Text.Json;
using Bordico.Client.Model;
using Bordico.Client.Service;
using Bordico.Client.ViewModel;

namespace Bordico.Client.View
{
    public partial class HomePage : ContentPage
    {
        private readonly RestService _api;
        private readonly INavigation _navigation;

        public HomePage(RestService api)
        {
            InitializeComponent();
            _api = api;

            BindingContext = new HomeViewModel(_api, this, Navigation);
            _navigation = Navigation;
        }
    }
}