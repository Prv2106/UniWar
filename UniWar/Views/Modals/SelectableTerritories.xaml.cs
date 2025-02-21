namespace UniWar {
    public partial class SelectableTerritories : ContentPage {
        public List<string> SelectableTerritoriesNames {get;} = [];

        private TaskCompletionSource<string> _territoryChoosen;
        
        // pubblico perchè deve essere accessibilen allo XAML
        public SelectableTerritories(List<string> neighboringTerritories, TaskCompletionSource<string> taskCompletionSource, bool isAttack) {
            // attackingTerritory è senza spazi (chiave pronta per dizionario)
            InitializeComponent();
            foreach (var territory in neighboringTerritories) 
                // mettiamo gli spazi
                SelectableTerritoriesNames.Add(territory.AddSpaces());

            BindingContext = this; // serve far si che CollectionView possa accedere alle proprietà

            _territoryChoosen = taskCompletionSource;

            if (isAttack) {
                // allora l'utente sta selezionando il territorio nemico da attaccare
                image.Source = "soldier.png";
                text.Text = "Seleziona il territorio nemico da attaccare!";
            } else {
                // sta effettuando uno spostamento strategico, i territori mostrati sono i suoi!
                image.Source = "running_soldiers.png";
                text.Text = "Seleziona uno dei tuoi territori confinanti dove vuoi spostare qualche carro armato!";
            }
        }

        
        private async void OnTerritoryClicked (object sender, EventArgs eventArgs) {
            // qui invochiamo l'operazione di sistema che simula lo scontro.
            var button = sender as Button;
            string attackedTerritory = button!.CommandParameter.ToString()!.RemoveSpaces();
            _territoryChoosen.SetResult(attackedTerritory);
            await Navigation.PopModalAsync();
        }

        private async void OnCancelButtonClicked(object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}