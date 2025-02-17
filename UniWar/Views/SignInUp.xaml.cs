using System.Threading.Tasks;
using Statistics;
using Windows.Storage.Provider;

namespace UniWar{
    public partial class SignInUp: ContentPage{

        private bool isSignIn;
        public bool isUsernameValid {get; set;}
        public SignInUp() : this(true){}

        public SignInUp(bool signIn = true) {
            InitializeComponent();
            isSignIn = signIn;
            UpdateUI();
            
        }

        private async Task UpdateUI(){
                 UsernameEntry.TextColor = Colors.White;
                 Button.Clicked -= OnSigniInButtonClicked!;
                 Button.Clicked -= OnSigniUpButtonClicked!;
                 UsernameEntry.Completed -= OnSigniInButtonClicked!;
                 PasswordEntry.Completed -= OnSigniInButtonClicked!;
                 UsernameEntry.Completed -= OnSigniUpButtonClicked!;
                 PasswordEntry.Completed -= OnSigniUpButtonClicked!;

                UsernameEntry.Unfocused -= OnUsernameUnFocused!;
                UsernameEntry.Focused -= OnUsernameFocused!;
            
                if(isSignIn){
                    Header.Text = "Inserisci nome utente e password";
                    ImageInfo.Source = "military_hat.png";
                    Button.Text = "Accedi";
                    Button.Clicked += OnSigniInButtonClicked!;
                    UsernameEntry.Completed += OnSigniInButtonClicked!;
                    PasswordEntry.Completed += OnSigniInButtonClicked!;
                    SwitchModeLabel.Text = "Se non hai ancora un account, Registrati";
                }
                else{
                    Header.Text = "Crea un nome utente e una password";
                    ImageInfo.Source = "soldier.png";
                    Button.Text = "Registrati";
                    await Task.Delay(100);
                    Button.Clicked += OnSigniUpButtonClicked!;
                    UsernameEntry.Completed += OnSigniUpButtonClicked!;
                    PasswordEntry.Completed += OnSigniUpButtonClicked!;

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
            
            try{
                var response =ClientGrpc.SignIn(username, password);
                if(response.Status == false){
                    ShowWarning(response.Message);
                    return;
                }
                

                await Navigation.PushAsync(new MainPage());
            }
            catch (Grpc.Core.RpcException) {
                ShowWarning("Non è stato possibile effettuare la registrazione per qualche problema nella rpc");
                
            }
            catch (Exception) {
                ShowWarning("Si è verificato un errore sconosciuto durante la registrazione");
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
                if(response.Status == false){
                    ShowWarning(response.Message);
                    return;
                }
                Console.WriteLine("SignUp effettuato con successo");
                await Navigation.PushAsync(new MainPage());
            }
            catch (Grpc.Core.RpcException) {
                ShowWarning("Non è stato possibile effettuare la registrazione per qualche problema nella rpc");
                
            }
            catch (Exception) {
                ShowWarning("Si è verificato un errore sconosciuto durante la registrazione");
            }
           

        }

        public async void OnUsernameUnFocused(object sender, EventArgs args){
            
            if(string.IsNullOrWhiteSpace(UsernameEntry.Text))
                return;

            try{
                var response = ClientGrpc.UsernameCheck(UsernameEntry.Text);
                if(response.Status == false){
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

        public async void OnUsernameFocused(object sender, EventArgs args){
            UsernameEntry.TextColor = Colors.White;
        }

        




    }

}