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
            BindingContext = this;    
        }

        private void ShowLoadingAnimation() {
            loading.IsVisible = true;
            page.IsVisible = false;
            warning.IsVisible = false; 
        }

        private void HideLoadingAnimation(Exception? e = null) {
            // da invocare alla fine del blocco try o del catch
            if (e is not null) {
                // ci sono problemi
                page.IsVisible = false;
                loading.IsVisible = false;
                warning.Text = e.Message;
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
                foreach (GameInfo game in response.Games) 
                    Games.Add(new Game(game.Id,game.Date,game.State));
                HideLoadingAnimation();
            } catch (Exception e) {
                Console.WriteLine(e);
                HideLoadingAnimation(e);
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

