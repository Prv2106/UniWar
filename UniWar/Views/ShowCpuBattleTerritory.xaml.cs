namespace UniWar {
    public partial class ShowCpuBattleTerritory: ContentPage{
        public ShowCpuBattleTerritory(string cpuTerritory, string playerTerritory){
            InitializeComponent();

            this.cpuTerritory.Text = cpuTerritory;
            this.playerTerritory.Text = playerTerritory;


        }


        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(3000); 
            await Navigation.PopModalAsync();   
         
        }

    }
}