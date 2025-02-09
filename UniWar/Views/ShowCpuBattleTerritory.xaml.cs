namespace UniWar {
    public partial class ShowCpuBattleTerritory: ContentPage{
        private TaskCompletionSource _tcs;
        public ShowCpuBattleTerritory(string cpuTerritory, string playerTerritory, TaskCompletionSource tcs) {
            InitializeComponent();

            this.cpuTerritory.Text = cpuTerritory.AddSpaces();
            this.playerTerritory.Text = playerTerritory.AddSpaces();

            _tcs = tcs;
        }


        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            _tcs.SetResult();
            await Navigation.PopModalAsync();
        }

    }
}