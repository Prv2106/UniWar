namespace UniWar {
    /// <summary>
    /// Displays the summary shown after a new game is initialized.
    /// </summary>
    public partial class InitializationSummary : ContentPage {

        private Player? User {get; set;}  // Reference to the user for this user-only page

        // Public because it is bound from XAML.
        public List<string> UserTerritories {get;} = [];
        

        
        /// <summary>
        /// Initializes the page and disables the back button.
        /// </summary>
        public InitializationSummary() {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
            InitializeComponent();
        }

        /// <summary>
        /// Initializes or resets the game and builds the summary card.
        /// </summary>
        protected async override void OnAppearing() {
            try {
                if (UniWarSystem.Instance.IsGameInitialized) {
                    // A new game is starting after a previous one.
                    UniWarSystem.Instance.ResetAll();
                    await UniWarSystem.Instance.InitializeGame();
                } else {
                    // First game since the application started.
                    await UniWarSystem.Instance.InitializeGame();
                }
            } catch (Exception e) {
                Console.WriteLine("Si è verificata un'eccezione: " + e.Message);
            }
            
            User = UniWarSystem.Instance.User!;
            BuildThePage();           
        }

        /// <summary>
        /// Populates the summary card from the initialized user state.
        /// </summary>
        private void BuildThePage() {
            // Territory names shown on the cards.
            foreach (var territory in User!.Territories.Values) 
                UserTerritories.Add(territory.Name.AddSpaces());

             
            BindingContext = this;  // Allows CollectionView to access the properties

            // Tank color.
            string iconSrcUser = User.Territories.Values.First().Tanks[0].GetTankIconByColor();
            TankIcon.Source = iconSrcUser;

            // Goal.
            GoalDescription.Text = User.Goal?.Description;
        }

        /// <summary>
        /// Opens the game table and removes this page from the stack.
        /// </summary>
        private async void OnConfirmButtonClicked(object o, EventArgs args) {
            await Navigation.PushAsync(TablePage.Instance);
            Navigation.RemovePage(this);
        }

        
    }
}

