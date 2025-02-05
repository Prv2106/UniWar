using System.Text.RegularExpressions;
using System.Xml.XPath;

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
            // qui dovremmo invocare l'operazione di sistema che simula lo scontro.
            var button = sender as Button;
            /*
                il metodo di UniWarSystem che restituisce una tupla contenente:
                    - List<int> dado utente
                    - List<int> dado cpu
            */
            List<int> userDice;
            List<int> cpuDice;
            (userDice, cpuDice) = UniWarSystem.Instance.AttackTerritory(AttackingTerritoryName, button!.CommandParameter.ToString()!.RemoveSpaces());
            // dopo quest'operazione, il sistema ha già aggiornato la situazione dei giocatori...
            TablePage.Instance.DeployTanks();
            TablePage.Instance.BuildUserInformation();
            // chiudiamo la modale attuale
            await Navigation.PopModalAsync();
            
        }

        private async void OnCancelButtonClicked(object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}