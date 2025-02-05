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

    private static TablePage? _instance;
    private Player User {get; set;}
    private Player CPU {get; set;}
    private string IconSrcUser {get;}
    private string IconSrcCpu {get;}
    private bool UserWantsToAttack {get; set;} = false;

    public static TablePage Instance {
        get {
            if (_instance == null) 
                _instance = new TablePage();
            return _instance;
        }
    }


    private TablePage() {
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
        InitializeComponent();
        User = UniWarSystem.Instance.User!;
        // recuperiamo il nome del file dell'icona del carro armato utente
        IconSrcUser = User.Territories.Values.First().Tanks[0].GetTankIconByColor();
        CPU = UniWarSystem.Instance.Cpu!;
        // recuperiamo il nome del file dell'icona del carro armato CPU
        IconSrcCpu = CPU.Territories.Values.First().Tanks[0].GetTankIconByColor();

        BuildGraphics();
    }
    


    private void BuildGraphics() {
        // costruiamo la mappa distribuendo i carri armati sia per l'utente che per la CPU
        DeployTanks();

        // costruiamo la parte delle informazioni per l'utente sotto la mappa
        BuildUserInformation();

        // pop-up da mostrare quando il turno passa dalla CPU all'utente
        CheckIfIsUserTurn();
    }

    private async void CheckIfIsUserTurn() {
        await Task.Delay(1500);
        if (User.Turn != null && User.Turn.Phase == TurnPhases.Attack) {
            // è il turno dell'utente, mostriamo una modal view dove gli comunichiamo che è il suo turno
            await Navigation.PushModalAsync(new NewUserTurn());

            // mostrimo il pulsante "attacca"
            AttackButton.IsVisible = true;
            // mostriamo il pulsante "passa"
            PassButton.IsVisible = true;
        } else {
            // è il turno della CPU

        }
    }

    public void BuildUserInformation() {
        UserTankIcon.Source = IconSrcUser;
        NumTanks.Text = User.GetNumTanks().ToString();
        GoalDescr.Text = User.Goal!.Description;
    }

    public void DeployTanks() {
        // nella distribuzione dei carri armati degli utenti, aggiungiamo un metadato 
        // che ci permette di capire quali sono i territori selezionabili per l'attacco!
        foreach (Territory territory in User.Territories.Values) {
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
                        case Button btn:
                            btn.CommandParameter = "user"; // sono quelli dell'utente
                            break;
                        default: 
                            continue;
                    }
                }
            } else {
                Console.WriteLine("problema con: " + territory.Name);
            }
        }

        foreach (Territory territory in CPU.Territories.Values) {
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
            } else {
                Console.WriteLine("problema con: " + territory.Name);
            }
        }
    }

        private async void OnTerritoryClicked(object sender, EventArgs e) {
            // recuperiamo il metadato (nome) associato al territorio cliccato
            var button = sender as Button;
            var territoryName = button?.ClassId;
            if (territoryName != null) {
                if (UserWantsToAttack) { // se è il turno di attacco dell'utente
                    // come prima cosa dobbiamo capire se il territorio cliccato è posseduto dall'utente
                    if (button?.CommandParameter != null && button?.CommandParameter.ToString() == "user") {
                        // allora l'utente può attaccare da questo territorio
                        // invochiamo l'operazione di sistema che restituisce
                        // l'elenco dei territori attaccabili sulla base dei territori confinanti!
                        List<string> neighboringTerritories = UniWarSystem.Instance.AttackableTerritories(territoryName.RemoveSpaces());
                        if (neighboringTerritories.Count > 0) {
                            // mostriamo la modale dove l'utente clicca il territorio da attaccare
                            await Navigation.PushModalAsync(new AttackableTerritoriesPage(neighboringTerritories, territoryName));
                        } else {// l'utente deve selezionare un altro territorio
                            ShowInformation("I territori confinanti appartengono tutti a te, scegli un altro territorio..");
                        }
                    } else {
                        // l'utente ha selezionato il territorio avversario da cui non può attaccare
                        ShowInformation("Devi selezionare un territorio che appartiene a te!");
                    }
                } else { // l'utente non ha detto di voler attaccare
                    // mostriamo semplicemente il nome del territorio
                    // perchè potrebbe essere nascosto dal carro armato
                    tooltipLabel.Text = territoryName;  
                    tooltipLabel.IsVisible = true;      
                    await Task.Delay(2500);
                    tooltipLabel.IsVisible = false; 
                }
            }
        }
        

        private async void ShowInformation(string text) {
            attackBanner.IsVisible = true;
            attackBanner.Text = text;
            await Task.Delay(3500);
            attackBanner.IsVisible = false;
        }

        private void OnAttackButtonClicked(object sender, EventArgs e) {
            if (!UserWantsToAttack) {
                // mostriamo un banner in cui informiamo l'utente di dove selezionare
                // un territorio dal quale effettuare l'attacco
                ShowInformation("Seleziona il territorio da cui vuoi effettuare l'attacco");  
                UserWantsToAttack = true;
                AttackButton.Text = "ANNULLA ATTACCO";      
            } else {                 
                UserWantsToAttack = false;
                ShowInformation("Ora, clickando su un territorio, ne vedrai soltanto il nome");
                AttackButton.Text = "ATTACCA";      
            }
        }

        private void OnPassButtonClicked(object sender, EventArgs e) {
            // Interagisci con la classe Singleton UniWarSystem 
            
        }
  }
    
}
