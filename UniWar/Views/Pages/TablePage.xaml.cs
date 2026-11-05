/*
The System.Runtime.InteropServices namespace provides types and methods for
interoperating with native code (for example, C or C++). It is required for the
managed-to-native calls used by this page.
*/
using System.Runtime.InteropServices;
using System.Text.Json;

namespace UniWar {
    /// <summary>
    /// Main game table. Displays the map, the user's army placement, total tank
    /// and territory counts, the user's tank color, and the assigned goal.
    /// </summary>
    public partial class TablePage : ContentPage {
    /*
            This page renders the game board from the user's perspective:
            - army placement on the map;
            - total tank count;
            - total owned territories;
            - tank color;
            - assigned goal.
    */

    // The singleton system provides the player instances, each referencing a
    // non-null territory collection whose items reference tank lists.

    private static TablePage? _instance;
    private Player? User {get; set;}
    private Player? CPU {get; set;}
    private Turn? Turn {get; set;}
    private string? IconSrcUser {get;}  // Colored tank icon path
    private string? IconSrcCpu {get;}

    private bool UserWantsToAttack {get; set;} = false;  // Tracks territory clicks during attack mode

    private bool CpuTurnCompleted {get; set;} = false;
    private bool UserTurnCompleted {get; set;} = false;
    private bool UserAttack {get; set;} = false;

    private readonly JsonSerializerOptions jsonSerializerOpt = new() { WriteIndented = true};

    /// <summary>
    /// Gets the singleton page instance, creating it on first access.
    /// </summary>
    public static TablePage Instance {
        get {
            if (_instance == null) 
                _instance = new TablePage();
            return _instance;
        }
    }

    /// <summary>
    /// Discards the singleton instance so a new page can be created.
    /// </summary>
    public static void Reset() {
        _instance = null;
    }


    private TablePage() {
        try {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior{IsVisible=false});
            InitializeComponent();
            Turn = UniWarSystem.Instance.Turn!;
            User = UniWarSystem.Instance.User!;
            // Resolve the user tank icon file name.
            IconSrcUser = User.Territories.Values.First().Tanks[0].GetTankIconByColor();
            CPU = UniWarSystem.Instance.Cpu!;
            // Resolve the CPU tank icon file name.
            IconSrcCpu = CPU.Territories.Values.First().Tanks[0].GetTankIconByColor();

            BuildGraphics();
        } catch (Exception e) {
            ShowInformation("Errore: " + e.Message);
        }
    }
    

    /// <summary>
    /// Deploys tanks, builds the user information panel, and starts turn
    /// handling.
    /// </summary>
    private void BuildGraphics() {
        // Build the map and place the tanks for both players.
        DeployTanks();

        // Build the information panel below the map.
        BuildUserInformation();

        // Start turn handling.
        HandleTurns();
    }

    /// <summary>
    /// Increments the round counter when both players complete their turns.
    /// </summary>
     private void CheckIfRoundIsCompleted() {
        if (CpuTurnCompleted && UserTurnCompleted) {
                CpuTurnCompleted = false;
                UserTurnCompleted = false;
                Turn!.IdRound++;
                RoundCounter.Text = Turn!.IdRound.ToString();
            }
    }

