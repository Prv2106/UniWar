
namespace UniWar {
    public partial class InitializationSummary : ContentPage {
        /*
            Pagina dove viene mostrato il resoconto in seguito all'inizializzazione
            di una nuova partita.
        */
        public List<string> Territories { get; set; }
        public InitializationSummary() {
            InitializeComponent();

            Territories = new List<string> {
                "Alberta",
                "Congo",
                "Cina",
                "Brasile",
                "Argentina",
                "Perù",
                "Egitto"
            };

            // Impostiamo il BindingContext alla stessa pagina
            // serve per legare le collezioni allo xaml ed usare la CollectionView
            BindingContext = this;
        }

        public async void onBackButtonClicked(object o, EventArgs args) {
            await Navigation.PopAsync();
        } 

        public async void onConfirmButtonClicked(object o, EventArgs args) {
            await Navigation.PushAsync(new TablePage());
            Navigation.RemovePage(this);
        }

        
    }
}

