
namespace UniWar{
    public partial class WinOrLoseModal: ContentPage{
            private TaskCompletionSource _tcs;
            public WinOrLoseModal(bool PlayerWin,TaskCompletionSource tcs, bool draw = false) {
                InitializeComponent();

                if(draw){
                    ImageInfo.Source = "agreement.png";
                    Header.Text = ""; 
                    Info.Text="Tu e la Cpu avete pareggiato!";
                }
                else if(PlayerWin){
                    ImageInfo.Source = "win.png";
                    Header.Text = "Congratulazioni!"; 
                    Info.Text="Hai raggiunto l'obiettivo ed hai vinto!";
                }
                else{
                    ImageInfo.Source = "game_over.png";
                    Header.Text = "Peccato!"; 
                    Info.Text="La Cpu ha raggiunto l'obiettivo prima di te e ha vinto!";
                }

                _tcs = tcs;


            }
            public void OnNewGameClicked(object sender, EventArgs args) {
                UniWarSystem.Instance.NewGame();
                Navigation.InsertPageBefore(new InitializationSummary(), this);
                _tcs.SetResult();
                Navigation.RemovePage(this);
            }
    }

}