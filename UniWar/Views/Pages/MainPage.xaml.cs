using Statistics;

namespace UniWar {
    /// <summary>
    /// Home page shown after authentication or in offline mode.
    /// </summary>
    public partial class MainPage : ContentPage {
        /// <summary>
        /// Configures the page according to the active session mode.
        /// </summary>
        public MainPage() {
            InitializeComponent();

            if (!UniWarSystem.Instance.IsOffline) {
                // Online user.
                Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
                username.Text = UniWarSystem.Instance.LoggedUsername;
            } else {  // Offline user
                LoggedPart.IsVisible = false;
                History.IsVisible = false;
            }
        }

        /// <summary>
        /// Opens the new-game initialization page.
        /// </summary>
        private async void OnNuovaPartitaButtonClicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new InitializationSummary()); 
        }

        /// <summary>
        /// Opens the game history modal.
        /// </summary>
        private async void OnVisualizzaStoricoButtonClicked(object sender, EventArgs e) {
            await Navigation.PushModalAsync(new GamesHistory());
        }

        /// <summary>
        /// Clears the session and returns to the authentication shell.
        /// </summary>
        private void OnLogoutClicked(object sender, EventArgs e) {
            UniWarSystem.Instance.OfflineMode();
            Application.Current!.MainPage = new AppShell();
        }
    } 
}