/*
In C#, la direttiva using System.Runtime.InteropServices; serve a importare il namespace System.Runtime.InteropServices, che contiene classi e metodi per interagire con il codice nativo (ad esempio, codice C o C++). 
Questo namespace è fondamentale quando si ha bisogno di fare interoperabilità tra il codice gestito (C#) e il codice non gestito (come quello C++).
*/
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.UI.Composition.Scenes;


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
    private Turn Turn {get; set;}
    private string IconSrcUser {get;} // percorso icona carro armato col colore 
    private string IconSrcCpu {get;}

    private bool UserWantsToAttack {get; set;} = false; // per gestire il click su un territorio

    private bool CpuTurnCompleted {get; set;} = false;
    private bool UserTurnCompleted {get; set;} = false;

    public static TablePage Instance {
        get {
            if (_instance == null) 
                _instance = new TablePage();
            return _instance;
        }
    }

    public void Reset() {
        _instance = null;
    }


    private TablePage() {
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
        InitializeComponent();
        Turn = UniWarSystem.Instance.Turn!;
        User = UniWarSystem.Instance.User!;
        // recuperiamo il nome del file dell'icona del carro armato utente
        IconSrcUser = User.Territories.Values.First().Tanks[0].GetTankIconByColor();
        CPU = UniWarSystem.Instance.Cpu!;
        // recuperiamo il nome del file dell'icona del carro armato CPU
        IconSrcCpu = CPU.Territories.Values.First().Tanks[0].GetTankIconByColor();

        BuildGraphics();
    }
    
    public async void OpenNewModal(ContentPage page) {
        await Task.Delay(300);
        await Navigation.PushModalAsync(page);
    }


    private void BuildGraphics() {
        // costruiamo la mappa distribuendo i carri armati sia per l'utente che per la CPU
        DeployTanks();

        // costruiamo la parte delle informazioni per l'utente sotto la mappa
        BuildUserInformation();

        // gestiamo i turni
        HandleTurns();
    }


    public async void HandleTurns() {
        /*
            Questo è metodo che gestisce i "pop-up" da mostrare
            sulla base delle fasi dei turni e regola gli elementi
            grafici di supporto ad ogni fase
        */

        if(CpuTurnCompleted && UserTurnCompleted){
            CpuTurnCompleted = false;
            UserTurnCompleted = false;
            Turn.IdRound++;
        }

        
      

        while(Turn.currentPlayer == CPU) { // è il turno della CPU
            switch (Turn.Phase) {
                case TurnPhases.Reinforcement:
                    await CpuReinforcement();   
                    break;
                case TurnPhases.Attack:
                    await CpuAttack();
                    break;
            }
        }

        if (Turn.currentPlayer == User) {
            // è il turno dell'utente
            switch (Turn.Phase) {
                case TurnPhases.Reinforcement:
                    // a livello di UI aggiungiamo il contatore dei numeri dei carri armati da poter aggiungere rimanenti
                    // l'utente, al click su un territorio suo, può aggiungere un carro armato a quel territorio..
                    // una volta terminati i carri armati da posizionare, però, finisce la fase di rinforzo
                    ShowOrHideReinforcementView();
                    await Navigation.PushModalAsync(new NewUserReinforcementTurn());
                    counterTankReinforcement.Text = (User.Territories.Count/2).ToString();
                    break;
                case TurnPhases.Attack:
                    ShowOrdHideAttackView();
                    await Navigation.PushModalAsync(new NewUserAttackTurn());
                    break;
                case TurnPhases.StrategicShift:
                    ShowOrdHideAttackView();
                    break;
            }
        } 

    }

    private void ShowOrHideReinforcementView() {
        if (!tankReinforcementView.IsVisible) {
            tankReinforcementView.IsVisible=true;
            tankReinforcementIcon.Source=IconSrcUser;
        } else 
            tankReinforcementView.IsVisible=false;
    }

    private void ShowOrdHideAttackView() {
        if (!AttackButton.IsVisible) {
           // mostriamo il pulsante "attacca"
            AttackButton.IsVisible = true;
            AttackButton.Text = "ATTACCA";
            // mostriamo il pulsante "passa"
            PassButton.IsVisible = true;
            PassButton.Text = "PASSA";
        } else {
            AttackButton.IsVisible = false;
            PassButton.IsVisible = false;
        }
    }



    public void BuildUserInformation() {
        UserTankIcon.Source = IconSrcUser;
        NumTanks.Text = User.GetNumTanks().ToString();
        GoalDescr.Text = User.Goal!.Description;
    }

    public void UpdateUserCounters() {
        NumTanks.Text = User.GetNumTanks().ToString();
        NumTerritories.Text = User.Territories.Count.ToString();
    }

    private void UpdateTankCounter(string territoryName) {
        Grid territoryInMap = this.FindByName<Grid>(territoryName);
            if (territoryInMap != null) {
                Grid counterGrid = (Grid) territoryInMap.Children[1];
                Label text = (Label) counterGrid.Children[1];
                text.Text=User.Territories[territoryName].Tanks.Count.ToString();
            }
    }

    public void DeployTanks() {
        foreach (Territory territory in User.Territories.Values) {
            // prendiamoci la Grid corrispondente
            var territoryInMap = this.FindByName<Grid>(territory.Name);
            if (territoryInMap != null) {
                foreach (var child in territoryInMap.Children) {
                    switch (child) {
                        case Image img: // sulla grid meno profonda si poggia quest'immagine
                            img.Source = IconSrcUser;
                            break;
                        case Grid grid: // griglia per il contatore (meno profonda della precedente)
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

    private async void ShowNameOfTerritory(string territoryName) {
        // mostriamo semplicemente il nome del territorio
        // perchè potrebbe essere nascosto dal carro armato
        tooltipLabel.Text = territoryName;  
        tooltipLabel.IsVisible = true;      
        await Task.Delay(2500);
        tooltipLabel.IsVisible = false; 
    }

    private async void OnTerritoryClicked(object sender, EventArgs e) {
        // recuperiamo il metadato (nome) associato al territorio cliccato
        var button = sender as Button;
        var territoryNameWithSpaces = button!.ClassId;
        string territoryName = territoryNameWithSpaces!.RemoveSpaces();
        // come prima cosa ne mostriamo il nome in una label apposita
        ShowNameOfTerritory(territoryNameWithSpaces);
            // dopodichè, SOLO se il territorio è dell'utente, facciamo diverse cose....
            if (User!.Territories.TryGetValue(territoryName, out Territory? from)) {
                // l'utente sta interagendo coi suoi territori
                int numTanksinTerritory = from.Tanks.Count;
                TaskCompletionSource<string> discoverUserChoose;
                switch (Turn.Phase) {
                    case TurnPhases.Reinforcement:
                        // se clicchiamo su un territorio in questa fase, vogliamo aggiungere un carro armato!
                        User.Territories[territoryName].AddTanks(User.TankColor, 1);
                        // aggiorniamo la mappa 
                        UpdateTankCounter(territoryName);
                        // aggiorniamo il contatore sulla destra
                        int oldValue = int.Parse(counterTankReinforcement.Text);
                        int newValue = --oldValue;
                        counterTankReinforcement.Text = newValue.ToString();
                        // aggiorniamo gli altri contatori
                        UpdateUserCounters();
                        if (newValue == 0){
                            // sono finiti i carri armati da posizionare
                            Turn.Phase = TurnPhases.Attack;
                            ShowOrHideReinforcementView();
                            HandleTurns();
                        }
                        break;

                    case TurnPhases.Attack when UserWantsToAttack:
                        PassButton.Text = "FINE";
                        // l'utente ha prima fatto "click" su ATTACCA
                        // allora l'utente può attaccare da questo territorio...
                        // 1. come prima cosa, dobbiamo verificare se il territorio di partenza ha almeno 2 carri armati 
                        if (numTanksinTerritory > 1) {
                            // Si può procedere con l'attacco
                            // ci serve l'elenco dei territori attaccabili sulla base dei territori confinanti!
                            List<string> attackableTerritories = UniWarSystem.Instance.AttackableTerritories(territoryName);
                            if (attackableTerritories.Count > 0) {
                                // mostriamo la modale dove l'utente clicca il territorio da attaccare
                                discoverUserChoose = new TaskCompletionSource<string>();
                                await Navigation.PushModalAsync(new SelectableTerritories(attackableTerritories, discoverUserChoose));
                                string attackedTerritoryChoosen = await discoverUserChoose.Task;
                                await Task.Delay(400); // aspettiamo che si chiude la modale
                                                       // invochiamo la funzione di attacco passando territorio partenza e destinazione
                                AttackTerritory(territoryName, attackedTerritoryChoosen);
                            }
                            else // l'utente deve selezionare un altro territorio
                                ShowInformation("I territori confinanti appartengono tutti a te, scegli un altro territorio..");
                        }
                        else
                            ShowInformation("Questo territorio ha solo 1 carro armato!");
                        break;

                    case TurnPhases.StrategicShift:
                        UserWantsToAttack = false;
                        // l'utente vuole spostare i carri armati DA questo territorio cliccato, quindi,
                        // come primissima cosa, dobbiamo capire se il territorio possiede più di un carro armato
                        if (numTanksinTerritory > 1) {
                            //  restituiamo una lista di territori confinanti che lui possiede
                            List<string> neighboringTerritories = UniWarSystem.Instance.UserNeighboringTerritories(territoryName);
                            if (neighboringTerritories.Count > 0) {
                                // mostriamo la modale dove l'utente clicca il territorio verso cui spostare i carri armati
                                discoverUserChoose = new TaskCompletionSource<string>();
                                await Navigation.PushModalAsync(new SelectableTerritories(neighboringTerritories, discoverUserChoose));
                                string selectedTerritory = await discoverUserChoose.Task;
                                await Task.Delay(400);
                                // apriamo la modale dove facciamo scegliere all'utente il numero di carri armati da spostare
                                var discoverTanksNumber = new TaskCompletionSource<int>();
                                await Navigation.PushModalAsync(new StrategicShiftModal(numTanksinTerritory - 1, discoverTanksNumber));
                                int tanksToMove = await discoverTanksNumber.Task;
                                await Task.Delay(400);
                                // adesso invochiamo la funzione che si occupa di effettuare le modifiche ai territori
                                MoveTanks(territoryName, selectedTerritory, tanksToMove);
                                // fine dello spostamento strategico
                                PassTurnToCpu();
                            }
                            else // i territori confinanti sono tutti nemici
                                ShowInformation("I territori confinanti non appartengono a te, scegli un altro territorio..");
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            { // l'utente ha selezionato un territorio avversario 
                TurnPhases phase = Turn.Phase;
                if (UserWantsToAttack || phase == TurnPhases.Reinforcement || phase == TurnPhases.StrategicShift)
                    ShowInformation("Devi selezionare un territorio che appartiene a te!");
            }
        }

    public async void OnEndGameClicked(object sender, EventArgs args) {
        TaskCompletionSource tcs;
        // verifichiamo che siano passati almeno 2 giri
        if(Turn.IdRound >= 3){
            // decretiamo il vincitore
            int userScore = 0;
            int cpuScore = 0;

            foreach (var t in User.Territories.Values)
                userScore += t.Score;
                
            foreach (var t in CPU.Territories.Values)
                cpuScore += t.Score;

            Console.WriteLine($"CpuScore = {cpuScore}, UserScore = {userScore}");

            if(userScore > cpuScore){
                tcs = new TaskCompletionSource();
                await Navigation.PushModalAsync(new WinOrLoseModal(true, tcs));
                await tcs.Task; // aspetta che facciamo setResult()
                await Task.Delay(400); // per dare il tempo alla modale di chiudersi
            }
            else if(userScore < cpuScore){
                 tcs = new TaskCompletionSource();
                await Navigation.PushModalAsync(new WinOrLoseModal(false, tcs));
                await tcs.Task; // aspetta che facciamo setResult()
                await Task.Delay(400); // per dare il tempo alla modale di chiudersi
            }
            else{
                tcs = new TaskCompletionSource();
                await Navigation.PushModalAsync(new WinOrLoseModal(true, tcs, true)); // Pareggio
                await tcs.Task; // aspetta che facciamo setResult()
                await Task.Delay(400); // per dare il tempo alla modale di chiudersi
            }
            await Navigation.PopToRootAsync();

            // rimandiamo alla modale
        } else {
            ShowInformation("Devono almeno passare 2 giri!");
        }
        
        
        
        
    }

        

    private async void ShowInformation(string text) {
        attackBanner.IsVisible = true;
        attackBanner.Text = text;
        await Task.Delay(4000);
        attackBanner.IsVisible = false;
    }


    private void OnAttackButtonClicked(object sender, EventArgs e) {
        if (!UserWantsToAttack) {
            // mostriamo un banner in cui informiamo l'utente di dove selezionare
            // un territorio dal quale effettuare l'attacco
            ShowInformation("Seleziona il territorio da cui vuoi effettuare l'attacco");  
            UserWantsToAttack = true;
            AttackButton.Text = "ANNULLA";      
        } else {                 
            UserWantsToAttack = false;
            ShowInformation("Ora, clickando su un territorio, ne vedrai soltanto il nome");
            AttackButton.Text = "ATTACCA";      
        }
    }

    // spostamento di un tot di carri amrati da un territorio all'altro
    public async void MoveTanks(string territoryFrom, string territoryTo, int numOfTanksToMove) {
        await Task.Delay(200);
        Territory from = User!.Territories[territoryFrom];
        Territory to = User!.Territories[territoryTo];
        from.RemoveTanks(numOfTanksToMove);
        to.AddTanks(User.TankColor, numOfTanksToMove);
        // aggiorniamo la UI (solo la mappa)
        DeployTanks();
    }

    // Attacco di un territorio da parte dell'utente
    public async void AttackTerritory(string attackingTerritory, string attackedTerritory) {
        // prepariamo le statistiche
        StatisticsCollection stats = new StatisticsCollection();
        stats.PlayerId = User.Name;
        stats.DefendingTerritories = new Dictionary<string, int>();  
        stats.AttackingTerritories = new Dictionary<string, int>();
        stats.LostTerritories = new List<string>();          
        stats.OwnedTerritories = new List<string>();          
        stats.UserTurn = true;
        stats.OwnedTanks = 0;
        stats.RoundId = Turn.IdRound;



        Territory from = User!.Territories[attackingTerritory];
        int numTanksAttacker = from.Tanks.Count;
        Territory to = CPU!.Territories[attackedTerritory];
        int numTanksDefender = to.Tanks.Count;
        List<int> userDice;
        List<int> cpuDice;
        // lanciamo i dadi
        RollTheDice(out userDice, out cpuDice, numTanksAttacker, numTanksDefender);
        string result = CompareDiceAndRemoveTanks(in userDice, in cpuDice, from, to);

        // riempiamo le statistiche nei dizionari
        stats.AttackingTerritories.Add(from.Name, numTanksAttacker - from.Tanks.Count);
        stats.DefendingTerritories.Add(to.Name, numTanksDefender - to.Tanks.Count);

        var userClosedTheModal = new TaskCompletionSource();
        await Navigation.PushModalAsync(new ShowDiceResultPage(userDice, cpuDice, result, userClosedTheModal));
        // aspettiamo che l'utente chiude la modale
        await userClosedTheModal.Task; // questo, nella modale, avviene prima della .Pop()
        await Task.Delay(100); // aspettiamo che si chiude la modale
        // Aggiorniamo la UI
        DeployTanks();
        UpdateUserCounters();
        // Dopo un attacco, verifichiamo se il territorio avversario è stato conquistato 
        if (to.Tanks.Count == 0) {
            // allora l'utente ha conquistato il territorio della cpu
            CPU!.Territories.Remove(attackedTerritory);
            User!.Territories.Add(attackedTerritory, to);

            // statistica sui territori persi dalla CPU
            stats.LostTerritories.Add(to.Name);

            // adesso l'utente deve spostare i carri armati
            var tcs = new TaskCompletionSource<int>();
            await Navigation.PushModalAsync(new ConqueredTerritoryModal(maxValue: from.Tanks.Count - 1, tcs));
            int numberSelected = await tcs.Task; // Aspetta il risultato
            await Task.Delay(100); // aspettiamo che si chiude la modale
            from.RemoveTanks(numberSelected);
            to.AddTanks(User!.TankColor, numberSelected);
            // Aggiorniamo nuovamente la UI
            DeployTanks();
            UpdateUserCounters();

            // aggiorniamo le statistiche
            foreach (var territory in User.Territories.Values) 
                stats.OwnedTerritories.Add(territory.Name);
                
            stats.OwnedTanks = User.GetNumTanks();
            
            // verifichiamo se con questo territorio in più l'utente ha vinto:
            if (IsWin()) {
                stats.UserWin = true;
                await CollectsStatistics(stats);
                TaskCompletionSource t;
                t = new TaskCompletionSource();
                await Navigation.PushModalAsync(new WinOrLoseModal(true, t));
                await t.Task; // aspetta che facciamo setResult()
                await Task.Delay(400); // per dare il tempo alla modale di chiudersi
                await Navigation.PopToRootAsync();
            }
            
        } else { // aggiorniamo le statistiche
            foreach (var territory in User.Territories.Values) 
                stats.OwnedTerritories.Add(territory.Name);
                
            stats.OwnedTanks = User.GetNumTanks();
        }


        await CollectsStatistics(stats);

    }

    
    private static void RollTheDice(out List<int> userDice, out List<int> cpuDice, in int numTanksAttacker, in int numTanksDefender) {
        userDice = [];
        cpuDice = [];
        // simuliamo il lancio dei dadi dell'attaccante: un dado per ogni carro armato - 1
        Random gen = new Random();
        int counter = 1;
        for (int i = 0; i < numTanksAttacker - 1; i++) {
            userDice.Add(gen.Next(6)+1); 
            if (counter == 3) break; // non si possono lanciare più di 3 dadi
            counter++;
        }
        // simuliamo il lancio dei dadi della difesa: un dado per ogni carro armato
        counter = 1;
        for (int i = 0; i < numTanksDefender; i++) {
            cpuDice.Add(gen.Next(6)+1); 
            if (counter == 3) break; // non si possono lanciare più di 3 dadi
            counter++;
        }
        // ordiniamo i dadi in modo decrescente:
        userDice.Sort((a,b) => b.CompareTo(a));
        cpuDice.Sort((a,b) => b.CompareTo(a));
    }

    private static string CompareDiceAndRemoveTanks(in List<int> userDice, in List<int> cpuDice, Territory attacking, Territory defending) {
        // confrontiamo le due liste (dadi) per un numero di volte pari alla lunghezza della lista più corta
        int counterForUser = 0;
        int counterForCpu = 0;
        for (int i = 0; i < Math.Min(userDice.Count, cpuDice.Count); i++) {
            if (userDice[i] <= cpuDice[i]) {
                // se è minore o pari, vince la difesa
                // rimuoviamo un carro armato da quel territorio posseduto dall'utente
                attacking.Tanks.RemoveAt(0);
                counterForUser++;
            } else {
                // confronto "vinto" dall'utente
                defending.Tanks.RemoveAt(0);
                counterForCpu++;
            }
        }
        return $"Hai perso {counterForUser} carri armati, mentre la CPU ne ha persi {counterForCpu}";
    }

    // Dopo che l'utente clicca il bottone "passa" il suo turno termina ed inizia quello della cpu
    // Questa funzione è responsabile dell'aggiornamento delle informazioni di TablePage in modo tale che vengano mandate a display le informazioni sulla cpu
    private async void OnPassButtonClicked(object sender, EventArgs e) {        
        await Task.Delay(500);
        // chiediamo all'utente, tramite una modale, se vuole effettuare uno spostamento strategico o meno
        var discoverUserChoose = new TaskCompletionSource<bool>();
        await Navigation.PushModalAsync(new YesOrNotModal(discoverUserChoose));
        bool choose = await discoverUserChoose.Task;
        await Task.Delay(400); // aspettiamo che si chiuda la modale
        if (choose == true) {
            // passiamo alla terza ed ultima fase del turno
            Turn.Phase=TurnPhases.StrategicShift;
            ShowInformation("Fai click sul territorio DA cui vuoi spostare i carri armati");
            HandleTurns(); 
        } else {
            // passiamo il turno alla CPU
            AttackButton.IsVisible = false;
            PassButton.IsVisible = false;
            UserWantsToAttack = false;
            PassTurnToCpu();
        }
    }

    private void PassTurnToCpu() {
        UserTurnCompleted = true;
        Turn.currentPlayer = CPU;
        Turn.Phase = TurnPhases.Reinforcement;
        HandleTurns();
    }

    private void PassTurnToUser() {
        CpuTurnCompleted = true;
        Turn.currentPlayer = User;
        Turn.Phase = TurnPhases.Reinforcement;
    }
        
        
    
    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr reinforcement (string jsonData, int newTanks);

    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr cpuAttack (string jsonData);

    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool winCheck (string jsonData);   

        private async Task CpuReinforcement(){
            TaskCompletionSource tcs;
            Console.WriteLine("TURNO DELLA CPU");
            // Creazione della mappa da passare a C++
            List<MapData> playersMaps = new List<MapData> {
                new MapData {
                    PlayerId = CPU.Name,
                    // Usiamo un Espressione LINQ per creare un nuovo dizionario nella forma Dictionary<string, List<string>> a partire dal dizionario Dictionary<string, Territory>
                    // In questo caso creiamo un nuovo dizionario in cui la chiave è il nome del territorio e in cui il valore è la lista dei nomi dei territori confinanti
                    Neighbors = CPU.Territories.ToDictionary(
                        // Chiave del nuovo dizionario
                        // sfrutta una lambda expression in questo caso prende t e restituisce t.key, cioè usiamo la sua chiave come chiave del nuovo dizionario (t è la coppia chiave-valore del vecchio dizionrio)
                        t => t.Key,
                        // Valore del nuovo dizionario
                        // Select(n => n.Name) estrae solamente il nome dei territori vicini (perché NeighboringTerritories è una lista di oggetti)
                        t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                    ),

                    Tanks = CPU.Territories.ToDictionary(
                        t => t.Key,
                        t => t.Value.Tanks.Count

                    )

                }
            };

            // Converte l'oggetto playersMaps (una lista di oggetti MapData) in una stringa JSON formattata
            // WriteIndented = true opzione che formatta il JSON con spazi e indentazione per renderlo più leggibile
            string jsonData = JsonSerializer.Serialize(playersMaps, new JsonSerializerOptions { WriteIndented = true });
            // Console.WriteLine("JSON inviato a C++:\n" + jsonData);

            // reinforcement restituisce un puntatore (char*) ma è un puntatore a memoria non gestita (cioè non gestita dal GC di .NET).
            // per questo usiamo IntPtr che rappresenta una struttura C# che viene utilizzata per memorizzare degli indirizzi di memoria (per la memoria non gestita).
            // su IntPtr possono essere utilizzati i metodi della classe Marshal di C#.

            // Strategia di rifornimento delle truppe:
            // Ad ogni nuovo turno i giocatori ricevono un numero di nuove truppe pari al numero di carri armati posseduti diviso 2 (arrotondato per difetto)
            int newTanks = CPU.Territories.Values.Count / 2;

            IntPtr resultPtr = reinforcement(jsonData, newTanks); //Stiamo passando il contesto del giocatore della cpu insieme ai nuovi carri che ha a disposizione

            // In questo caso, usiamo Marshal.PtrToStringUTF8(resultPtr) per copiare la stringa della memoria non gestita (C++) in una stringa gestita dal GC di C#
            // Marshal.PtrToStringUTF8(resultPtr) può restituire null, quindi usiamo ?? (operatore di null-coalescing) per far si che in tal caso a resultJson venga assegnata una stringa vuota anziché null
            string resultJson = Marshal.PtrToStringUTF8(resultPtr) ?? string.Empty;

            // Nota: non ci occupiamo di deallocare la memoria non gestita perché nella funzione C++ usiamo una stringa statica 
            // che quindi viene allocata nella memoria statica e persiste per tutta la durata del rpogramma (non abbiamo problemi di memory leak).

            // JsonSerializer.Deserialize<MapData>(resultJson) converte (deserializza) quella stringa JSON in una mappa (MapData)
            // updatedMaps diventa quindi MapData (che quindi può essere utilizzata per aggiornare le classi in C#).
            var updatedMap = JsonSerializer.Deserialize<MapData>(resultJson);
            
        
            // Scorriamo i territori della CPU e de il numero di carri armati del territorio è minore di quello presente in updatedMap aggiungiamo alla lista di carri armati tanti carri armati quanti ne mancano
            foreach(var territory in updatedMap.Tanks){
                if(CPU.Territories.ContainsKey(territory.Key)){
                    // recuperiamo il territorio della cpu
                    var cpuTerritory = CPU.Territories[territory.Key];
                    int difference = territory.Value - cpuTerritory.Tanks.Count;
                    if(difference > 0)
                        cpuTerritory.AddTanks(CPU.TankColor, difference);
                    
                }                        
            }

            DeployTanks();
            tcs = new TaskCompletionSource();
            await Navigation.PushModalAsync(new GenericModal("La CPU si è rinforzata! ","Reinforcement",tcs,newTanks));
            await tcs.Task; // aspetta che facciamo setResult()
            await Task.Delay(400); // per dare il tempo alla modale di chiudersi
                                

            Turn.Phase = TurnPhases.Attack;
        }



        private async Task CpuAttack(){
                    TaskCompletionSource tcs;

                    List<MapData> playersMaps = new List<MapData>(){
                        new MapData {
                            PlayerId = CPU.Name,
                            Neighbors = CPU.Territories.ToDictionary(
                                t => t.Key,
                                t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                            ),
                            Tanks= CPU.Territories.ToDictionary(
                                t => t.Key,
                                t => t.Value.Tanks.Count
                            )

                        },

                        new MapData{
                            PlayerId = User.Name,
                            Neighbors = User.Territories.ToDictionary(
                                t => t.Key,
                                t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                            ),
                            Tanks= User.Territories.ToDictionary(
                                t => t.Key,
                                t => t.Value.Tanks.Count
                            )

                        }            

                    };


                    string jsonData = JsonSerializer.Serialize(playersMaps, new JsonSerializerOptions{WriteIndented = true});
                    // Console.WriteLine("JSON inviato a C++:\n" + jsonData);
                    IntPtr resultPtr = cpuAttack(jsonData);
                    string? resultJson = Marshal.PtrToStringUTF8(resultPtr);

                    if((resultJson == string.Empty) || resultJson == "[]"){
                        Console.WriteLine("La CPU ha deciso di non attaccare");
                        tcs = new TaskCompletionSource();
                        await Navigation.PushModalAsync(new GenericModal("La CPU ha deciso di non attaccare", "NoAttack",tcs));
                        await tcs.Task; // aspetta che facciamo setResult()
                        await Task.Delay(400); // per dare il tempo alla modale di chiudersi 
                    }
                    else{
                        List<BattleResult>? battleResults = JsonSerializer.Deserialize<List<BattleResult>>(resultJson!);
                        // Per il debug
                        // Console.WriteLine("JSON aggiornato:\n" + JsonSerializer.Serialize(battleResults, new JsonSerializerOptions { WriteIndented = true }));
                        if(battleResults is not null){
                            await SimulateBattle(battleResults); 
                        }
                       
                    }


                    PassTurnToUser();
         }    




        bool IsWin(){
            List<MapData> playersMaps = new List<MapData>(){
                        new MapData {
                            PlayerId = User.Name,
                            Neighbors = User.Territories.ToDictionary(
                                t => t.Key,
                                t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                            ),
                            Tanks= User.Territories.ToDictionary(
                                t => t.Key,
                                t => t.Value.Tanks.Count
                            )
                        }
            };
        

            string jsonData = JsonSerializer.Serialize(playersMaps, new JsonSerializerOptions{WriteIndented = true});

            return winCheck(jsonData);
        
        }

        private async Task SimulateBattle(List<BattleResult> battleList) {
            TaskCompletionSource tcs;
            Territory? lostTerritory = null;

            StatisticsCollection stats = new StatisticsCollection();
            stats.PlayerId = User.Name;
            stats.DefendingTerritories = new Dictionary<string, int>();  
            stats.AttackingTerritories = new Dictionary<string, int>();
            stats.LostTerritories = new List<string>();          
            stats.OwnedTerritories = new List<string>();          
            stats.UserTurn = false;
            stats.RoundId = Turn.IdRound;

                        
            foreach(var battle in battleList){
                 bool territoryLoss = false;
                
                foreach(var territory in User.Territories.Values){
                    if(battle.DefendingTanksCountMap.TryGetValue(territory.Name, out int value)) {
                        int difference = value - territory.Tanks.Count;
                        if(difference < 0){
                            territory.RemoveTanks(-difference);
                        }
                    }
                    else{
                        int difference = battle.AttackingTanksCountMap[territory.Name] - territory.Tanks.Count;
                        if (difference > 0){
                            territory.AddTanks(User.TankColor,difference);
                        }
                        else if(difference < 0){
                            territory.RemoveTanks(-difference);
                        }

                        CPU.AddTerritory(territory);
                        User.RemoveTerritory(territory);
                        lostTerritory = territory;
                        territoryLoss = true;
                        stats.LostTerritories.Add(lostTerritory.Name);
                    }
                }

                foreach(var territory in CPU.Territories.Values){
                    if(battle.AttackingTanksCountMap.TryGetValue(territory.Name,out int value)){
                        int difference = value - territory.Tanks.Count;
                        if(difference < 0){
                            territory.RemoveTanks(-difference);
                        }
                    }
                }

                if (stats.DefendingTerritories.ContainsKey(battle.DefendingTerritory)) {
                    stats.DefendingTerritories[battle.DefendingTerritory] += battle.LossesPlayer;
                }
                else {
                    stats.DefendingTerritories.Add(battle.DefendingTerritory, battle.LossesPlayer);
                }

                if (stats.AttackingTerritories.ContainsKey(battle.AttackingTerritory)) {
                    stats.AttackingTerritories[battle.AttackingTerritory] += battle.LossesCPU;
                }
                else {
                    stats.AttackingTerritories.Add(battle.AttackingTerritory, battle.LossesCPU);
                }

                foreach(var territory in CPU.Territories.Values)
                    stats.OwnedTerritories.Add(territory.Name);

                stats.OwnedTanks = CPU.GetNumTanks();

                tcs = new TaskCompletionSource();
                await Navigation.PushModalAsync(new ShowCpuBattleTerritory(battle.AttackingTerritory, battle.DefendingTerritory, tcs));
                await tcs.Task; // aspetta che facciamo setResult()
                await Task.Delay(400); // per dare il tempo alla modale di chiudersi

                tcs = new TaskCompletionSource();
                // Mostriamo il risultato del lancio dei dadi
                await Navigation.PushModalAsync(new ShowCpuDiceResultPage(battle.DicePlayer,battle.DiceCPU,battle.LossesCPU, battle.LossesPlayer, tcs));
                await tcs.Task; // aspetta che facciamo setResult()
                await Task.Delay(400); // per dare il tempo alla modale di chiudersi

                if(territoryLoss){
                    tcs = new TaskCompletionSource();
                    await Navigation.PushModalAsync(new ShowDefenceResult(tcs, lostTerritory!.Name));
                    await tcs.Task; // aspetta che facciamo setResult()
                    await Task.Delay(400); // per dare il tempo alla modale di chiudersi
                }
                    
                // Aggiorniamo la mappa
                DeployTanks();
                BuildUserInformation();

                if(battle.Win){     
                    stats.UserWin = false;
                    await CollectsStatistics(stats);
                    tcs = new TaskCompletionSource();
                    await Navigation.PushModalAsync(new WinOrLoseModal(false, tcs));
                    await tcs.Task; // aspetta che facciamo setResult()
                    await Task.Delay(400); // per dare il tempo alla modale di chiudersi
                    await Navigation.PopToRootAsync();
                    break;
                }
            }

            await CollectsStatistics(stats);
        }

        private async Task CollectsStatistics(StatisticsCollection statistics) {
            if (!UniWarSystem.Instance.IsOffline) {
                try {
                ClientGrpc.SendStatisticsAsync(statistics);
                } catch (Grpc.Core.RpcException e) {
                ShowInformation($"Non è stato possibile aggiornare le statistiche per questo round a causa di un errore nella chiamata rpc");
                Console.WriteLine($"Errore: {e}");
                await Task.Delay(1500);
                } catch (Exception) {
                ShowInformation("Si è verificato un errore sconosciuto nell'invio delle statistiche.");
                await Task.Delay(1500);
                }
            }   
        }
    }

}



