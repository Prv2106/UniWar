using Statistics;

namespace UniWar {
    public partial class GamesHistory : ContentPage {
        /*
            Pagina dove viene mostrato lo storico delle ultime 10 partite 
            (completate o non) dll'utente loggato
        */

        
        public class Game { 
            public int Id {get;}
            public string Date {get; set;}
            public string State {get; set;}

            public Game(int id, string date, string state) {
                Id = id;
                Date = date;
                State = state;
            }
        }   
        

        public List<Game> Games { get;} = []; // proprità necessario per il Binding allo xaml

        
        public GamesHistory() {
            InitializeComponent();
             
        }

        private void ShowLoadingAnimation() {
            loading.IsVisible = true;
            page.IsVisible = false;
            warning.IsVisible = false; 
        }

        private void HideLoadingAnimation(string? message = null) {
            // da invocare alla fine del blocco try o del catch
            if (message is not null) {
                // ci sono problemi
                page.IsVisible = false;
                loading.IsVisible = false;
                warning.Text = message;
                warning.IsVisible = true; 
            } else {
                // tutto ok
                loading.IsVisible = false;
                page.IsVisible = true;
                warning.IsVisible = false; 
            }
        }

        protected async override void OnAppearing() {
            base.OnAppearing();
            
            // Interfacciamoci col client grpc
            // per recuperare l'elenco delle partite per l'utente loggato
            ShowLoadingAnimation();
            try {
                GameInfoList response = await ClientGrpc.GetGames(UniWarSystem.Instance.LoggedUsername!);
                Console.WriteLine("Ho ricevuto la risposta");
                if (response.Games.Count == 0 && response.Status == true) {
                    // l'utente non ha ancora nessuna partita disputata nel database
                    HideLoadingAnimation("Non sono presenti partite da te giocate nel database!");
                } else if (response.Status == false) {
                    HideLoadingAnimation(response.Message);
                } else { // abbiamo delle partite!
                    // Controlla se il server ha inviato il grafico in base64
                    if (!string.IsNullOrEmpty(response.GameResultsPieChart)) {
                        byte[] imageBytes = Convert.FromBase64String(response.GameResultsPieChart);
                        // Create a new MemoryStream that will stay in scope
                        var stream = new MemoryStream(imageBytes);
                        Chart.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                        Chart.WidthRequest = 240;
                    }

                    string state;
                    foreach (GameInfo game in response.Games) {
                        if (game.State == 1)
                            state = "Vincitore";
                        else if (game.State == 0)
                            state = "Perdente";
                        else
                            state = "Incompleta";

                        Games.Add(new Game(game.Id,game.Date, state));
                    }

                    BindingContext = this;  
                    HideLoadingAnimation();
                }
                
                
            } catch (Exception e) {
                Console.WriteLine(e);
                HideLoadingAnimation(e.Message);
            } 
        }



        private async void OnCloseButtonClicked(object o, EventArgs args) {
            await Navigation.PopModalAsync();
        }

        private async void OnGameTapped(object o, ItemTappedEventArgs item) {
            // Ottieni l'ID della partita selezionata
            Game selectedGame = (Game) item.Item;
            int gameId = selectedGame.Id;
            Console.WriteLine($"Partita selezionata con ID: {gameId}");
            // Esempio: Navigare a una nuova pagina passando l'ID
            await Navigation.PushModalAsync(new GameStatisticsView(gameId));

        }

        
    }
}

