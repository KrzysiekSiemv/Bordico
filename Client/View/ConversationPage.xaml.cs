using Bordico.Client.Service;
using Bordico.Client.ViewModel;

namespace Bordico
{
    public partial class ConversationPage : ContentPage
    {
        public ConversationPage(RestService api, Conversations conversation)
        {
            InitializeComponent();
            var vm = new ConversationViewModel(api, conversation, this, Navigation);
            BindingContext = vm;

            vm.ScrollToEndRequested += () =>
            {
                if (vm.Messages.Count > 0)
                    MessagesView.ScrollTo(vm.Messages.Last(), position: ScrollToPosition.End);
            };
        }
    }
}
