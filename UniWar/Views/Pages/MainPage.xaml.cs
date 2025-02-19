using Statistics;

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
           
            if (!UniWarSystem.Instance.IsOffline) {
                try{
                    var response = await ClientGrpc.NewGame(UniWarSystem.Instance.LoggedUsername!);
                    // TODO: GESTIRE CASO IN CUI STATUS è FALSE  
                    UniWarSystem.Instance.GameId = response.GameId;
                    
                }
                catch (Grpc.Core.RpcException ex) {
                    Console.WriteLine($"Errore: {ex}");
                    return;
                    
                }
                catch (Exception ex) {
                    Console.WriteLine($"Errore: {ex}");
                    return;
                }
            }

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