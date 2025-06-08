namespace Bordico.Client.View
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("token", null);
            Preferences.Set("user", null);

            
        }
    }
}
