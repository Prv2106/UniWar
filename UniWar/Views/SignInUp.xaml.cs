using Windows.Storage.Provider;

namespace UniWar{
    public partial class SignInUp: ContentPage{

            private bool isSignIn;

            public SignInUp(bool signIn = true) {
                InitializeComponent();
                isSignIn = signIn;
                UpdateUI();
                
            }

        private void UpdateUI(){
                 Button.Clicked -= OnSigniInButtonClicked!;
                 Button.Clicked -= OnSigniUpButtonClicked!;
            
                if(isSignIn){
                    Header.Text = "Inserisci nome utente e password";
                    ImageInfo.Source = "military_hat.png";
                    Button.Text = "Accedi";
                    Button.Clicked += OnSigniInButtonClicked!;
                    SwitchModeLabel.Text = "Se non hai ancora un account, Registrati";
                }
                else{
                    Header.Text = "Crea un nome utente e una password";
                    ImageInfo.Source = "new_soldier.png";
                    Button.Text = "Registrati";
                    Button.Clicked += OnSigniUpButtonClicked!;
                    SwitchModeLabel.Text = "Hai già un account? Accedi";
                }

                // Assicura che il messaggio di errore scompaia quando l'utente scrive
                UsernameEntry.TextChanged -= OnEntryTextChanged!;
                PasswordEntry.TextChanged -= OnEntryTextChanged!;
                UsernameEntry.TextChanged += OnEntryTextChanged!;
                PasswordEntry.TextChanged += OnEntryTextChanged!;
        }


        private void OnSwitchModeTapped(object sender, EventArgs e) {
            isSignIn = !isSignIn;
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




        public async void OnSigniInButtonClicked(object sender, EventArgs args) {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)){
                ShowWarning("Devi inserire entrambi i campi");
                return;
            }
            
            try {
                ClientGrpc.SignIn(username, password);
                await Navigation.PushAsync(new InitializationSummary(username));
            }
             catch (Grpc.Core.RpcException e) {
                warning.Text = "Non è stato possibile effettuare il login per qualche problema nella rpc";
                warning.IsVisible = true;
                Console.WriteLine($"Errore: {e}");
            }
            catch (Exception) {
                warning.Text =  "Si è verificato un errore sconosciuto durante il login";
            }
           
            
        }

        public async void OnSigniUpButtonClicked(object sender, EventArgs args) {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)){
                ShowWarning("Devi inserire entrambi i campi");
                return;
            }
            
            try{
                var response = ClientGrpc.SignUp(username, password);
                if(response.Message == "Invalid Username"){
                    ShowWarning("Username non valido: deve iniziare con una lettera maiuscola e avere almeno un altro carattere");
                    return;
                }
                
                if(response.Message == "Unavaible Username"){
                    ShowWarning("Username già in uso");
                    return;
                }

                if(response.Message == "Database Error"){
                    ShowWarning("Errore nel database");
                    return;
                }

                if(response.Status == false){
                    ShowWarning("C'è stato un problema");
                    return;
                }

                await Navigation.PushAsync(new InitializationSummary(username));
            }
             catch (Grpc.Core.RpcException e) {
                warning.Text = "Non è stato possibile effettuare la registrazione per qualche problema nella rpc";
                warning.IsVisible = true;
                Console.WriteLine($"Errore: {e}");
            }
            catch (Exception) {
                warning.Text =  "Si è verificato un errore sconosciuto durante la registrazione";
            }
           

        }

        




    }

}