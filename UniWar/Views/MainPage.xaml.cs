namespace UniWar {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
            // metodo che, durante la compilazione, collega la logica C# agli elementi XAML 
        }

        private async void OnNuovaPartitaButtonClicked(object sender, EventArgs e) {
            // PushAsync è un metodo della proprietà Navigation che aggiunge una pagina allo stack e poi la raggiunge
            // N.B: await è usato perché PushAsync resituisce un Task, e quindi garantiamo la ripresa del controllo alla UI per 
            //      renderla responsiva.
            
           await Navigation.PushAsync(new InitializationSummary());           
            
        }

    } 
}