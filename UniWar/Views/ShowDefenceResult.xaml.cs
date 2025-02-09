namespace UniWar {
    public partial class ShowDefenceResult: ContentPage{
        private TaskCompletionSource _tcs;
        public ShowDefenceResult(TaskCompletionSource tcs, string territory){
            InitializeComponent();
            _tcs = tcs;
            Info.Text = territory.AddSpaces(); 
            
        }


        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            _tcs.SetResult();
            await Navigation.PopModalAsync();
        }
    }
}