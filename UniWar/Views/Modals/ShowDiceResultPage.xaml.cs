namespace UniWar {
    public partial class ShowDiceResultPage : ContentPage {
        private TaskCompletionSource _tsc;

        public ShowDiceResultPage(List<int> userDice, List<int> cpuDice, int tanksLostForUser, int tanksLostForCpu, TaskCompletionSource tsc) {
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

            WarningText.Text = $"Hai perso {tanksLostForUser} carri armati, mentre la CPU ne ha persi {tanksLostForCpu}";

            _tsc = tsc;
        }

        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            _tsc.SetResult();
            await Navigation.PopModalAsync();
        }
    }
}