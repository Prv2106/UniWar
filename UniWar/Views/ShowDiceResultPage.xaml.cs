namespace UniWar {
    public partial class ShowDiceResultPage : ContentPage {

        public ShowDiceResultPage(List<int> userDice, List<int> cpuDice, string result) {
            InitializeComponent();

            for (int i = 0; i < userDice.Count; i++) {
                Image img = (Image) userDiceImages.Children[i];
                img.Source = "attack" + userDice[i] + ".png";
                img.IsVisible = true;
            }

            for (int i = 0; i < cpuDice.Count; i++) {
                Image img = (Image) cpuDiceImages.Children[i];
                img.Source = "defend" + cpuDice[i] + ".png";
                img.IsVisible = true;
            }

            WarningText.Text = result;
        }

        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            await Navigation.PopModalAsync();
        }
    }
}