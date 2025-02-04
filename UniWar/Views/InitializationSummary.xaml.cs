
using System.Text.RegularExpressions;

namespace UniWar {
    public partial class InitializationSummary : ContentPage {
        /*
            Pagina dove viene mostrato il resoconto in seguito all'inizializzazione
            di una nuova partita.
        */

        private Player User {get; set;}
        private Player CPU {get; set;}

        public List<string> UserTerritories {get;} = [];
        // proprietà pubblica perchè deve essere accessibile allo XAML
        

        
        public InitializationSummary() {
            InitializeComponent();
            (User, CPU) = UniWarSystem.Instance.InitializeGame();
            BuildThePage();           
        }

        private void BuildThePage() {
            // nomi dei territori per le carte
            foreach (var territory in User.Territories) 
                // mettiamo gli spazi
                UserTerritories.Add(Regex.Replace(territory.Name, "(?<!^)([A-Z])", " $1"));
            BindingContext = this; // serve far si che CollectionView possa accedere alle proprietà

            // colore carro armato
            string iconSrcUser = User.Territories[0].Tanks[0].GetTankIconByColor();
            TankIcon.Source = iconSrcUser; // TankIcon è il name dell'Image nello XAML

            // obiettivo
            GoalDescription.Text = User.Goal?.Description;
        }

        private async void OnConfirmButtonClicked(object o, EventArgs args) {
            // passiamo gli oggetti player alla pagina successiva
            // che mostrerà la mappa
            await Navigation.PushAsync(new TablePage(User,CPU));
            Navigation.RemovePage(this);
        }

        
    }
}

