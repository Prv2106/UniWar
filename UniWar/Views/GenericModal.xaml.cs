
namespace UniWar{
    public partial class GenericModal: ContentPage{
            public GenericModal(string text) {
                InitializeComponent();

                // agganciamoci alla label dello xaml e inizializziamolo
                textInfo.Text = text;
            }
            public async void OnConfirmButtonClicked(object sender, EventArgs args) {
                await Navigation.PopModalAsync();
            }


    }

}