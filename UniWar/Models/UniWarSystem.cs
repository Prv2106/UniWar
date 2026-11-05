using UniWar;

/// <summary>
/// Singleton holding the global game state, including the continent and
/// territory catalog, the current players, and the active turn.
/// </summary>
public class UniWarSystem {
    private static UniWarSystem? _instance;
    private readonly Dictionary<string, Continent> _continents;  // All continents managed by the game
    private readonly Dictionary<string, Territory> _territories;  // All territories managed by the game

    private readonly List<Goal> _goals;  // All available goals
    public Player? User {get; set;}
    public Player? Cpu {get; set;}

    public Turn? Turn {get; set;}
    
    public bool IsOffline {get; set;} = false;
    public string? LoggedUsername {get; set;}
    public int? GameId {get; set;}

    public bool IsGameInitialized {get; private set;}



    /// <summary>
    /// Gets the singleton instance, creating it on first access.
    /// </summary>
    public static UniWarSystem Instance {
        get {
            if (_instance == null) 
                _instance = new UniWarSystem();
            return _instance;
        }
    }
    
    // The constructor is private so that only the class itself can instantiate it.
    private UniWarSystem() {
        _continents = [];
        _territories = [];
        _goals = [new Goal("Conquista almeno 3 continenti e almeno 28 territori")];
        InitializeAll();
    }

    /// <summary>
    /// Loads the static game data: continents and territory adjacency.
    /// </summary>
    private void InitializeAll() {
        // Create the continents.
        CreateContinents();

        // Configure the neighboring territories.
        SetsNeighboringTerritories();
    }

    
    /// <summary>
    /// Initializes a new match: assigns tank colors and territories to both
    /// players, assigns the goal, and randomly selects the first player.
    /// </summary>
    /// <remarks>
    /// When the session is online, a new game record is created in the
    /// database before the in-memory state is initialized.
    /// </remarks>
    public async Task InitializeGame() {
        IsGameInitialized = true;  // True once at least one match has been started in this session
        /*
            Initialization steps:
            - pick a random tank color for each participant among the five
              colors supported by the game;
            - shuffle the territory deck and deal 21 territories to each player;
            - assign the goal;
            - choose which participant starts the turn.
        */

        // Create the game identifier and the corresponding database row first.
        await CreateGameInDatabase();

        Random gen = new();

        // Deal the territories evenly between the two players.
        User = new Player(LoggedUsername ?? "UserPlayer");
        Cpu = new Player("CpuPlayer");

        // Collect every territory.
        List<Territory> allTerritories = [];
        allTerritories.AddRange(_territories.Values);

        // Shuffle using the extension method.
        allTerritories.Shuffle();

        // 21 territories to the user and 21 to the CPU.
        var firstHalf = allTerritories.Take(21).ToList();  // First 21 elements
        var secondHalf = allTerritories.Skip(21).ToList();  // Elements after the first 21

        foreach (var ter in firstHalf)
            User.AddTerritory(ter);

        foreach (var ter in secondHalf) 
            Cpu.AddTerritory(ter);
        
        // Pick two distinct random colors out of five.
        int colorForUser = gen.Next(5);
        int colorForCpu;
        do {  // Ensure the colors differ.
            colorForCpu = gen.Next(5);
        } while (colorForUser == colorForCpu);
           
        // Assign three tanks to each user territory.
        foreach (Territory territory in User.Territories.Values) 
            territory.AddTanks(colorForUser,3);
        
        // Assign three tanks to each CPU territory.
        foreach (Territory territory in Cpu.Territories.Values) 
            territory.AddTanks(colorForCpu,3);
        
            
        // Assign the goal to both participants.
        User.Goal = _goals[0];
        Cpu.Goal = _goals[0];

        // Required when adding new tanks.
        User.TankColor = colorForUser;
        Cpu.TankColor = colorForCpu;

        if (gen.Next(2)== 0) 
            Turn = new Turn(User);
        else 
            Turn = new Turn(Cpu);
    }

