namespace UniWar {
    public partial class NewUserAttackTurn : ContentPage {
        public NewUserAttackTurn() {
            InitializeComponent();
            
        }

        private async void OnConfirmButtonClicked (object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}