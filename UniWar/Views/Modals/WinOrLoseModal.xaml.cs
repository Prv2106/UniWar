
using System.Threading.Tasks;

namespace UniWar{
    public partial class WinOrLoseModal: ContentPage{
            public WinOrLoseModal(bool PlayerWin, bool draw = false) {
                InitializeComponent();

                if (draw) {
                    ImageInfo.Source = "agreement.png";
                    Header.Text = ""; 
                    Info.Text="Tu e la Cpu avete pareggiato!";
                } else if (PlayerWin) {
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