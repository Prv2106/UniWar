namespace UniWar{
    /// <summary>
    /// Sign-in and sign-up page, including the offline game entry point.
    /// </summary>
    public partial class SignInUp: ContentPage{

        private bool _isSignIn;

        /// <summary>
        /// Creates the page in sign-in mode.
        /// </summary>
        public SignInUp() : this(true) {}

        /// <summary>
        /// Creates the page and redirects to the main page when a session is
        /// already active.
        /// </summary>
        /// <param name="signIn">Whether to start in sign-in mode.</param>
        public SignInUp(bool signIn = true) {
            InitializeComponent();

            // Check whether a login session already exists.
            if (UniWarSystem.Instance.LoggedUsername != null) {
                // Navigate to the main page.
                Navigation.PushAsync(new MainPage());
            }

            _isSignIn = signIn;
            UpdateUI();
        }

        /// <summary>
        /// Rewires event handlers and labels for the active authentication
        /// mode.
        /// </summary>
        private async void UpdateUI() {
            UsernameEntry.TextColor = Colors.White;

            Button.Clicked -= OnSignInButtonClicked!;
            Button.Clicked -= OnSignUpButtonClicked!;

            UsernameEntry.Completed -= OnSignInButtonClicked!;
            PasswordEntry.Completed -= OnSignInButtonClicked!;
            UsernameEntry.Completed -= OnSignUpButtonClicked!;
            PasswordEntry.Completed -= OnSignUpButtonClicked!;

            UsernameEntry.Unfocused -= OnUsernameUnFocused!;
            UsernameEntry.Focused -= OnUsernameFocused!;
    
            if (_isSignIn) {
                Header.Text = "Inserisci nome utente e password";
                ImageInfo.Source = "military_hat.png";
                Button.Text = "Accedi";
                Button.Clicked += OnSignInButtonClicked!;
                UsernameEntry.Completed += OnSignInButtonClicked!;
                PasswordEntry.Completed += OnSignInButtonClicked!;
                SwitchModeLabel.Text = "Se non hai ancora un account, Registrati";
            } else {
                Header.Text = "Crea un nome utente e una password";
                ImageInfo.Source = "soldier.png";
                Button.Text = "Registrati";
                await Task.Delay(100);
                Button.Clicked += OnSignUpButtonClicked!;
                UsernameEntry.Completed += OnSignUpButtonClicked!;
                PasswordEntry.Completed += OnSignUpButtonClicked!;

                UsernameEntry.Unfocused += OnUsernameUnFocused!;
                UsernameEntry.Focused += OnUsernameFocused!;
                SwitchModeLabel.Text = "Hai già un account? Accedi";
            }

            UsernameEntry.TextChanged -= OnEntryTextChanged!;
            PasswordEntry.TextChanged -= OnEntryTextChanged!;
            UsernameEntry.TextChanged += OnEntryTextChanged!;
            PasswordEntry.TextChanged += OnEntryTextChanged!;
        }

        private void OnSwitchModeTapped(object sender, EventArgs e) {
            _isSignIn = !_isSignIn;
            warning.IsVisible = false;
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            UpdateUI(); 
        }


        private void OnEntryTextChanged(object sender, TextChangedEventArgs e) {
            warning.IsVisible = false;
        }


         private void ShowWarning(string message) {
            warning.Text = message;
            warning.IsVisible = true;
        }

        private void HideWarning() {
            warning.Text = "";
            warning.IsVisible = false;
        }

        private void ShowOrHideLoading() {
            if (!loading.IsVisible) {
                loading.IsVisible = true;
                warning.IsVisible = false;
            } else {
                warning.IsVisible = true;
                loading.IsVisible = false;
            }   
        }

        /// <summary>
        /// Authenticates the user and opens the main page on success.
        /// </summary>
        public async void OnSignInButtonClicked(object sender, EventArgs args) {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) {
                ShowWarning("Devi inserire entrambi i campi");
                return;
            }
            
            ShowOrHideLoading();
            try { 
                var response = await ClientGrpc.SignIn(username, password);
                if (response.Status == false) {
                    ShowWarning(response.Message);
                    return;
                }
                UniWarSystem.Instance.SetLogged(username);
                HideWarning();
                await Navigation.PushAsync(new MainPage());
            } catch (Grpc.Core.RpcException ex) {
                Console.WriteLine($"{ex}");
                ShowWarning("Non è stato possibile effettuare l'accesso per qualche problema nella comunicazione con il server");
            } catch (Exception ex) {
                Console.WriteLine($"{ex}");
                ShowWarning("Si è verificato un errore sconosciuto durante l'accesso");
            } finally {
                ShowOrHideLoading();
            }
        }

        /// <summary>
        /// Registers the user and opens the main page on success.
        /// </summary>
        public async void OnSignUpButtonClicked(object sender, EventArgs args) {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)){
                ShowWarning("Devi inserire entrambi i campi");
                return;
            }
            
            ShowOrHideLoading();
            try {
                var response = await ClientGrpc.SignUp(username, password);
                if(response.Status == false){
                    ShowWarning(response.Message);
                    return;
                }
                HideWarning();
                UniWarSystem.Instance.SetLogged(username);
                await Navigation.PushAsync(new MainPage());
            } catch (Grpc.Core.RpcException ex) {
                Console.WriteLine($"{ex}");
                ShowWarning("Non è stato possibile effettuare la registrazione per qualche problema nella comunicazione con il server");
            } catch (Exception ex) {
                Console.WriteLine($"{ex}");
                ShowWarning("Si è verificato un errore sconosciuto durante la registrazione");
            } finally {
                ShowOrHideLoading();
            }
           

        }

        /// <summary>
        /// Validates the username when the entry loses focus.
        /// </summary>
        public async void OnUsernameUnFocused(object sender, EventArgs args) {
            
            if(string.IsNullOrWhiteSpace(UsernameEntry.Text))
                return;

            try {
                var response = await ClientGrpc.UsernameCheck(UsernameEntry.Text);
                if (response.Status == false) {
                    ShowWarning(response.Message);
                    UsernameEntry.TextColor = Colors.Red;
                    return;
                }
                UsernameEntry.TextColor = Colors.LightGreen;
            }
            catch (Grpc.Core.RpcException ex) {
                Console.WriteLine($"{ex}");
                ShowWarning("Non è stato possibile verificare lo username a causa di qualche problema nella comunicazione con il server");
            }
            catch (Exception ex) {
                Console.WriteLine($"{ex}");
                ShowWarning("Non è stato possibile verificare lo username a causa di un errore sconosciuto");
            }

        }

        private void OnUsernameFocused(object sender, EventArgs args){
            UsernameEntry.TextColor = Colors.White;
        }

        /// <summary>
        /// Enables offline mode and opens the main page.
        /// </summary>
        private async void OnPlayOfflineClicked(object sender, EventArgs eventArg) {
            // Enable offline mode.
            UniWarSystem.Instance.OfflineMode();

            // Navigate to the game page.
            await Navigation.PushAsync(new MainPage());
        }
    }

}