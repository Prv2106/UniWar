namespace UniWar {
    /// <summary>
    /// Modal that reports a territory lost in defense.
    /// </summary>
    public partial class ShowDefenceResult: ContentPage{
        private TaskCompletionSource _tcs;

        /// <summary>
        /// Initializes the modal with the lost territory.
        /// </summary>
        /// <param name="tcs">Source completed when the modal is closed.</param>
        /// <param name="territory">Name of the lost territory.</param>
        public ShowDefenceResult(TaskCompletionSource tcs, string territory){
            InitializeComponent();
            _tcs = tcs;
            Info.Text = territory.AddSpaces(); 
            
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