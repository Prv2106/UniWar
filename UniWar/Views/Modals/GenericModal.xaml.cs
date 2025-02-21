namespace UniWar{
    public partial class GenericModal: ContentPage{
            private readonly TaskCompletionSource _tcs;

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

            
        public async void OnConfirmButtonClicked(object sender, EventArgs args) {
            _tcs.SetResult();
            await Navigation.PopModalAsync();
        }


    }

}