using System.Xml.XPath;

namespace UniWar {
    public partial class NewUserReinforcementTurn : ContentPage {
        public NewUserReinforcementTurn() {
            InitializeComponent();
            
        }

        private async void OnConfirmButtonClicked (object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}