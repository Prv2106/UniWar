namespace UniWar{
    public partial class SignInUp: ContentPage{

        private bool _isSignIn;
        public SignInUp() : this(true) {}

        public SignInUp(bool signIn = true) {
            InitializeComponent();

            // come prima cosa, controlliamo se è già presente una "sessione" di login
            if (UniWarSystem.Instance.IsGameInitialized) {
                // allora dovremmo essere nella main page
                Navigation.PushAsync(new MainPage());
            }

            _isSignIn = signIn;
            UpdateUI();
        }

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

            // Assicura che il messaggio di errore scompaia quando l'utente scrive
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

        private void ShowOrHideLoading() {
            if (!loading.IsVisible) {
                loading.IsVisible = true;
                warning.IsVisible = false;
            } else {
                warning.IsVisible = true;
                loading.IsVisible = false;
            }   
        }

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
                await Navigation.PushAsync(new MainPage());
            } catch (Grpc.Core.RpcException) {
                ShowWarning("Non è stato possibile effettuare l'accesso per qualche problema nella remote procedure call");
            } catch (Exception) {
                ShowWarning("Si è verificato un errore sconosciuto durante l'accesso");
            } finally {
                ShowOrHideLoading();
            }
        }

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
                Console.WriteLine("SignUp effettuato con successo");
                UniWarSystem.Instance.SetLogged(username);
                await Navigation.PushAsync(new MainPage());
            } catch (Grpc.Core.RpcException) {
                ShowWarning("Non è stato possibile effettuare la registrazione per qualche problema nella remote procedure call");
            } catch (Exception) {
                ShowWarning("Si è verificato un errore sconosciuto durante la registrazione");
            } finally {
                ShowOrHideLoading();
            }
           

        }

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
            catch (Grpc.Core.RpcException) {
                ShowWarning("Non è stato possibile effettuare la registrazione per qualche problema nella rpc");
            }
            catch (Exception) {
                ShowWarning("Si è verificato un errore sconosciuto durante la registrazione");
            }

        }

        private void OnUsernameFocused(object sender, EventArgs args){
            UsernameEntry.TextColor = Colors.White;
        }

        private async void OnPlayOfflineClicked(object sender, EventArgs eventArg) {
            // L'utente vuole giocare offline...

            // prima impostiamo la modalità offline nel gioco
            UniWarSystem.Instance.OfflineMode();

            // adesso mandiamo l'utente nella pagina di gioco
            await Navigation.PushAsync(new MainPage());
        }
    }

}