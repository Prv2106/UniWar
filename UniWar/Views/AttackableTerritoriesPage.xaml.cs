namespace UniWar {
    public partial class AttackableTerritoriesPage : ContentPage {
        public List<string> AttackableTerritories {get;} = [];
        private string AttackingTerritoryName {get;}
        // pubblico perchè deve essere accessibilen allo XAML
        public AttackableTerritoriesPage(List<string> neighboringTerritories, string attackingTerritory) {
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
            List<int> userDice;
            List<int> cpuDice;
            string result;
            try {
                (userDice, cpuDice, result) = UniWarSystem.Instance.AttackTerritory(AttackingTerritoryName.RemoveSpaces(), button!.CommandParameter.ToString()!.RemoveSpaces());
                // dopo quest'operazione, il sistema ha già aggiornato la situazione dei giocatori...
                // aggiorniamo la UI
                TablePage.Instance.DeployTanks();
                TablePage.Instance.BuildUserInformation();
                
                ShowDiceResultPage diceResultPage = new ShowDiceResultPage(userDice, cpuDice, result);
                // TablePage.Instance.NewPage = diceResultPage;
                TablePage.Instance.OpenNewModal(diceResultPage);
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