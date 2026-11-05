namespace UniWar {
    /// <summary>
    /// Modal that displays the dice rolled during a battle and the resulting
    /// tank losses.
    /// </summary>
    public partial class ShowDiceResultPage : ContentPage {
        private TaskCompletionSource _tsc;

        /// <summary>
        /// Initializes the modal and renders the dice images.
        /// </summary>
        /// <param name="userDice">User dice values.</param>
        /// <param name="cpuDice">CPU dice values.</param>
        /// <param name="tanksLostForUser">Tanks lost by the user.</param>
        /// <param name="tanksLostForCpu">Tanks lost by the CPU.</param>
        /// <param name="tsc">Source completed when the modal is closed.</param>
        public ShowDiceResultPage(List<int> userDice, List<int> cpuDice, int tanksLostForUser, int tanksLostForCpu, TaskCompletionSource tsc) {
            InitializeComponent();

            for (int i = 0; i < userDice.Count; i++) {
                if (userDice[i] == 0) continue;  // Zero marks a die not rolled by the C++ algorithm
                Image img = (Image) userDiceImages.Children[i];
                img.Source = "attack" + userDice[i] + ".png";
                img.IsVisible = true;
            }

            for (int i = 0; i < cpuDice.Count; i++) {
                if (cpuDice[i] == 0) continue;  // Zero marks a die not rolled by the C++ algorithm
                Image img = (Image) cpuDiceImages.Children[i];
                img.Source = "defend" + cpuDice[i] + ".png";
                img.IsVisible = true;
            }

            WarningText.Text = $"Carri armati persi: Tu = {tanksLostForUser}, CPU = {tanksLostForCpu}";

            _tsc = tsc;
        }

        /// <summary>
        /// Closes the modal and signals completion.
        /// </summary>
        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            _tsc.SetResult();
            await Navigation.PopModalAsync();
        }
    }
}