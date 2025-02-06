/*
In C#, la direttiva using System.Runtime.InteropServices; serve a importare il namespace System.Runtime.InteropServices, che contiene classi e metodi per interagire con il codice nativo (ad esempio, codice C o C++). 
Questo namespace è fondamentale quando si ha bisogno di fare interoperabilità tra il codice gestito (C#) e il codice non gestito (come quello C++).
*/
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;


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
    private string IconSrcUser {get;} // percorso icona carro armato col colore 
    private string IconSrcCpu {get;}
    private bool UserWantsToAttack {get; set;} = false; // per gestire il click su un territorio

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
    
    public async void OpenNewModal(ContentPage page) {
        await Task.Delay(500);
        await Navigation.PushModalAsync(page);
    }


    private void BuildGraphics() {
        // costruiamo la mappa distribuendo i carri armati sia per l'utente che per la CPU
        DeployTanks();

        // costruiamo la parte delle informazioni per l'utente sotto la mappa
        BuildUserInformation();

        // pop-up da mostrare quando il turno passa dalla CPU all'utente
        HandleTurns();
    }




    public async void HandleTurns() {
        await Task.Delay(1000);
        while(CPU.Turn != null) { // è il turno della CPU
            switch (CPU.Turn.Phase) {
                case TurnPhases.Reinforcement:
                    CpuReinforcement();   
                    break;
                case TurnPhases.Attack:
                    CpuAttack();
                    break;
            }
        }

        if (User.Turn != null) {
            // è il turno dell'utente
            switch (User.Turn.Phase) {
                case TurnPhases.Reinforcement:
                    // a livello di UI aggiungiamo il contatore dei numeri dei carri armati da poter aggiungere rimanenti
                    //TODO:
                    // l'utente, al click su un territorio suo, può aggiungere un carro armato a quel territorio..
                    // una volta terminati i carri armati da posizionare, però, finisce la fase di rinforzo
                    int tanksToAdd = User.Territories.Count / 2;
                    User.Turn!.NumTanksToAddInReinforcementPhase = tanksToAdd;
                    await Navigation.PushModalAsync(new NewUserReinforcementTurn(tanksToAdd));
                    break;
                case TurnPhases.Attack:
                    // 1. adattiamo la UI
                    // mostriamo il pulsante "attacca"
                    AttackButton.IsVisible = true;
                    // mostriamo il pulsante "passa"
                    PassButton.IsVisible = true;
                    PassButton.Text = "PASSA";
                    break;

                case TurnPhases.StrategicShift:
                    // TODO:
                    break;
            }
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
            var territoryInMap = this.FindByName<Grid>(territory.Name); // Grid meno profonda nello schermo 
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
                        case Button btn:
                            btn.CommandParameter = "user"; // CommandParameter è un metadato per distinguere i territori dell'utente 
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
            var territoryNameWithSpaces = button?.ClassId;
            string territoryName = territoryNameWithSpaces!.RemoveSpaces();
            if (territoryName != null) {
                switch (User.Turn!.Phase) {
                    case TurnPhases.Reinforcement when button!.CommandParameter.ToString() == "user":
                        // se clicchiamo su un territorio in questa fase, vogliamo aggiungere un carro armato!
                        if (User.Turn!.NumTanksToAddInReinforcementPhase > 0) {
                            User.Territories[territoryName].AddTanks(User.TankColor, 1);
                            User.Turn!.NumTanksToAddInReinforcementPhase--;
                        } else {
                            // sono finiti i carri armati da posizionare
                            User.Turn!.Phase = TurnPhases.Attack;
                            HandleTurns();
                        }
                        
                        break;
                    case TurnPhases.Attack when button!.CommandParameter.ToString() == "user":
                        if (UserWantsToAttack) { // l'utente ha prima fatto "click" su ATTACCA
                            // allora l'utente può attaccare da questo territorio
                            // ci serve l'elenco dei territori attaccabili sulla base dei territori confinanti!
                            List<string> neighboringTerritories = UniWarSystem.Instance.AttackableTerritories(territoryName);
                            if (neighboringTerritories.Count > 0) 
                                // mostriamo la modale dove l'utente clicca il territorio da attaccare
                                await Navigation.PushModalAsync(new AttackableTerritoriesPage(neighboringTerritories, territoryName));
                            else // l'utente deve selezionare un altro territorio
                                ShowInformation("I territori confinanti appartengono tutti a te, scegli un altro territorio..");
                        } else { 
                                // l'utente non ha ancora indicato esplicitamente di voler attaccare ...
                                // mostriamo semplicemente il nome del territorio
                                // perchè potrebbe essere nascosto dal carro armato
                                tooltipLabel.Text = territoryName;  
                                tooltipLabel.IsVisible = true;      
                                await Task.Delay(2500);
                                tooltipLabel.IsVisible = false; 
                        }
                        break;
                    case TurnPhases.StrategicShift:
                        break;
                    default:
                        // l'utente ha selezionato un territorio avversario 
                        ShowInformation("Devi selezionare un territorio che appartiene a te!");
                        break;
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
                AttackButton.Text = "ANNULLA";      
            } else {                 
                UserWantsToAttack = false;
                ShowInformation("Ora, clickando su un territorio, ne vedrai soltanto il nome");
                AttackButton.Text = "ATTACCA";      
            }
        }
        
    
    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr reinforcement (string jsonData, int newTanks);

    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr cpuAttack (string jsonData);

    
    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool winCheck (string jsonData);



      

        // Dopo che l'utente clicca il bottone "passa" il suo turno termina ed inizia quello della cpu
        // Questa funzione è responsabile dell'aggiornamento delle informazioni di TablePage in modo tale che vengano mandate a display le informazioni sulla cpu
        private async void OnPassButtonClicked(object sender, EventArgs e) {
            // Fine del turno dell'utente
            User.Turn = null;

            AttackButton.IsVisible = false;
            PassButton.IsVisible = false;

            await Task.Delay(500);

            CPU.Turn = new Turn(TurnPhases.Reinforcement);

            HandleTurns(); // Adesso è il turno della CPU
        }
        
    

        void CpuReinforcement(){
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
            Console.WriteLine("JSON inviato a C++:\n" + jsonData);

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

            CPU.Turn!.Phase = TurnPhases.Attack;
        }





        private void CpuAttack(){
        
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
                    Console.WriteLine("JSON inviato a C++:\n" + jsonData);
                    IntPtr resultPtr = cpuAttack(jsonData);
                    string? resultJson = Marshal.PtrToStringUTF8(resultPtr);

                    if((resultJson == string.Empty) || resultJson == "[]"){
                        Console.WriteLine("La CPU ha deciso di non attaccare");
                        OpenNewModal(new GenericModal("La CPU ha deciso di non attaccare"));
                    }
                    else{
                        List<BattleResult>? battleResults = JsonSerializer.Deserialize<List<BattleResult>>(resultJson!);
                        // Per il debug
                        Console.WriteLine("JSON aggiornato:\n" + JsonSerializer.Serialize(battleResults, new JsonSerializerOptions { WriteIndented = true }));
                        if(battleResults is not null){
                            SimulateBattle(battleResults); 
                        }
                       
                    }

                    // CPU passa il turno
                    CPU.Turn = null;
                    User.Turn = new Turn(TurnPhases.Attack);
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




        private void SimulateBattle(List<BattleResult> battleList){
            
            foreach( var battle in battleList){
                /*
                    Per la UI serve:
                    - territorio da cui la CPU sta attaccando
                    - territorio dell'user attaccato
                    - dado utente
                    - dado cpu
                    - num carri armati persi dall'utente
                    - num carri armati persi dalla cpu 
                */


                // TODO: 
                // verificare per ogni territorio di User:
                //  - se possiede ancora quel territorio e in caso affermativo se il numero di carri armati di tale territorio è rimasto o meno invariato

                foreach(var territory in User.Territories.Values){
                    if(battle.DefendingTanksCountMap.ContainsKey(territory.Name)){
                        int difference = battle.DefendingTanksCountMap[territory.Name] - territory.Tanks.Count;
                        if(difference > 0){
                            territory.AddTanks(User.TankColor, difference);
                        }
                        else if(difference < 0){
                            // TODO: Implementare metodo di Territory removeTanks(int num = 1)
                        }
                        /*
                            // Scorriamo i territori della CPU e de il numero di carri armati del territorio è minore di quello presente in updatedMap aggiungiamo alla lista di carri armati tanti carri armati quanti ne mancano
                            foreach(var territory in updatedMap.Tanks){
                                if(CPU.Territories.ContainsKey(territory.Key)){
                                    // recuperiamo il territorio della cpu
                                    var cpuTerritory = CPU.Territories[territory.Key];
                                    int difference = territory.Value - cpuTerritory.Tanks.Count;
                                    if(difference > 0)
                                cpuTerritory.AddTanks(CPU.TankColor, difference);
                        
                        
                        */

                    }
                    else{
                        User.RemoveTerritory(territory);
                    }
                }

                // verificare per ogni territorio di CPU
                // - se possiede lo stesso numero di carri armati di prima oppure no


                

            }
        }



        



    }
}



