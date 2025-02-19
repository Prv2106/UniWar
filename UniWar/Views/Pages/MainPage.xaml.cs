namespace UniWar {
    public partial class MainPage : ContentPage {
        public MainPage() {
            if (!UniWarSystem.Instance.IsOffline) 
                // perchè se è online l'utente può tornare indietro facendo il logout
                Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
            
            InitializeComponent();

            if (UniWarSystem.Instance.IsOffline) {
                LoggedPart.IsVisible = false;
                History.IsVisible = false;
            } else {
                username.Text = UniWarSystem.Instance.LoggedUsername;
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