
using System.Threading.Tasks;

namespace UniWar{
    /// <summary>
    /// Modal shown at the end of a game with the outcome.
    /// </summary>
    public partial class WinOrLoseModal: ContentPage{
            /// <summary>
            /// Initializes the modal with the supplied outcome.
            /// </summary>
            /// <param name="PlayerWin">Whether the user won.</param>
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

            /// <summary>
            /// Returns to the authentication shell.
            /// </summary>
            public void OnGoToMainPageButtonClicked(object sender, EventArgs args) {
                Application.Current!.MainPage = new AppShell();
            }
    }

}