namespace UniWar {
    public partial class ShowDefenceResult: ContentPage{
        public ShowDefenceResult(){
            InitializeComponent();
            
        }


        protected override async void OnAppearing() {
            base.OnAppearing();
            await Task.Delay(2000); 
            await Navigation.PopModalAsync();   
         
        }

    }
}