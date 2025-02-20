
using System.Threading.Tasks;

namespace UniWar{
    public partial class WinOrLoseModal: ContentPage{
            public WinOrLoseModal(bool PlayerWin) {
                InitializeComponent();

                if (PlayerWin) {
                    ImageInfo.Source = "win.png";
                    Header.Text = "Congratulazioni!"; 
                    Info.Text="Hai raggiunto l'obiettivo ed hai vinto!";
                } else {
                    ImageInfo.Source = "game_over.png";
                    Header.Text = "Peccato!"; 
                    Info.Text="La Cpu ha raggiunto l'obiettivo prima di te e ha vinto!";
                }
            }

            public void OnGoToMainPageButtonClicked(object sender, EventArgs args) {
                // Navighiamo fino alla pagina di login / registrazione
                Application.Current!.MainPage = new AppShell();
            }
    }

}