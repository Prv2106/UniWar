
using System.Text.RegularExpressions;

namespace UniWar {
    public partial class InitializationSummary : ContentPage {
        /*
            Pagina dove viene mostrato il resoconto in seguito all'inizializzazione
            di una nuova partita.
        */

        public Player User {get;}
        public Player CPU {get;}

        public List<string> UserTerritories {get;} = [];
        

        
        public InitializationSummary() {
            InitializeComponent();

            (User, CPU) = UniWarSystem.Instance.InitializeGame();

            // nomi dei territori per le carte
            foreach (var territory in User.Territories) 
                // mettiamo gli spazi
                UserTerritories.Add(Regex.Replace(territory.Name, "(?<!^)([A-Z])", " $1"));
            BindingContext = this; // serve far si che CollectionView possa accedere alle proprietà

            // colore carro armato
            

        }

        public async void onBackButtonClicked(object o, EventArgs args) {
            await Navigation.PopAsync();
        } 

        public async void onConfirmButtonClicked(object o, EventArgs args) {
            await Navigation.PushAsync(new TablePage(User,CPU));
            Navigation.RemovePage(this);
        }

        
    }
}

