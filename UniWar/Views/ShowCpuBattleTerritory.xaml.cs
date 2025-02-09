namespace UniWar {
    public partial class ShowCpuBattleTerritory: ContentPage{
        public ShowCpuBattleTerritory(string cpuTerritory, string playerTerritory){
            InitializeComponent();

            this.cpuTerritory.Text = cpuTerritory.AddSpaces();
            this.playerTerritory.Text = playerTerritory.AddSpaces();


        }


        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(3000); 
            await Navigation.PopModalAsync();   
         
        }

    }
}