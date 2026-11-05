namespace UniWar{
    /// <summary>
    /// Generic informational modal with a confirmation button.
    /// </summary>
    public partial class GenericModal: ContentPage{
            private readonly TaskCompletionSource _tcs;

            /// <summary>
            /// Initializes the modal.
            /// </summary>
            /// <param name="text">Message to display.</param>
            /// <param name="reinforcement">Whether this is a reinforcement notice.</param>
            /// <param name="tcs">Source completed when the modal is closed.</param>
            /// <param name="numTanks">Number of reinforcement tanks.</param>
            public GenericModal(string text, bool reinforcement ,TaskCompletionSource tcs,int numTanks= 1) {
                InitializeComponent();

                if(!reinforcement){
                    ImageInfo.Source = "info.png";
                }
                else {
                    ImageInfo.Source = "shock.png";
                    TankCount.IsVisible = true;
                    if(numTanks > 1)
                        TankCount.Text = numTanks + " nuovi carri armati";
                    else
                        TankCount.Text = numTanks  + " nuovo carro armato";
                }

                textInfo.Text = text;
                
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