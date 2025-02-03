namespace UniWar {
    public partial class TablePage : ContentPage {
    /*
            Pagina dove viene mostrato il "tavolo" da gioco dal punto di vista dell'utente..
            Quindi, questo vede informazioni come:
            - il posizionamento delle sue armate sulla mappa,
            - il numero totale di carri armati,
            - il numero totale di territori posseduti,
            - il suo colore di carri armati,
            - il suo obiettivo,
    */

    // riceveremo dalla classe singleton due istanze di player, le quali avranno
    // il riferimento alla lista di territori != da null e dove ogni territorio ha 
    // a sua volta il riferimento ad una lista di carri armati

    public Player User {get; private set;}
    public Player CPU {get; private set;}
    public string IconSrcUser {get;}
    public string IconSrcCpu {get;}


    public TablePage(Player user, Player cpu) {
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
        InitializeComponent();
        User = user;
        // recuperiamo il nome del file dell'icona del carro armato utente
        IconSrcUser = User.Territories[0].Tanks[0].GetTankIconByColor();
        CPU = cpu;
        // recuperiamo il nome del file dell'icona del carro armato CPU
        IconSrcCpu = CPU.Territories[0].Tanks[0].GetTankIconByColor();

        // costruiamo la mappa distribuendo i carri armati sia per l'utente che per la CPU
        DeployTanks();

        // costruiamo la parte delle informazioni per l'utente sotto la mappa
        BuildUserInformation();

        // gestiamo la grafica sulla base del turno
        CheckIfIsUserTurn();
    }

        private async void CheckIfIsUserTurn() {
            if (User.Turn != null && User.Turn.Phase == TurnPhases.Attack) {
                // è il turno dell'utente, mostriamo una modal view dove gli comunichiamo che è il suo turno
                await Navigation.PushModalAsync(new NewUserTurn());

                // mostrimo il pulsante "attacca"
                AttackButton.IsVisible = true;
                // mostriamo il pulsante "passa"
                AttackButton.IsVisible = true;
            } else {
                // è il turno della CPU

            }
        }

        private void BuildUserInformation() {
            UserTankIcon.Source = IconSrcUser;
            NumTanks.Text = User.GetNumTanks().ToString();
            GoalDescr.Text = User.Goal?.Description;
        }

        private void DeployTanks() {
            foreach (Territory territory in User.Territories) {
                // prendiamoci la Grid corrispondente
                var territoryInMap = this.FindByName<Grid>(territory.Name);
                if (territoryInMap != null) {
                    foreach (var child in territoryInMap.Children) {
                        switch (child) {
                            case Image img:
                                img.Source = IconSrcUser;
                                break;
                            case Grid grid:
                                // la grid contenente 
                                foreach (var gridChild in grid.Children)
                                    if (gridChild is Label label)
                                        label.Text = territory.Tanks.Count.ToString();
                                break;
                            default: 
                                continue;
                        }
                    }
                }
            }

            foreach (Territory territory in CPU.Territories) {
                // prendiamoci la Grid corrispondente
                var territoryInMap = this.FindByName<Grid>(territory.Name);
                if (territoryInMap != null) {
                    foreach (var child in territoryInMap.Children) {
                        switch (child) {
                            case Image img:
                                img.Source = IconSrcCpu;
                                break;
                            case Grid grid:
                                // la grid contenente 
                                foreach (var gridChild in grid.Children)
                                    if (gridChild is Label label)
                                        label.Text = territory.Tanks.Count.ToString();
                                break;
                            default: 
                                continue;
                        }
                    }
                }
            }
        }

        private async void OnTerritoryClicked(object sender, EventArgs e) {
            var button = sender as Button;
            var territoryName = button?.ClassId;
            tooltipLabel.Text = territoryName;  // Testo del tooltip
            tooltipLabel.IsVisible = true;      // Mostra il tooltip
            await Task.Delay(2500);
            tooltipLabel.IsVisible = false;
    }
        private async void OnAttackButtonClicked(object sender, EventArgs e) {
            // Interagisci con la classe Singleton UniWarSystem per
            // implementare il caso d'uso di attacco

        }

        private async void OnPassButtonClicked(object sender, EventArgs e) {
            // Interagisci con la classe Singleton UniWarSystem 
            
        }
  }
    
}
