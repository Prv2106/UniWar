namespace UniWar {
    public partial class ShowCpuDiceResultPage : ContentPage {

        public ShowCpuDiceResultPage(List<int> userDice, List<int> cpuDice, int CpuLosses, int PlayerLosses) {
            InitializeComponent();

            for (int i = 0; i < userDice.Count; i++) {
                Image img = (Image) userDiceImages.Children[i];
                img.Source = "defend" + userDice[i] + ".png";
                img.IsVisible = true;
            }

            for (int i = 0; i < cpuDice.Count; i++) {
                Image img = (Image) cpuDiceImages.Children[i];
                img.Source = "attack" + cpuDice[i] + ".png";
                img.IsVisible = true;
            }
            if(PlayerLosses == 0){
                if (CpuLosses == 1)
                    WarningText.Text = "Non hai perso carri armati da questa battaglia, mentre la cpu ne ha perso " + CpuLosses;
                else
                    WarningText.Text = "Non hai perso carri armati da questa battaglia, mentre la cpu ne ha persi " + CpuLosses;
            }
            else{
                 WarningText.Text = "Hai perso " + PlayerLosses;
                if(PlayerLosses == 1)
                    WarningText.Text += " carro armato";
                else
                    WarningText.Text += " carri armati";

                WarningText.Text += ", mentre la cpu ha perso " + CpuLosses;

                if(CpuLosses == 1)
                    WarningText.Text += " carro armato";
                else
                    WarningText.Text += " carri armati";


            }
              

            }

        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(3000); 
            await Navigation.PopModalAsync();   
         
        }

    }
}