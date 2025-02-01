
namespace UniWar {
    public partial class InitializationSummary : ContentPage {
        public List<string> Territories { get; set; }
        public InitializationSummary() {
            InitializeComponent();

            Territories = new List<string>{
                "Alberta",
                "Congo",
                "Cina",
                "Brasile",
                "Argentina",
                "Perù",
                "Egitto"
            };

            // Impostiamo il BindingContext alla stessa pagina
            BindingContext = this;
        }

        
    }
}

