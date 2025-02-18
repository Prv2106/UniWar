﻿namespace UniWar {
    public partial class MainPage : ContentPage {
        public MainPage() {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
            InitializeComponent();

            if (UniWarSystem.Instance.IsOffline) {
                LoggedPart.IsVisible = false;
                History.IsVisible = false;
            }
        }

        private async void OnNuovaPartitaButtonClicked(object sender, EventArgs e) {
            // PushAsync è un metodo della proprietà Navigation che aggiunge una pagina allo stack e poi la raggiunge
            // N.B: await è usato perché PushAsync resituisce un Task, e quindi garantiamo la ripresa del controllo alla UI per 
            //      renderla responsiva.
             
           await Navigation.PushAsync(new InitializationSummary("mygiuseppe09"));    
        }

        private async void OnVisualizzaStoricoButtonClicked(object sender, EventArgs e) {
            await Navigation.PushModalAsync(new GamesHistory("mygiuseppe09"));
        }

        private async void OnLogoutClicked(object sender, EventArgs e) {
            UniWarSystem.Instance.OfflineMode();
            await Navigation.PopAsync();
        }

    } 
}