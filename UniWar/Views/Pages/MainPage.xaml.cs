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
            // PushAsync è un metodo della proprietà Navigation che aggiunge una pagina allo stack e poi la raggiunge
            // N.B: await è usato perché PushAsync resituisce un Task, e quindi garantiamo la ripresa del controllo alla UI per 
            //      renderla responsiva.
             
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