    /// <summary>
    /// Creates a new game record when the session is online.
    /// </summary>
    private async Task CreateGameInDatabase() {
        if (IsOffline) return;
        var response = await ClientGrpc.NewGame(LoggedUsername!);
        GameId = response.GameId;
        Console.WriteLine("GameId: " + GameId);
    }

    /// <summary>
    /// Resets the singleton game state and the table page after game over.
    /// The logged username is preserved.
    /// </summary>
    public void ResetAll() {
        TablePage.Reset();

        // Preserve the logged username across the reset.
        if (IsOffline) {
            _instance = null;
            Instance.IsOffline = true;
        } else {
            string loggedUsername = Instance.LoggedUsername!;
            _instance = null;
            Instance.LoggedUsername = loggedUsername;
            Instance.IsOffline = false;
        }              
    }


    /// <summary>
    /// Returns the enemy territories adjacent to the specified territory.
    /// </summary>
    /// <param name="territoryName">Name of the source territory.</param>
    /// <returns>Names of the attackable neighboring territories.</returns>
    public List<string> AttackableTerritories(string territoryName) {
        List<Territory> attackableNeighboringTerritories = [];
        attackableNeighboringTerritories.AddRange(_territories[territoryName].NeighboringTerritories);
        // Exclude territories owned by the user.
        
        attackableNeighboringTerritories.RemoveAll(territory => User!.Territories.ContainsValue(territory));
        
        List<string> attackableNeighboringTerritoriesNames = [];
        foreach (Territory item in attackableNeighboringTerritories) 
            attackableNeighboringTerritoriesNames.Add(item.Name);
        
        return attackableNeighboringTerritoriesNames;
    }

    /// <summary>
    /// Returns the user-owned territories adjacent to the specified territory.
    /// </summary>
    /// <param name="territoryName">Name of the source territory.</param>
    /// <returns>Names of the friendly neighboring territories.</returns>
    public List<string> UserNeighboringTerritories(string territoryName) {
        List<Territory> userNeighboringTerritories = [];
        userNeighboringTerritories.AddRange(_territories[territoryName].NeighboringTerritories);
        // Exclude territories owned by the CPU.
        userNeighboringTerritories.RemoveAll(territory => Cpu!.Territories.ContainsValue(territory));
        
        List<string> userNeighboringTerritoriesNames = [];
        foreach (Territory item in userNeighboringTerritories) {
            userNeighboringTerritoriesNames.Add(item.Name);
        }
        return userNeighboringTerritoriesNames;
    }

    /// <summary>
    /// Enables offline mode and clears the logged username.
    /// </summary>
    public void OfflineMode() {
        IsOffline = true;
        LoggedUsername = null;
    }

    /// <summary>
    /// Enables online mode for the specified user.
    /// </summary>
    /// <param name="username">Authenticated username.</param>
    public void SetLogged(string username) {
        LoggedUsername = username;
        IsOffline = false;
    }



    // Territory-loading helpers used by InitializeAll().

    /// <summary>
    /// Configures the adjacency lists for every territory, continent by
    /// continent.
    /// </summary>
    private void SetsNeighboringTerritories() {
        AmericaDelNord();
        AmericaDelSud();
        Europa();
        Africa();
        Asia();
        Oceania();
    }

