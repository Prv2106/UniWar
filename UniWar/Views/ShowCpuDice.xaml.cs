namespace UniWar {
    public partial class ShowCpuDiceResultPage : ContentPage {

        public ShowCpuDiceResultPage(List<int> userDice, List<int> cpuDice) {
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
        }

        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(4000); 
            await Navigation.PopModalAsync();   
         
        }

    }
}