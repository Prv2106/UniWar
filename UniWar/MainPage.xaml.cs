using Microsoft.Maui.Controls;
using UniWar.views;

namespace UniWar
{
    /*
    * dichiara una classe pubblica MainPage che eredita da ContentPage. 
    * In .NET MAUI, ContentPage rappresenta una singola pagina con contenuti definiti tramite XAML o codice C#. 
    * partial indica che questa classe è "parziale", cioè ha una parte del codice scritta nel file .cs e un'altra parte nel file XAML collegato.
    */
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            /*
            * InitializeComponent() è un metodo generato automaticamente durante la compilazione che collega la logica C# agli elementi XAML definiti 
            * per questa pagina, consentendo di accedere a controlli e layout presenti nella MainPage.xaml.
            */
            InitializeComponent();
        }

        private async void OnNuovaPartitaButtonClicked(object sender, EventArgs e)
        {
            /*
            * await Navigation.PushAsync(new Integration()); avvia la navigazione verso una nuova pagina (Integration).
            * Navigation.PushAsync(new Integration()): Usa il metodo PushAsync della proprietà Navigation per navigare verso la pagina Integration e aggiungerla alla stack di navigazione.
            * await: È usato perché PushAsync è un metodo asincrono, per cui l’app attende fino a quando la navigazione non è completata prima di procedere. Questo garantisce che il flusso dell’interfaccia utente resti fluido.
            */
            await Navigation.PushAsync(new Integration());
        }
    }
}