    /// <summary>
    /// Creates the continent catalog and indexes every territory by name.
    /// </summary>
    private void CreateContinents() {
        _continents.Add("AmericaDelNord", new Continent("AmericaDelNord", 9, new List<Territory>() {
            new Territory("Alaska",3),
            new Territory("Alberta",4),
            new Territory("StatiUnitiOccidentali",4),
            new Territory("StatiUnitiOrientali",4),
            new Territory("AmericaCentrale",3),
            new Territory("Ontario",6),
            new Territory("TerritoriDelNordOvest",4),
            new Territory("Groenlandia",4),
            new Territory("Quebec",3),
        }));
        _continents.Add("AmericaDelSud", new Continent("AmericaDelSud", 4, new List<Territory>() {
            new Territory("Venezuela",3),
            new Territory("Perù",3),
            new Territory("Brasile",4),
            new Territory("Argentina",2),
        }));
        _continents.Add("Africa", new Continent("Africa", 6, new List<Territory>() {
            new Territory("AfricaDelNord",6),
            new Territory("Egitto",4),
            new Territory("AfricaOrientale",5),
            new Territory("Congo",3),
            new Territory("AfricaDelSud",3),
            new Territory("Madagascar",2),
        }));
        _continents.Add("Asia", new Continent("Asia", 12, new List<Territory>() {
            new Territory("Kamchatka",5),
            new Territory("Jacuzia",3),
            new Territory("Cita",4),
            new Territory("Giappone",2),
            new Territory("Cina",7),
            new Territory("Siam",3),
            new Territory("India",3),
            new Territory("MedioOriente",6),
            new Territory("Afghanistan",4),
            new Territory("Urali",4),
            new Territory("Siberia",5),
            new Territory("Mongolia",5),
        }));
         _continents.Add("Europa", new Continent("Africa", 7, new List<Territory>() {
            new Territory("EuropaOccidentale",4),
            new Territory("EuropaMeridionale",6),
            new Territory("GranBretagna",4),
            new Territory("Islanda",3),
            new Territory("Ucraina",6),
            new Territory("EuropaSettentrionale",5),
            new Territory("Scandinavia",4),
        }));
        _continents.Add("Oceania", new Continent("Oceania", 4, new List<Territory>() {
            new Territory("AustraliaOccidentale",3),
            new Territory("NuovaGuinea",3),
            new Territory("Indonesia",3),
            new Territory("AustraliaOrientale",3),
        }));

        // Copy every territory into the class-level collection.
        foreach (Continent con in _continents.Values) 
            foreach (Territory ter in con.Territories) 
                _territories.Add(ter.Name, ter);
    }


    // Adjacency configuration, grouped by continent.
    private void AmericaDelNord() {
         // 1. Alaska
        _territories["Alaska"].NeighboringTerritories.AddRange([
            _territories["Kamchatka"], 
            _territories["TerritoriDelNordOvest"], 
            _territories["Alberta"],]);
        // 2. Alberta
         _territories["Alberta"].NeighboringTerritories.AddRange([
            _territories["Alaska"], 
            _territories["TerritoriDelNordOvest"],
            _territories["Ontario"],
            _territories["StatiUnitiOccidentali"],]);
        // 3. StatiUnitiOccidentali
         _territories["StatiUnitiOccidentali"].NeighboringTerritories.AddRange([
            _territories["Alberta"], 
            _territories["StatiUnitiOrientali"],
            _territories["Ontario"],
            _territories["AmericaCentrale"],]);
        // 4. Territori del Nord Ovest
        _territories["TerritoriDelNordOvest"].NeighboringTerritories.AddRange([
            _territories["Alaska"],
            _territories["Alberta"],
            _territories["Ontario"],
            _territories["Groenlandia"]]);
        // 5. Ontario
        _territories["Ontario"].NeighboringTerritories.AddRange([
            _territories["TerritoriDelNordOvest"],
            _territories["Alberta"],
            _territories["StatiUnitiOccidentali"],
            _territories["StatiUnitiOrientali"],
            _territories["Quebec"],
            _territories["Groenlandia"]]);
        // 6. Groenlandia
        _territories["Groenlandia"].NeighboringTerritories.AddRange([
            _territories["TerritoriDelNordOvest"],
            _territories["Ontario"],
            _territories["Quebec"],
            _territories["Islanda"]]);
        // 7. Quebec
        _territories["Quebec"].NeighboringTerritories.AddRange([
            _territories["Ontario"],
            _territories["StatiUnitiOrientali"],
            _territories["Groenlandia"]]);
        // 8. Stati Uniti Orientali
        _territories["StatiUnitiOrientali"].NeighboringTerritories.AddRange([
            _territories["Ontario"],
            _territories["Quebec"],
            _territories["StatiUnitiOccidentali"],
            _territories["AmericaCentrale"]]);
        // 9. America Centrale
        _territories["AmericaCentrale"].NeighboringTerritories.AddRange([
            _territories["StatiUnitiOccidentali"],
            _territories["StatiUnitiOrientali"],
            _territories["Venezuela"]
        ]);
    }

