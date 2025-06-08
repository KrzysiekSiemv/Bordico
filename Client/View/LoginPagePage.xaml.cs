using Bordico.Client.Service;
using Bordico.Client.ViewModel;

namespace Bordico.Client.View
{
    public partial class LoginPagePage : ContentPage
    {
        private readonly INavigation _navigation;
        public LoginPagePage(RestService api)
        {
            InitializeComponent();
            _navigation = Navigation;
            BindingContext = new LoginViewModel(api, this, _navigation);
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            _navigation.PushAsync(new RegisterPage());
        }
    }
}
