namespace UniWar{
    public partial class GenericModal: ContentPage{


            public GenericModal(string text, string info ,int numTanks= 1) {
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
                
               



            }

            
        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(3000); 
            await Navigation.PopModalAsync();   
         
        }

    }

}