namespace UniWar {
    public partial class InitializationSummary : ContentPage {
        /*
            Pagina dove viene mostrato il resoconto in seguito all'inizializzazione
            di una nuova partita.
        */

        private Player? User {get; set;} // Ci teniamo un riferimento all'utente perché questa pagina è dedicata solo a lui

        public List<string> UserTerritories {get;} = [];
        // proprietà pubblica perchè deve essere accessibile allo XAML
        

        
        public InitializationSummary() {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
            InitializeComponent();
        }

        protected async override void OnAppearing() {
            try {
                if (UniWarSystem.Instance.IsGameInitialized) {
                    // stiamo iniziando una nuova partita ma ne è già stata fatta una
                    UniWarSystem.Instance.ResetAll();
                    await UniWarSystem.Instance.InitializeGame(); // caricamento dei dati
                } else {
                    // prima partita da quando l'utente ha avviato l'applicazione
                    await UniWarSystem.Instance.InitializeGame(); // caricamento dei dati
                }
            } catch (Exception e) {
                //TODO: fare un testo di warning a livello grafico
                Console.WriteLine("Si è verificata un'eccezione: " + e.Message);
            }
            
            User = UniWarSystem.Instance.User!;
            BuildThePage();           
        }

        // Per estrarre le informazioni da visualizzare nella card
        private void BuildThePage() {
            // nomi dei territori per le carte
            foreach (var territory in User!.Territories.Values) 
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

