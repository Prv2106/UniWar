using System.Xml.XPath;

namespace UniWar {
    public partial class NewUserTurn : ContentPage {
        public NewUserTurn() {
            InitializeComponent();
            
        }

        public async void OnConfirmButtonClicked (object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}