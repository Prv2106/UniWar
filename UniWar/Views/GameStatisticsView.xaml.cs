using Statistics;

namespace UniWar {
    public partial class GameStatisticsView : ContentPage {
        /*
            Pagina dove vengono mostrate le statistiche
            (completate o non) dll'utente loggato
        */

        /*
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
        

        */

        
        public GameStatisticsView(int gameId) {
            InitializeComponent();
            BindingContext = this;    
    
            // Facciamo la query direttamente nel costruttore della pagina
            // per recuperare le statistiche relative alla gara in corso
            try {
                /*
                GameInfoList response = ClientGrpc.GetGames(playerUsername);
                Console.WriteLine("Ho ricevuto la risposta");
                foreach (GameInfo game in response.Games) 
                    Games.Add(new Game(game.Id,game.Date,game.State));
                */
                } catch (Exception e) {
               Console.WriteLine(e);
            }

                  
        }


    

        private async void OnCloseButtonClicked(object o, EventArgs args) {
            await Navigation.PopModalAsync();
        }

        
    }
}

