namespace UniWar {
    public partial class ShowDiceResultPage : ContentPage {
        private TaskCompletionSource _tsc;

        public ShowDiceResultPage(List<int> userDice, List<int> cpuDice, int tanksLostForUser, int tanksLostForCpu, TaskCompletionSource tsc) {
            InitializeComponent();

            for (int i = 0; i < userDice.Count; i++) {
                if (userDice[i] == 0) continue; // nel caso dell'algoritmo in c++
                Image img = (Image) userDiceImages.Children[i];
                img.Source = "attack" + userDice[i] + ".png";
                img.IsVisible = true;
            }

            for (int i = 0; i < cpuDice.Count; i++) {
                if (cpuDice[i] == 0) continue; // nel caso dell'algoritmo in c++
                Image img = (Image) cpuDiceImages.Children[i];
                img.Source = "defend" + cpuDice[i] + ".png";
                img.IsVisible = true;
            }

            WarningText.Text = $"Carri armati persi: Tu = {tanksLostForUser}, CPU = {tanksLostForCpu}";

            _tsc = tsc;
        }

        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            _tsc.SetResult();
            await Navigation.PopModalAsync();
        }
    }
}