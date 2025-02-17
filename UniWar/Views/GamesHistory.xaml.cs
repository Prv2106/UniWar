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

        
        public GamesHistory(string playerUsername) {
            InitializeComponent();
            BindingContext = this;    
    
            // Facciamo la query direttamente nel costruttore della pagina
            // per recuperare l'elenco delle partite per l'utente loggato
            try {
                GameInfoList response = ClientGrpc.GetGames(playerUsername);
                Console.WriteLine("Ho ricevuto la risposta");
                foreach (GameInfo game in response.Games) 
                    Games.Add(new Game(game.Id,game.Date,game.State));
                } catch (Exception e) {
               Console.WriteLine(e);
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