    /// <summary>
    /// Drives the turn state machine and shows the modal dialogs associated
    /// with each phase.
    /// </summary>
    public async void HandleTurns() {
        try {
            CheckIfRoundIsCompleted();            
            while (Turn!.CurrentPlayer == CPU) {  // CPU turn
                statisticsButton.IsVisible = false;
                endGameButton.IsVisible = false; 
                switch (Turn.Phase) {
                    case TurnPhases.Reinforcement:
                        await CpuReinforcement();   
                        break;
                    case TurnPhases.Attack:
                        await CpuAttack();
                        break;
                }
            }

            CheckIfRoundIsCompleted();

            if (Turn.CurrentPlayer == User) {
                // User turn.
                UserAttack = false;

                // Update button visibility.
                if (!UniWarSystem.Instance.IsOffline) 
                    statisticsButton.IsVisible = true;
                endGameButton.IsVisible = true;
                
                switch (Turn.Phase) {
                    case TurnPhases.Reinforcement:
                        // Show the remaining reinforcement counter. Clicking one
                        // of the user's territories adds one tank; the phase
                        // ends when no tanks remain.
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
        } catch (Exception e) {
            ShowInformation("Errore: " + e.Message);
        }


    }

    /// <summary>
    /// Toggles the reinforcement counter panel.
    /// </summary>
    private void ShowOrHideReinforcementView() {
        if (!tankReinforcementView.IsVisible) {
            tankReinforcementView.IsVisible=true;
            tankReinforcementIcon.Source=IconSrcUser;
        } else 
            tankReinforcementView.IsVisible=false;
    }

    /// <summary>
    /// Toggles the attack and pass buttons.
    /// </summary>
    private void ShowOrdHideAttackView() {
        if (!AttackButton.IsVisible) {
            AttackButton.IsVisible = true;
            AttackButton.Text = "ATTACCA";
            PassButton.IsVisible = true;
            PassButton.Text = "PASSA";
        } else {
            AttackButton.IsVisible = false;
            PassButton.IsVisible = false;
        }
    }


    /// <summary>
    /// Populates the user information panel.
    /// </summary>
    private void BuildUserInformation() {
        UserTankIcon.Source = IconSrcUser;
        NumTanks.Text = User!.GetNumTanks.ToString();
        GoalDescr.Text = User.Goal!.Description;
        NumTerritories.Text = User.Territories.Count.ToString();
    }

    /// <summary>
    /// Refreshes the user tank and territory counters.
    /// </summary>
    private void UpdateUserCounters() {
        NumTanks.Text = User!.GetNumTanks.ToString();
        NumTerritories.Text = User.Territories.Count.ToString();
    }


    /// <summary>
    /// Updates the on-map tank counter for a territory.
    /// </summary>
    /// <param name="territoryName">Territory name.</param>
    private void UpdateTankCounter(string territoryName) {
        Grid territoryInMap = this.FindByName<Grid>(territoryName);
            if (territoryInMap != null) {
                Grid counterGrid = (Grid) territoryInMap.Children[1];
                Label text = (Label) counterGrid.Children[1];
                text.Text=User!.Territories[territoryName].Tanks.Count.ToString();
            }
    }

    /// <summary>
    /// Renders both players' tanks and counters on the map.
    /// </summary>
    public void DeployTanks() {
        foreach (Territory territory in User!.Territories.Values) {
            // Resolve the corresponding grid.
            var territoryInMap = this.FindByName<Grid>(territory.Name);
            if (territoryInMap != null) {
                foreach (var child in territoryInMap.Children) {
                    switch (child) {
                        case Image img:  // Tank icon layer
                            img.Source = IconSrcUser;
                            break;
                        case Grid grid:  // Counter layer
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

        foreach (Territory territory in CPU!.Territories.Values) {
            // Resolve the corresponding grid.
            var territoryInMap = this.FindByName<Grid>(territory.Name);
            if (territoryInMap != null) {
                foreach (var child in territoryInMap.Children) {
                    switch (child) {
                        case Image img:
                            img.Source = IconSrcCpu;
                            break;
                        case Grid grid:
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

    /// <summary>
    /// Shows the territory name in a temporary tooltip, since the label may
    /// be obscured by the tank icon.
    /// </summary>
    /// <param name="territoryName">Territory name.</param>
    private async void ShowNameOfTerritory(string territoryName) {
        tooltipLabel.Text = territoryName;  
        tooltipLabel.IsVisible = true;      
        await Task.Delay(2500);
        tooltipLabel.IsVisible = false; 
    }

    /// <summary>
    /// Handles a territory click according to the current turn phase.
    /// </summary>
    /// <param name="sender">Clicked territory button.</param>
    /// <param name="args">Event arguments.</param>
    private async void OnTerritoryClicked(object sender, EventArgs args) {
        try {
            // Read the territory name from the clicked button metadata.
            var button = sender as Button;
            var territoryNameWithSpaces = button!.ClassId;
            string territoryName = territoryNameWithSpaces!.RemoveSpaces();
            // Show the name first.
            ShowNameOfTerritory(territoryNameWithSpaces);
                // The remaining actions apply only to user-owned territories.
                if (User!.Territories.TryGetValue(territoryName, out Territory? from)) {
                    int numTanksinTerritory = from.Tanks.Count;
                    TaskCompletionSource<string> discoverUserChoose;
                    switch (Turn!.Phase) {
                        case TurnPhases.Reinforcement:
                            // Add one tank to the clicked territory.
                            User.Territories[territoryName].AddTanks(User.TankColor, 1);
                            // Refresh the map.
                            UpdateTankCounter(territoryName);
                            // Refresh the remaining-tank counter.
                            int oldValue = int.Parse(counterTankReinforcement.Text);
                            int newValue = --oldValue;
                            counterTankReinforcement.Text = newValue.ToString();
                            // Refresh the other counters.
                            UpdateUserCounters();
                            if (newValue == 0){
                                // No reinforcement tanks remain.
                                Turn.Phase = TurnPhases.Attack;
                                ShowOrHideReinforcementView();
                                HandleTurns();
                            }
                            break;

                        case TurnPhases.Attack when UserWantsToAttack:
                            PassButton.Text = "FINE";
                            // The user selected ATTACCA and is choosing an
                            // attacking territory.
                            // 1. The source territory must hold at least two tanks.
                            if (numTanksinTerritory > 1) {
                                // Retrieve the attackable neighboring territories.
                                List<string> attackableTerritories = UniWarSystem.Instance.AttackableTerritories(territoryName);
                                if (attackableTerritories.Count > 0) {
                                    // Let the user choose the target territory.
                                    discoverUserChoose = new TaskCompletionSource<string>();
                                    await Navigation.PushModalAsync(new SelectableTerritories(attackableTerritories, discoverUserChoose, isAttack: true));
                                    string attackedTerritoryChoosen = await discoverUserChoose.Task;
                                    await Task.Delay(400);  // Wait for the modal to close
                                    // Run the attack.
                                    AttackTerritory(territoryName, attackedTerritoryChoosen);
                                }
                                else
                                    ShowInformation("I territori confinanti appartengono tutti a te, scegli un altro territorio..");
                            }
                            else
                                ShowInformation("Questo territorio ha solo 1 carro armato!");
                            break;

                        case TurnPhases.StrategicShift:
                            UserWantsToAttack = false;
                            // Perform a strategic shift from the clicked territory.
                            // The source must hold more than one tank.
                            if (numTanksinTerritory > 1) {
                                // Retrieve the user-owned neighboring territories.
                                List<string> neighboringTerritories = UniWarSystem.Instance.UserNeighboringTerritories(territoryName);
                                if (neighboringTerritories.Count > 0) {
                                    // Let the user choose the destination territory.
                                    discoverUserChoose = new TaskCompletionSource<string>();
                                    await Navigation.PushModalAsync(new SelectableTerritories(neighboringTerritories, discoverUserChoose, isAttack: false));
                                    string selectedTerritory = await discoverUserChoose.Task;
                                    await Task.Delay(400);
                                    // Let the user choose how many tanks to move.
                                    var discoverTanksNumber = new TaskCompletionSource<int>();
                                    await Navigation.PushModalAsync(new StrategicShiftModal(numTanksinTerritory - 1, discoverTanksNumber));
                                    int tanksToMove = await discoverTanksNumber.Task;
                                    await Task.Delay(400);
                                    // Apply the move to the territories.
                                    MoveTanks(territoryName, selectedTerritory, tanksToMove);
                                    // The strategic shift ends the user turn.
                                    PassTurnToCpu();
                                }
                                else
                                    ShowInformation("I territori confinanti non appartengono a te, scegli un altro territorio..");
                            } 
                                else
                                    ShowInformation("Il territorio selezionato ha solo 1 carro armato.. cosa dovresti spostare?!");
                            break;
                        default:
                            break;
                    }
                } else {  // The user selected an enemy territory
                    TurnPhases phase = Turn!.Phase;
                    if (UserWantsToAttack || phase == TurnPhases.Reinforcement || phase == TurnPhases.StrategicShift)
                        ShowInformation("Devi selezionare un territorio che appartiene a te!");
                  }
        } catch (Exception ex) {
            ShowInformation("Errore: " + ex.Message);
        }
    }

    /// <summary>
    /// Ends the game and determines the winner from territory scores.
    /// </summary>
    public async void OnEndGameClicked(object sender, EventArgs args) {
        // At least two full rounds must have elapsed.
        if (Turn!.IdRound >= 3) {
            // Compute the final scores.
            int userScore = 0;
            int cpuScore = 0;

            foreach (var t in User!.Territories.Values)
                userScore += t.Score;
                
            foreach (var t in CPU!.Territories.Values)
                cpuScore += t.Score;
            Console.WriteLine($"CpuScore = {cpuScore}, UserScore = {userScore}");

            if (userScore > cpuScore) {
                await UpdateGameResult(true);
                await Navigation.PushModalAsync(new WinOrLoseModal(true));
            } else {
                await UpdateGameResult(false);
                await Navigation.PushModalAsync(new WinOrLoseModal(false));
            } 
        } else {
            ShowInformation("Bisogna quantomeno trovarsi al terzo giro!");
        }
    }

    /// <summary>
    /// Shows a temporary information banner.
    /// </summary>
    /// <param name="text">Message to display.</param>
    private async void ShowInformation(string text) {
        banner.IsVisible = true;
        bannerText.Text = text;
        await Task.Delay(4000);
        bannerText.Text = "";
        banner.IsVisible = false;
    }

    /// <summary>
    /// Toggles attack-selection mode.
    /// </summary>
    private void OnAttackButtonClicked(object sender, EventArgs e) {
        if (!UserWantsToAttack) {
            // Prompt the user to select the attacking territory.
            ShowInformation("Seleziona il territorio da cui vuoi effettuare l'attacco");  
            UserWantsToAttack = true;
            AttackButton.Text = "ANNULLA";      
        } else {                 
            UserWantsToAttack = false;
            ShowInformation("Ora, clickando su un territorio, ne vedrai soltanto il nome");
            AttackButton.Text = "ATTACCA";      
        }
    }

    /// <summary>
    /// Moves tanks between two user-owned territories.
    /// </summary>
    /// <param name="territoryFrom">Source territory.</param>
    /// <param name="territoryTo">Destination territory.</param>
    /// <param name="numOfTanksToMove">Number of tanks to move.</param>
    public async void MoveTanks(string territoryFrom, string territoryTo, int numOfTanksToMove) {
        await Task.Delay(200);
        Territory from = User!.Territories[territoryFrom];
        Territory to = User!.Territories[territoryTo];
        from.RemoveTanks(numOfTanksToMove);
        to.AddTanks(User.TankColor, numOfTanksToMove);
        // Refresh the map.
        DeployTanks();
    }

    /// <summary>
    /// Executes a user attack, including dice resolution, conquest handling,
    /// and statistics collection.
    /// </summary>
    /// <param name="attackingTerritory">Attacking territory.</param>
    /// <param name="attackedTerritory">Defending territory.</param>
    public async void AttackTerritory(string attackingTerritory, string attackedTerritory) {
        // Initialize the statistics payload.
        InitializeStatistics(out StatisticsCollection stats, true);

        Territory from = User!.Territories[attackingTerritory];
        int numTanksAttacker = from.Tanks.Count;
        Territory to = CPU!.Territories[attackedTerritory];
        int numTanksDefender = to.Tanks.Count;
        // Roll the dice.
        RollTheDice(out List<int> userDice, out List<int> cpuDice, numTanksAttacker, numTanksDefender);
        (int tanksLostForUser, int tanksLostForCpu) = CompareDiceAndRemoveTanks(in userDice, in cpuDice, from, to);

        // Record the per-territory losses.
        stats.AttackingTerritories.Add(from.Name, numTanksAttacker - from.Tanks.Count);
        stats.DefendingTerritories.Add(to.Name, numTanksDefender - to.Tanks.Count);

        var userClosedTheModal = new TaskCompletionSource();
        await Navigation.PushModalAsync(new ShowDiceResultPage(userDice, cpuDice, tanksLostForUser, tanksLostForCpu, userClosedTheModal));
        // Wait until the user closes the modal.
        await userClosedTheModal.Task;  // Resolved before the modal is popped
        await Task.Delay(100);  // Wait for the modal to close
        // Refresh the UI.
        DeployTanks();
        UpdateUserCounters();

        UserAttack = true;  // At least one attack occurred this turn

        // Check whether the defending territory was conquered.
        if (to.Tanks.Count == 0) {
            // The user conquered the CPU territory.
            CPU!.Territories.Remove(attackedTerritory);
            User!.Territories.Add(attackedTerritory, to);

            // Record the territory lost by the CPU.
            stats.LostTerritories!.Add(to.Name);

            // The user must move tanks into the conquered territory.
            var tcs = new TaskCompletionSource<int>();
            await Navigation.PushModalAsync(new ConqueredTerritoryModal(maxValue: from.Tanks.Count - 1, tcs));
            int numberSelected = await tcs.Task;
            await Task.Delay(100);  // Wait for the modal to close
            from.RemoveTanks(numberSelected);
            to.AddTanks(User!.TankColor, numberSelected);
            // Refresh the UI again.
            DeployTanks();
            UpdateUserCounters();

            // Refresh the statistics.
            SetStatsTanksAndTerritories(ref stats);
            
            // Check whether the new territory satisfies the victory condition.
            if (IsWin()) {
                await CollectsStatistics(stats);
                await UpdateGameResult(IsWin());
                await Navigation.PushModalAsync(new WinOrLoseModal(true));
            }
            
        } else {
            SetStatsTanksAndTerritories(ref stats);
        }

        await CollectsStatistics(stats);
    }

    
    /// <summary>
    /// Rolls attacker and defender dice and sorts them in descending order.
    /// </summary>
    /// <param name="userDice">Attacker dice.</param>
    /// <param name="cpuDice">Defender dice.</param>
    /// <param name="numTanksAttacker">Attacker tank count.</param>
    /// <param name="numTanksDefender">Defender tank count.</param>
    private static void RollTheDice(out List<int> userDice, out List<int> cpuDice, in int numTanksAttacker, in int numTanksDefender) {
        userDice = [];
        cpuDice = [];
        // The attacker rolls one die per tank, excluding one garrison tank.
        Random gen = new Random();
        int counter = 1;
        for (int i = 0; i < numTanksAttacker - 1; i++) {
            userDice.Add(gen.Next(6)+1); 
            if (counter == 3) break;  // At most three dice
            counter++;
        }
        // The defender rolls one die per tank.
        counter = 1;
        for (int i = 0; i < numTanksDefender; i++) {
            cpuDice.Add(gen.Next(6)+1); 
            if (counter == 3) break;  // At most three dice
            counter++;
        }
        // Sort in descending order.
        userDice.Sort((a,b) => b.CompareTo(a));
        cpuDice.Sort((a,b) => b.CompareTo(a));
    }

    /// <summary>
    /// Compares dice pairs and removes the corresponding tanks.
    /// </summary>
    /// <param name="userDice">Attacker dice.</param>
    /// <param name="cpuDice">Defender dice.</param>
    /// <param name="attacking">Attacking territory.</param>
    /// <param name="defending">Defending territory.</param>
    /// <returns>User losses and CPU losses.</returns>
    private static (int, int) CompareDiceAndRemoveTanks(in List<int> userDice, in List<int> cpuDice, Territory attacking, Territory defending) {
        // Compare as many pairs as the shorter dice list allows.
        int counterForUser = 0;
        int counterForCpu = 0;
        for (int i = 0; i < Math.Min(userDice.Count, cpuDice.Count); i++) {
            if (userDice[i] <= cpuDice[i]) {
                // Ties and lower values favor the defender.
                attacking.Tanks.RemoveAt(0);
                counterForUser++;
            } else {
                // The attacker wins the comparison.
                defending.Tanks.RemoveAt(0);
                counterForCpu++;
            }
        }
        // Return user losses followed by CPU losses.
        return (counterForUser, counterForCpu);
    }

    /// <summary>
    /// Ends the user turn, optionally after a strategic shift, and passes
    /// control to the CPU.
    /// </summary>
    private async void OnPassButtonClicked(object sender, EventArgs e) {        
        PassButton.IsVisible = false;
        if(!UserAttack){
            InitializeStatistics(out StatisticsCollection stats, true);
            SetStatsTanksAndTerritories(ref stats);
            await CollectsStatistics(stats);
        }
        await Task.Delay(500);
        // Ask whether the user wants a strategic shift.
        var discoverUserChoose = new TaskCompletionSource<bool>();
        await Navigation.PushModalAsync(new YesOrNotModal(discoverUserChoose));
        bool choose = await discoverUserChoose.Task;
        await Task.Delay(400);  // Wait for the modal to close
        if (choose == true) {
            // Enter the final phase of the turn.
            Turn!.Phase=TurnPhases.StrategicShift;
            ShowInformation("Fai click sul territorio DA cui vuoi spostare i carri armati");
            HandleTurns(); 
        } else {
            // Pass the turn to the CPU.
            AttackButton.IsVisible = false;
            PassButton.IsVisible = false;
            UserWantsToAttack = false;
            PassTurnToCpu();
        }
    }

    /// <summary>
    /// Marks the user turn as completed and activates the CPU turn.
    /// </summary>
    private void PassTurnToCpu() {
        UserTurnCompleted = true;
        Turn!.CurrentPlayer = CPU!;
        Turn.Phase = TurnPhases.Reinforcement;
        HandleTurns();
    }

    /// <summary>
    /// Marks the CPU turn as completed and activates the user turn.
    /// </summary>
    private void PassTurnToUser() {
        CpuTurnCompleted = true;
        Turn!.CurrentPlayer = User!;
        Turn.Phase = TurnPhases.Reinforcement;
    }

    /// <summary>
    /// Opens the statistics modal for the current game.
    /// </summary>
    private async void OnViewStatisticsClicked(object sender, EventArgs args) {
        await Navigation.PushModalAsync(new GameStatisticsView((int)UniWarSystem.Instance.GameId!));
    }
        
        
    
    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr reinforcement (string jsonData, int newTanks);

    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr cpuAttack (string jsonData);

    [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool winCheck (string jsonData);   

    /// <summary>
    /// Executes the CPU reinforcement phase through the native library and
    /// reflects the updated tank counts in the UI.
    /// </summary>
    private async Task CpuReinforcement(){
        TaskCompletionSource tcs;
        // Build the map payload passed to the C++ library.
        List<MapData> playersMaps = [
            new MapData {
                PlayerId = CPU!.Name,
                // Project the territory dictionary into the adjacency map
                // expected by the native layer (territory name -> neighbor names).
                Neighbors = CPU!.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                ),

                Tanks = CPU.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.Tanks.Count
                )
            }
        ];

        // Serialize the payload; WriteIndented produces readable JSON.
        string jsonData = JsonSerializer.Serialize(playersMaps, jsonSerializerOpt);

        // The native function returns a char* pointing to unmanaged memory,
        // represented by IntPtr on the managed side.

        // Reinforcement rule: each turn grants half of the owned territory
        // count as new tanks (integer division).
        int newTanks = CPU.Territories.Values.Count / 2;

        IntPtr resultPtr = reinforcement(jsonData, newTanks);

        // Copy the unmanaged UTF-8 string into managed memory. The null
        // coalescing operator handles a null pointer.
        string resultJson = Marshal.PtrToStringUTF8(resultPtr) ?? string.Empty;

        if (string.IsNullOrEmpty(resultJson) || resultJson == "{}") {
            ShowInformation("Si è verificato un errore");
            await Task.Delay(2000);  // Allow the error message to be read
            Application.Current!.MainPage = new AppShell();  // Return to the main screen
        }


        // The native function returns a static string, so no explicit
        // deallocation is required and no memory leak occurs.

        // Deserialize the updated CPU map.
        var updatedMap = JsonSerializer.Deserialize<MapData>(resultJson);
        
        // Add the missing tanks to each CPU territory.
        foreach(var territory in updatedMap.Tanks){
            if(CPU.Territories.TryGetValue(territory.Key, out Territory? value)) {
                var cpuTerritory = value;
                int difference = territory.Value - cpuTerritory.Tanks.Count;
                if(difference > 0)
                    cpuTerritory.AddTanks(CPU.TankColor, difference);
            }                        
        }

        DeployTanks();
        tcs = new TaskCompletionSource();
        await Navigation.PushModalAsync(new GenericModal("La CPU si è rinforzata! ",true,tcs,newTanks));
        await tcs.Task;  // Wait for setResult()
        await Task.Delay(400);  // Wait for the modal to close
        Turn!.Phase = TurnPhases.Attack;
    }



    /// <summary>
    /// Executes the CPU attack phase through the native library and replays
    /// the resulting battles in the UI.
    /// </summary>
    private async Task CpuAttack() {
        TaskCompletionSource tcs;
        List<MapData> playersMaps = [
            new MapData {
                PlayerId = CPU!.Name,
                Neighbors = CPU.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                ),
                Tanks= CPU.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.Tanks.Count
                )
            },

            new MapData {
                PlayerId = User!.Name,
                Neighbors = User.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                ),
                Tanks= User.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.Tanks.Count
                )
            }            
        ];

        string jsonData = JsonSerializer.Serialize(playersMaps, jsonSerializerOpt);
        IntPtr resultPtr = cpuAttack(jsonData);
            string resultJson = Marshal.PtrToStringUTF8(resultPtr) ?? string.Empty;

            if (string.IsNullOrEmpty(resultJson) || resultJson == "{}") {
            ShowInformation("Si è verificato un errore");
            await Task.Delay(2000);  // Allow the error message to be read
            Application.Current!.MainPage = new AppShell();  // Return to the main screen
        }


        if (resultJson == "Pass") {  // The CPU decided not to attack
            tcs = new TaskCompletionSource();
            InitializeStatistics(out StatisticsCollection stats, false);
            SetStatsTanksAndTerritories(ref stats);
            await CollectsStatistics(stats);
            await Navigation.PushModalAsync(new GenericModal("La CPU ha deciso di non attaccare",false,tcs));
            await tcs.Task;  // Wait for setResult()
            await Task.Delay(400);  // Wait for the modal to close
        } else {
            List<BattleResult>? battleResults = JsonSerializer.Deserialize<List<BattleResult>>(resultJson!);
            if (battleResults is not null) {
                await SimulateBattle(battleResults); 
            }
        }

        PassTurnToUser();
    }    




    /// <summary>
    /// Evaluates the user victory condition through the native library.
    /// </summary>
    /// <returns><c>true</c> when the user has won.</returns>
    bool IsWin(){
        List<MapData> playersMaps = [
            new MapData {
                PlayerId = User!.Name,
                Neighbors = User.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.NeighboringTerritories.Select(n => n.Name).ToList()
                ),
                Tanks= User.Territories.ToDictionary(
                    t => t.Key,
                    t => t.Value.Tanks.Count
                )
            }
        ];
    
        string jsonData = JsonSerializer.Serialize(playersMaps,jsonSerializerOpt);
        return winCheck(jsonData);
    }

    /// <summary>
    /// Replays CPU battles in the UI and updates the game state and
    /// statistics.
    /// </summary>
    /// <param name="battleList">Battles returned by the native library.</param>
    private async Task SimulateBattle(List<BattleResult> battleList) {
        TaskCompletionSource tcs;
        Territory? lostTerritory = null;
        InitializeStatistics(out StatisticsCollection stats, false);
        bool cpuWin = false;

        foreach(var battle in battleList){
            bool territoryLoss = false;
            
            foreach(var territory in User!.Territories.Values){
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

                    CPU!.AddTerritory(territory);
                    User.RemoveTerritory(territory);
                    lostTerritory = territory;
                    territoryLoss = true;
                    stats.LostTerritories!.Add(lostTerritory.Name);
                }
            }

            foreach(var territory in CPU!.Territories.Values){
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



            tcs = new TaskCompletionSource();
            await Navigation.PushModalAsync(new ShowCpuBattleTerritory(battle.AttackingTerritory, battle.DefendingTerritory, tcs));
            await tcs.Task;  // Wait for setResult()
            await Task.Delay(400);  // Wait for the modal to close

            tcs = new TaskCompletionSource();
            // Show the dice result.
            await Navigation.PushModalAsync(new ShowDiceResultPage(battle.DicePlayer,battle.DiceCPU, battle.LossesPlayer, battle.LossesCPU, tcs));
            await tcs.Task;  // Wait for setResult()
            await Task.Delay(400);  // Wait for the modal to close

            if(territoryLoss){
                tcs = new TaskCompletionSource();
                await Navigation.PushModalAsync(new ShowDefenceResult(tcs, lostTerritory!.Name));
                await tcs.Task;  // Wait for setResult()
                await Task.Delay(400);  // Wait for the modal to close
            }
                
            // Refresh the map.
            DeployTanks();
            BuildUserInformation();

            if (battle.Win) {     
                cpuWin = true;
                break;
            }
        }

        
        SetStatsTanksAndTerritories(ref stats);
        await CollectsStatistics(stats);

        if(cpuWin){
            await UpdateGameResult(true);
            await Navigation.PushModalAsync(new WinOrLoseModal(false));
        }

    }


    /// <summary>
    /// Fills the tank totals and owned territory lists in the statistics
    /// payload.
    /// </summary>
    /// <param name="stats">Statistics payload to update.</param>
    private void SetStatsTanksAndTerritories(ref StatisticsCollection stats){
        stats.CpuOwnedTanks = CPU!.GetNumTanks;
        stats.UserOwnedTanks = User!.GetNumTanks;

        foreach(var territory in CPU!.Territories.Values)
                stats.CpuOwnedTerritories.Add(territory.Name);

        foreach(var territory in User.Territories.Values)
                stats.UserOwnedTerritories.Add(territory.Name);

    }



    /// <summary>
    /// Initializes the statistics payload for a turn, including the case where
    /// no attack occurs.
    /// </summary>
    /// <param name="stats">Initialized statistics payload.</param>
    /// <param name="isUserTurn">Whether the current turn belongs to the user.</param>
    private void InitializeStatistics(out StatisticsCollection stats, bool isUserTurn){
        stats = new StatisticsCollection {
            Username = User!.Name,
            DefendingTerritories = [],
            AttackingTerritories = [],
            LostTerritories = [],
            UserOwnedTerritories = [],
            CpuOwnedTerritories = [],
            UserTurn = isUserTurn,
            RoundId = Turn!.IdRound
        };

        if(!UniWarSystem.Instance.IsOffline)
            stats.GameId = (int)UniWarSystem.Instance.GameId!;
    }



    /// <summary>
    /// Sends the final game result to the server when online.
    /// </summary>
    /// <param name="IsWin">Whether the user won.</param>
    private async Task UpdateGameResult(bool IsWin){
        if (!UniWarSystem.Instance.IsOffline) {
            try {
                var response = await ClientGrpc.EndGame((int)UniWarSystem.Instance.GameId!,IsWin);
                if (response.Status == false)
                    ShowInformation(response.Message);
            } catch (Grpc.Core.RpcException e) {
                ShowInformation($"{e}");
                Console.WriteLine($"Errore: {e}");
                await Task.Delay(1500);
            } catch (Exception e) {
                ShowInformation($"{e}");
                await Task.Delay(1500);
            }
        }   
    }

    /// <summary>
    /// Sends battle statistics to the server when online.
    /// </summary>
    /// <param name="statistics">Statistics payload to send.</param>
    private async Task CollectsStatistics(StatisticsCollection statistics) {
        if (!UniWarSystem.Instance.IsOffline) {
            try {
                await ClientGrpc.SendData(statistics);
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



