
namespace UniWar {
    public partial class YesOrNotModal : ContentPage {
        private TaskCompletionSource<bool> _tcs;
        public YesOrNotModal(TaskCompletionSource<bool> tcs) {
            InitializeComponent();
            _tcs = tcs;
        }

        private async void OnYesButtonClicked (object sender, EventArgs eventArgs) {
            _tcs.SetResult(true);
            await Navigation.PopModalAsync();
        }

        private async void OnNoButtonClicked (object sender, EventArgs eventArgs) {
             _tcs.SetResult(false);
            await Navigation.PopModalAsync();
        }


    }
}