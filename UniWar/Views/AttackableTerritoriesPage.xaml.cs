namespace UniWar {
    public partial class AttackableTerritoriesPage : ContentPage {
        public List<string> AttackableTerritories {get;} = [];
        private string AttackingTerritoryName {get;}
        // pubblico perchè deve essere accessibilen allo XAML
        public AttackableTerritoriesPage(List<string> neighboringTerritories, string attackingTerritory) {
            // attackingTerritory è senza spazi (chiave pronta per dizionario)
            InitializeComponent();
            foreach (var territory in neighboringTerritories) 
                // mettiamo gli spazi
                AttackableTerritories.Add(territory.AddSpaces());

            BindingContext = this; // serve far si che CollectionView possa accedere alle proprietà
            
            AttackingTerritoryName = attackingTerritory;
        }

        
        private async void OnTerritoryClicked (object sender, EventArgs eventArgs) {
            // qui invochiamo l'operazione di sistema che simula lo scontro.
            var button = sender as Button;
            try {
                string attackedTerritory = button!.CommandParameter.ToString()!.RemoveSpaces();
                TablePage.Instance.AttackTerritory(AttackingTerritoryName, attackedTerritory);                
                await Navigation.PopModalAsync();
            }
            catch (Exception e) {
                WarningText.Text = e.Message;
                WarningText.IsVisible = true;
                await Task.Delay(3000);
                WarningText.IsVisible = false;
            }
        }

        private async void OnCancelButtonClicked(object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}