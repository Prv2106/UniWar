using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace UniWar {
    public partial class AttackableTerritoriesPage : ContentPage {
        public List<string> AttackableTerritories {get;} = [];
        // pubblico perchè deve essere accessibilen allo XAML
        public AttackableTerritoriesPage(List<Territory> territories) {
            InitializeComponent();
            foreach (var territory in territories) 
                // mettiamo gli spazi
                AttackableTerritories.Add(territory.Name.AddSpaces());
            BindingContext = this; // serve far si che CollectionView possa accedere alle proprietà         
        }

        private  void OnTerritoryClicked (object sender, EventArgs eventArgs) {
            // qui dovremmo invocare l'operazione di sistema che simula lo scontro.
            // il metodo di UniWarSystem dovrebbe restituire una tupla contenente 3 elementi:
            // - 
        }

        private async void OnCancelButtonClicked(object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}