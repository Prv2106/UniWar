
namespace UniWar {
    /// <summary>
    /// Modal that asks the user a yes/no question.
    /// </summary>
    public partial class YesOrNotModal : ContentPage {
        private TaskCompletionSource<bool> _tcs;

        /// <summary>
        /// Initializes the modal.
        /// </summary>
        /// <param name="tcs">Source completed with the user's choice.</param>
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