
namespace UniWar {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
            // metodo che, durante la compilazione, collega la logica C# agli elementi XAML 
        }

        private async void OnNuovaPartitaButtonClicked(object sender, EventArgs e) {
            // PushAsync: metodo della proprietà Navigation per navigare verso la pagina Integration e aggiungerla alla stack di navigazione.
            // N.B: await è usato perché PushAsync è un metodo asincrono.
            await Navigation.PushAsync(new InitializationSummary());

            // Rimuove la pagina corrente dallo stack (quindi la freccia "Indietro" non verrà mostrata)
        }
    }
}
