namespace UniWar{
    public partial class GenericModal: ContentPage{
            private TaskCompletionSource _tcs;

            public GenericModal(string text, string info ,TaskCompletionSource tcs,int numTanks= 1) {
                InitializeComponent();

                if(info == "NoAttack"){
                    ImageInfo.Source = "info.png";
                }
                else if(info == "Reinforcement"){
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

            
        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(3000);
            _tcs.SetResult(); 
            await Navigation.PopModalAsync();   
         
        }

    }

}