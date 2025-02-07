namespace UniWar {
    public partial class ShowDefenceResult: ContentPage{
        public ShowDefenceResult(bool tankLoss, bool territoryLoss , int playerLosses){
            InitializeComponent();
            if(territoryLoss){
                ImageInfo.Source = "white_flag.png";
                TextInfo.Text = "Hai perso il territorio";
            }
            else if(!tankLoss){
                ImageInfo.Source = "shield.png";
                TextInfo.Text = "Non hai perso carri armati dalla battaglia";

            }
            else{
                ImageInfo.Source = "sad.png";
                if(playerLosses > 1)
                    LossesInfo.Text = "Hai perso " + playerLosses + " carri armati";
                else
                    LossesInfo.Text = "Hai perso " + playerLosses + " carro armato";
            }



        }


        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(2500); 
            await Navigation.PopModalAsync();   
         
        }

    }
}