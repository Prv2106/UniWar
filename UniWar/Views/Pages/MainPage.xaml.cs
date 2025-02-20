using Statistics;

namespace UniWar {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();

            if (!UniWarSystem.Instance.IsOffline) {
                // utente online
                Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
                username.Text = UniWarSystem.Instance.LoggedUsername;
            } else { // utente offline
                LoggedPart.IsVisible = false;
                History.IsVisible = false;
            }
        }

        private async void OnNuovaPartitaButtonClicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new InitializationSummary()); 
        }

        private async void OnVisualizzaStoricoButtonClicked(object sender, EventArgs e) {
            await Navigation.PushModalAsync(new GamesHistory());
        }

        private async void OnLogoutClicked(object sender, EventArgs e) {
            UniWarSystem.Instance.OfflineMode();
            await Navigation.PopToRootAsync();
        }

    } 
}