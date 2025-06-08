using Bordico.Client.Service;

namespace Bordico
{
    public partial class NewConversationPage : ContentPage
    {
        private readonly RestService _api;
        public NewConversationPage(RestService api)
        {
            InitializeComponent();
            _api = api;
        }
    }
}
