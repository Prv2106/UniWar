namespace UniWar {
    /// <summary>
    /// Modal that shows which territories are involved in a CPU attack.
    /// </summary>
    public partial class ShowCpuBattleTerritory: ContentPage{
        private TaskCompletionSource _tcs;

        /// <summary>
        /// Initializes the modal with the attacking and defending territories.
        /// </summary>
        /// <param name="cpuTerritory">Attacking CPU territory.</param>
        /// <param name="playerTerritory">Defending user territory.</param>
        /// <param name="tcs">Source completed when the modal is closed.</param>
        public ShowCpuBattleTerritory(string cpuTerritory, string playerTerritory, TaskCompletionSource tcs) {
            InitializeComponent();

            this.cpuTerritory.Text = cpuTerritory.AddSpaces();
            this.playerTerritory.Text = playerTerritory.AddSpaces();

            _tcs = tcs;
        }


        /// <summary>
        /// Closes the modal and signals completion.
        /// </summary>
        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            _tcs.SetResult();
            await Navigation.PopModalAsync();
        }

    }
}