using System.Xml.XPath;

namespace UniWar {
    public partial class NewUserReinforcementTurn : ContentPage {
        public NewUserReinforcementTurn(int numtanksToAdd) {
            InitializeComponent();
            
        }

        private async void OnConfirmButtonClicked (object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}