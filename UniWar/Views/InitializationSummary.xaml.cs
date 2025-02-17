namespace UniWar {
    public partial class InitializationSummary : ContentPage {
        /*
            Pagina dove viene mostrato il resoconto in seguito all'inizializzazione
            di una nuova partita.
        */

        private Player User {get;} // Ci teniamo un riferimento all'utente perché questa pagina è dedicata solo a lui

        public List<string> UserTerritories {get;} = [];
        // proprietà pubblica perchè deve essere accessibile allo XAML
        

        
        public InitializationSummary(string playerUsername) {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
            InitializeComponent();
            UniWarSystem.Instance.InitializeGame(); // caricamento dei dati
            User = UniWarSystem.Instance.User!;
            BuildThePage();           
        }


        // Per estrarre le informazioni da visualizzare nella card
        private void BuildThePage() {
            // nomi dei territori per le carte
            foreach (var territory in User.Territories.Values) 
                // mettiamo gli spazi
                UserTerritories.Add(territory.Name.AddSpaces());

             
            BindingContext = this; // serve far si che CollectionView possa accedere alle proprietà

            // colore carro armato
            string iconSrcUser = User.Territories.Values.First().Tanks[0].GetTankIconByColor();
            TankIcon.Source = iconSrcUser; // TankIcon è il name dell'Image nello XAML

            // obiettivo
            GoalDescription.Text = User.Goal?.Description;
        }

        private async void OnConfirmButtonClicked(object o, EventArgs args) {
            // passiamo gli oggetti player alla pagina successiva
            // che mostrerà la mappa
            await Navigation.PushAsync(TablePage.Instance);
            Navigation.RemovePage(this);
        }

        
    }
}