    private void AmericaDelSud() {
        // 10. Venezuela
        _territories["Venezuela"].NeighboringTerritories.AddRange([
            _territories["AmericaCentrale"],
            _territories["Brasile"],
            _territories["Perù"]
        ]);
        // 11. Brasile
        _territories["Brasile"].NeighboringTerritories.AddRange([
            _territories["Venezuela"],
            _territories["Perù"],
            _territories["Argentina"],
            _territories["AfricaDelNord"]
        ]);
        // 12. Perù
        _territories["Perù"].NeighboringTerritories.AddRange([
            _territories["Venezuela"],
            _territories["Brasile"],
            _territories["Argentina"]
        ]);
        // 13. Argentina
        _territories["Argentina"].NeighboringTerritories.AddRange([
            _territories["Perù"],
            _territories["Brasile"]
        ]);
    }
    private void Europa() {
        // 14. Islanda
        _territories["Islanda"].NeighboringTerritories.AddRange([
            _territories["Groenlandia"],
            _territories["GranBretagna"],
            _territories["Scandinavia"]
        ]);
        // 15. Gran Bretagna
        _territories["GranBretagna"].NeighboringTerritories.AddRange([
            _territories["Islanda"],
            _territories["Scandinavia"],
            _territories["EuropaOccidentale"],
            _territories["EuropaSettentrionale"]
        ]);
        // 16. Scandinavia
        _territories["Scandinavia"].NeighboringTerritories.AddRange([
            _territories["Islanda"],
            _territories["GranBretagna"],
            _territories["EuropaSettentrionale"],
            _territories["Ucraina"]
        ]);
        // 17. Europa Occidentale
        _territories["EuropaOccidentale"].NeighboringTerritories.AddRange([
            _territories["GranBretagna"],
            _territories["EuropaSettentrionale"],
            _territories["EuropaMeridionale"],
            _territories["AfricaDelNord"]
        ]);
        // 18. Europa Settentrionale
        _territories["EuropaSettentrionale"].NeighboringTerritories.AddRange([
            _territories["GranBretagna"],
            _territories["Scandinavia"],
            _territories["Ucraina"],
            _territories["EuropaMeridionale"],
            _territories["EuropaOccidentale"]
        ]);
        // 19. Europa Meridionale
        _territories["EuropaMeridionale"].NeighboringTerritories.AddRange([
            _territories["EuropaOccidentale"],
            _territories["EuropaSettentrionale"],
            _territories["Ucraina"],
            _territories["MedioOriente"],
            _territories["AfricaDelNord"],
            _territories["Egitto"]
        ]);
        // 20. Ucraina
        _territories["Ucraina"].NeighboringTerritories.AddRange([
            _territories["Scandinavia"],
            _territories["EuropaSettentrionale"],
            _territories["EuropaMeridionale"],
            _territories["Afghanistan"],
            _territories["Urali"],
            _territories["MedioOriente"]
        ]);
    }
    private void Africa() {
        // 21. Africa del Nord
        _territories["AfricaDelNord"].NeighboringTerritories.AddRange([
            _territories["Brasile"],
            _territories["EuropaOccidentale"],
            _territories["EuropaMeridionale"],
            _territories["Egitto"],
            _territories["AfricaOrientale"],
            _territories["Congo"]
        ]);
        // 22. Egitto
        _territories["Egitto"].NeighboringTerritories.AddRange([
            _territories["AfricaDelNord"],
            _territories["EuropaMeridionale"],
            _territories["MedioOriente"],
            _territories["AfricaOrientale"]
        ]);
        // 23. Congo
        _territories["Congo"].NeighboringTerritories.AddRange([
            _territories["AfricaDelNord"],
            _territories["AfricaOrientale"],
            _territories["AfricaDelSud"]
        ]);
        // 24. Africa Orientale
        _territories["AfricaOrientale"].NeighboringTerritories.AddRange([
            _territories["Egitto"],
            _territories["AfricaDelNord"],
            _territories["Congo"],
            _territories["AfricaDelSud"],
            _territories["Madagascar"]
        ]);
        // 25. Africa del Sud
        _territories["AfricaDelSud"].NeighboringTerritories.AddRange([
            _territories["Congo"],
            _territories["AfricaOrientale"],
            _territories["Madagascar"]
        ]);
        // 26. Madagascar
        _territories["Madagascar"].NeighboringTerritories.AddRange([
            _territories["AfricaOrientale"],
            _territories["AfricaDelSud"]
        ]);
    }
    private void Asia() {
        // 27. Urali
        _territories["Urali"].NeighboringTerritories.AddRange([
            _territories["Ucraina"],
            _territories["Afghanistan"],
            _territories["Siberia"],
            _territories["Cina"]
        ]);
        // 28. Siberia
        _territories["Siberia"].NeighboringTerritories.AddRange([
            _territories["Urali"],
            _territories["Cina"],
            _territories["Mongolia"],
            _territories["Jacuzia"],
            _territories["Cita"]
        ]);
        // 29. Jacuzia
        _territories["Jacuzia"].NeighboringTerritories.AddRange([
            _territories["Siberia"],
            _territories["Cita"],
            _territories["Kamchatka"]
        ]);
        // 30. Kamchatka
        _territories["Kamchatka"].NeighboringTerritories.AddRange([
            _territories["Jacuzia"],
            _territories["Mongolia"],
            _territories["Giappone"],
            _territories["Alaska"],
            _territories["Cita"]
        ]);
        // 31. Mongolia
        _territories["Mongolia"].NeighboringTerritories.AddRange([
            _territories["Siberia"],
            _territories["Cita"],
            _territories["Kamchatka"],
            _territories["Cina"],
            _territories["Giappone"]
        ]);
        // 32. Giappone
        _territories["Giappone"].NeighboringTerritories.AddRange([
            _territories["Mongolia"],
            _territories["Kamchatka"]
        ]);
        // 33. Afghanistan
        _territories["Afghanistan"].NeighboringTerritories.AddRange([
            _territories["Ucraina"],
            _territories["Urali"],
            _territories["Cina"],
            _territories["MedioOriente"]
        ]);
        // 34. Cina
        _territories["Cina"].NeighboringTerritories.AddRange([
            _territories["Urali"],
            _territories["Siberia"],
            _territories["Mongolia"],
            _territories["Afghanistan"],
            _territories["India"],
            _territories["Siam"],
            _territories["MedioOriente"]
        ]);
        // 35. Medio Oriente
        _territories["MedioOriente"].NeighboringTerritories.AddRange([
            _territories["Egitto"],
            _territories["EuropaMeridionale"],
            _territories["Ucraina"],
            _territories["Afghanistan"],
            _territories["India"],
            _territories["Cina"]
        ]);
        // 36. India
        _territories["India"].NeighboringTerritories.AddRange([
            _territories["MedioOriente"],
            _territories["Cina"],
            _territories["Siam"]
        ]);
        // 37. Siam
        _territories["Siam"].NeighboringTerritories.AddRange([
            _territories["India"],
            _territories["Cina"],
            _territories["Indonesia"]
        ]);
        // 38. Cita
        _territories["Cita"].NeighboringTerritories.AddRange([
            _territories["Siberia"],
            _territories["Mongolia"],
            _territories["Kamchatka"],
            _territories["Jacuzia"],
        ]);
    }
    private void Oceania() {
        // 39. Indonesia
        _territories["Indonesia"].NeighboringTerritories.AddRange([
            _territories["Siam"],
            _territories["NuovaGuinea"],
            _territories["AustraliaOccidentale"]
        ]);
        // 40. Nuova Guinea
        _territories["NuovaGuinea"].NeighboringTerritories.AddRange([
            _territories["Indonesia"],
            _territories["AustraliaOrientale"],
            _territories["AustraliaOccidentale"]
        ]);
        // 41. Australia Occidentale
        _territories["AustraliaOccidentale"].NeighboringTerritories.AddRange([
            _territories["Indonesia"],
            _territories["NuovaGuinea"],
            _territories["AustraliaOrientale"]
        ]);
        // 42. Australia Orientale
        _territories["AustraliaOrientale"].NeighboringTerritories.AddRange([
            _territories["AustraliaOccidentale"],
            _territories["NuovaGuinea"]
        ]);
    }   
}