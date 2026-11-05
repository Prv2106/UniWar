namespace UniWar {
    /// <summary>
    /// Modal that lets the user choose among a list of territories.
    /// </summary>
    public partial class SelectableTerritories : ContentPage {
        public List<string> SelectableTerritoriesNames {get;} = [];

        private TaskCompletionSource<string> _territoryChoosen;
        
        /// <summary>
        /// Initializes the modal with the selectable territories.
        /// </summary>
        /// <param name="neighboringTerritories">Territory names to display.</param>
        /// <param name="taskCompletionSource">Source completed with the choice.</param>
        /// <param name="isAttack">Whether the selection is for an attack.</param>
        public SelectableTerritories(List<string> neighboringTerritories, TaskCompletionSource<string> taskCompletionSource, bool isAttack) {
            // Territory names are stored without spaces so they can be used as
            // dictionary keys.
            InitializeComponent();
            foreach (var territory in neighboringTerritories) 
                SelectableTerritoriesNames.Add(territory.AddSpaces());

            BindingContext = this;  // Allows CollectionView to access the properties

            _territoryChoosen = taskCompletionSource;

            if (isAttack) {
                // The user is selecting an enemy territory to attack.
                image.Source = "soldier.png";
                text.Text = "Seleziona il territorio nemico da attaccare!";
            } else {
                // The user is selecting a destination for a strategic shift.
                image.Source = "running_soldiers.png";
                text.Text = "Seleziona uno dei tuoi territori confinanti dove vuoi spostare qualche carro armato!";
            }
        }

        
        /// <summary>
        /// Completes the selection with the clicked territory.
        /// </summary>
        private async void OnTerritoryClicked (object sender, EventArgs eventArgs) {
            var button = sender as Button;
            string attackedTerritory = button!.CommandParameter.ToString()!.RemoveSpaces();
            _territoryChoosen.SetResult(attackedTerritory);
            await Navigation.PopModalAsync();
        }

        /// <summary>
        /// Closes the modal without selecting a territory.
        /// </summary>
        private async void OnCancelButtonClicked(object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
    }
}