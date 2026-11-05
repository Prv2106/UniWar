using Statistics;

namespace UniWar {
    /// <summary>
    /// Displays the game history of the logged-in user.
    /// </summary>
    public partial class GamesHistory : ContentPage {
        
        /// <summary>
        /// Row model displayed in the games table.
        /// </summary>
        public class Game { 
            public int Id {get;}
            public string Date {get; set;}
            public string State {get; set;}

            /// <summary>
            /// Initializes a game row.
            /// </summary>
            /// <param name="id">Game identifier.</param>
            /// <param name="date">Formatted game date.</param>
            /// <param name="state">Outcome label.</param>
            public Game(int id, string date, string state) {
                Id = id;
                Date = date;
                State = state;
            }
        }   
        

        // Public because it is bound from XAML.
        public List<Game> Games { get;} = [];

        
        public GamesHistory() {
            InitializeComponent();
             
        }

        /// <summary>
        /// Shows the loading indicator and hides the content.
        /// </summary>
        private void ShowLoadingAnimation() {
            loading.IsVisible = true;
            page.IsVisible = false;
            warning.IsVisible = false; 
        }

        /// <summary>
        /// Hides the loading indicator and shows the content or an error
        /// message.
        /// </summary>
        /// <param name="message">Optional error message.</param>
        private void HideLoadingAnimation(string? message = null) {
            // Call at the end of the try block or from the catch block.
            if (message is not null) {
                // Failure.
                page.IsVisible = false;
                loading.IsVisible = false;
                warning.Text = message;
                warning.IsVisible = true; 
            } else {
                // Success.
                loading.IsVisible = false;
                page.IsVisible = true;
                warning.IsVisible = false; 
            }
        }

        /// <summary>
        /// Loads the game history when the modal appears.
        /// </summary>
        protected async override void OnAppearing() {
            base.OnAppearing();
            
            // Query the gRPC client for the logged-in user's games.
            ShowLoadingAnimation();
            try {
                GameInfoList response = await ClientGrpc.GetGames(UniWarSystem.Instance.LoggedUsername!);
                Console.WriteLine("Ho ricevuto la risposta");
                if (response.Games.Count == 0 && response.Status == true) {
                    // No games are stored for the user.
                    HideLoadingAnimation("Non sono presenti partite da te giocate nel database!");
                } else if (response.Status == false) {
                    HideLoadingAnimation(response.Message);
                } else {  // Games are available
                    // Decode the base64 pie chart when present.
                    if (!string.IsNullOrEmpty(response.GameResultsPieChart)) {
                        byte[] imageBytes = Convert.FromBase64String(response.GameResultsPieChart);
                        var stream = new MemoryStream(imageBytes);
                        Chart.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
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



        /// <summary>
        /// Closes the modal.
        /// </summary>
        private async void OnCloseButtonClicked(object o, EventArgs args) {
            await Navigation.PopModalAsync();
        }

        /// <summary>
        /// Opens the statistics modal for the selected game.
        /// </summary>
        private async void OnGameTapped(object o, ItemTappedEventArgs item) {
            Game selectedGame = (Game) item.Item;
            int gameId = selectedGame.Id;
            Console.WriteLine($"Partita selezionata con ID: {gameId}");
            await Navigation.PushModalAsync(new GameStatisticsView(gameId));

        }

        
    }
}

