namespace UniWar {
    /// <summary>
    /// Introductory modal for the user attack phase.
    /// </summary>
    public partial class NewUserAttackTurn : ContentPage {
        /// <summary>
        /// Initializes the modal.
        /// </summary>
        public NewUserAttackTurn() {
            InitializeComponent();
            
        }

        private async void OnConfirmButtonClicked (object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}