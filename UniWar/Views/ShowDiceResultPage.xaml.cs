namespace UniWar {
    public partial class ShowDiceResultPage : ContentPage {

        public ShowDiceResultPage(List<int> userDice, List<int> cpuDice, string result) {
            InitializeComponent();
        }

        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            await Navigation.PopModalAsync();
        }
    }
}