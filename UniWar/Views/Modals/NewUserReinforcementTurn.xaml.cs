using System.Xml.XPath;

namespace UniWar {
    /// <summary>
    /// Introductory modal for the user reinforcement phase.
    /// </summary>
    public partial class NewUserReinforcementTurn : ContentPage {
        /// <summary>
        /// Initializes the modal.
        /// </summary>
        public NewUserReinforcementTurn() {
            InitializeComponent();
            
        }

        private async void OnConfirmButtonClicked (object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}