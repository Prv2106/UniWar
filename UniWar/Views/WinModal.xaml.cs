
namespace UniWar{
    public partial class WinModal: ContentPage{
            public WinModal() {
                InitializeComponent();
            }
            public async void OnNewGameClicked(object sender, EventArgs args) {
                await Navigation.PopModalAsync();
            }
    }

}