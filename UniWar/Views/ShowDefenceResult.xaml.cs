namespace UniWar {
    public partial class ShowDefenceResult: ContentPage{
        private TaskCompletionSource _tcs;
        public ShowDefenceResult(TaskCompletionSource tcs){
            InitializeComponent();
            _tcs = tcs;
            
        }


        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(2000);
            _tcs.SetResult();
            await Navigation.PopModalAsync();   
         
        }

    }
}