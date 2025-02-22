using UniWar;

public class UniWarSystem { // singleton
    private static UniWarSystem? _instance;
    private readonly Dictionary<string, Continent> _continents; // collezione di tutti i continenti gestiti dal gioco
    private readonly Dictionary<string, Territory> _territories; // collezione di tutti i territori gestiti dal gioco

    private readonly List<Goal> _goals; // collezione di tutti gli obiettivi
    public Player? User {get; set;}
    public Player? Cpu {get; set;}

    public Turn? Turn {get; set;}
    
    public bool IsOffline {get; set;} = false;
    public string? LoggedUsername {get; set;}
    public int? GameId {get; set;}

    public bool IsGameInitialized {get; private set;}



    // Proprietà pubblica per accedere all'istanza Singleton
    public static UniWarSystem Instance {
        get {
            if (_instance == null) 
                _instance = new UniWarSystem();
            return _instance;
        }
    }
    
    // il costruttore deve essere privato (accessibile solo da dentro)
    private UniWarSystem() {
        _continents = [];
        _territories = [];
        _goals = [new Goal("Conquista almeno 3 continenti e almeno 28 territori")];
        InitializeAll();
    }

    private void InitializeAll() {
        /*
            Metodo che "carica" i dati necessari al gioco
        */

        // creiamo i continenti
        CreateContinents();

        // impostiamo, per ogni territorio, i territori confinanti
        SetsNeighboringTerritories();
    }

    
    // OPERAZIONE DI SISTEMA UC1 -> inizializza la partita
    public async Task InitializeGame() {
        IsGameInitialized = true; // booleano che se vero ci dice che sono già state fatte partite nella sessione di gioco corrente
        /* 
            è, sostanzialmente, il metodo che inizializza la partita...
            - sceglie un colore per i carri armati per i partecipanti in maniera random tra i colori previsti dal risiko.
            - “mischia” le “carte” dei territori e le distribuisce equamente tra i partecipanti.
            - assegna un obiettivo ai partecipanti.
            - sceglie chi inizia il turno tra cpu e utente

        */

        // come PRIMISSIMA cosa, dobbiamo creare un GameId e creare una nuova entry nel database!
        await CreateGameInDatabase();

        Random gen = new();

        // bisogna distribuire equamente i territori ai due giocatori
        User = new Player("UserPlayer");
        Cpu = new Player("CpuPlayer");

        // recuperiamo tutti i territori
        List<Territory> allTerritories = [];
        allTerritories.AddRange(_territories.Values);

        // eseguiamo uno shuffle (metodo implementato nell'extension)
        allTerritories.Shuffle();

        // 21 territori a User e 21 alla CPU
        var firstHalf = allTerritories.Take(21).ToList(); // restituisce i primi 21 elementi
        var secondHalf = allTerritories.Skip(21).ToList(); // restituisce i restanti elementi dopo i primi 21

        foreach (var ter in firstHalf)
            User.AddTerritory(ter);

        foreach (var ter in secondHalf) 
            Cpu.AddTerritory(ter);
        
        // scegliamo due colori random, i colori sono 5
        int colorForUser = gen.Next(5);
        int colorForCpu;
        do { // vogliamo assolutamente evitare che i colori siano uguali
            colorForCpu = gen.Next(5);
        } while (colorForUser == colorForCpu);
           
        // per ogni territorio del'utente, associamo 3 carri armati 
        foreach (Territory territory in User.Territories.Values) 
            territory.AddTanks(colorForUser,3);
        
        // per ogni territorio della CPU, associamo 3 carri armati 
        foreach (Territory territory in Cpu.Territories.Values) 
            territory.AddTanks(colorForCpu,3);
        
            
        // obiettivo ai partecipanti
        User.Goal = _goals[0];
        Cpu.Goal = _goals[0];

        // Necessario per l'inserimento di nuovi carri armati
        User.TankColor = colorForUser;
        Cpu.TankColor = colorForCpu;

        if (gen.Next(2)== 0) 
            Turn = new Turn(User);
        else 
            Turn = new Turn(Cpu);
    }

    private async Task CreateGameInDatabase() {
        // Ovviamente, il GameId nel db va creato solo se l'utente sta giocando online
        if (IsOffline) return;
        var response = await ClientGrpc.NewGame(LoggedUsername!);
        GameId = response.GameId;
        Console.WriteLine("GameId: " + GameId);
    }

    public void ResetAll() {
        /*
            Questo metodo viene invocato nella modale di GameOver e si occupa 
            di far sì che le istanze della classi singleton TablePage e UniWar 
            siano reistanziate
        */

        TablePage.Reset();

        // per quanto riguarda, però, l'istanza di questa classe, dobbiamo 
        // preservare l'utente loggato
        
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


    // Mostra elenco territori confinanti "attaccabili"
    // invocata dalla pagina "TablePage"
    public List<string> AttackableTerritories(string territoryName) {
        /*
            Dato il nome di un territorio, ne restituisce quelli confinanti appartenenti all'avversario.
        */
        List<Territory> attackableNeighboringTerritories = [];
        attackableNeighboringTerritories.AddRange(_territories[territoryName].NeighboringTerritories);
        // dobbiamo restituire quelli che non appartengono all'utente
        
        attackableNeighboringTerritories.RemoveAll(territory => User!.Territories.ContainsValue(territory));
        
        List<string> attackableNeighboringTerritoriesNames = [];
        foreach (Territory item in attackableNeighboringTerritories) 
            attackableNeighboringTerritoriesNames.Add(item.Name);
        
        return attackableNeighboringTerritoriesNames;
    }

    // Mostra elenco territori confinanti "amici"
    // invocata dalla pagina "TablePage"
    public List<string> UserNeighboringTerritories(string territoryName) {
        /*
            Dato il nome di un territorio, ne restituisce quelli confinanti appartenenti all'utente stesso.
        */
        List<Territory> userNeighboringTerritories = [];
        userNeighboringTerritories.AddRange(_territories[territoryName].NeighboringTerritories);
        // dobbiamo restituire quelli che appartengono all'utente; in altri termini
        // rimuoviamo quelli appartenenti alla CPU
        userNeighboringTerritories.RemoveAll(territory => Cpu!.Territories.ContainsValue(territory));
        
        List<string> userNeighboringTerritoriesNames = [];
        foreach (Territory item in userNeighboringTerritories) {
            userNeighboringTerritoriesNames.Add(item.Name);
        }
        return userNeighboringTerritoriesNames;
    }

    public void OfflineMode() {
        IsOffline = true;
        LoggedUsername = null;
    }

    public void SetLogged(string username) {
        LoggedUsername = username;
        IsOffline = false;
    }



    // SOTTO, i metodi di caricamento dei dati (relativi all'InitializeAll()).
    private void SetsNeighboringTerritories() {
        // qui, territorio per territorio, bisogna impostare manualmente quelli confinanti...
        // procediamo per continente
        AmericaDelNord();
        AmericaDelSud();
        Europa();
        Africa();
        Asia();
        Oceania();
    }

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

        // travasiamo tutti i territori nella collezione della classe
        foreach (Continent con in _continents.Values) 
            foreach (Territory ter in con.Territories) 
                _territories.Add(ter.Name, ter);
    }


    // metodi per impostare i territori confinanti
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
    }
    private void Oceania() {
        // 38. Indonesia
        _territories["Indonesia"].NeighboringTerritories.AddRange([
            _territories["Siam"],
            _territories["NuovaGuinea"],
            _territories["AustraliaOccidentale"]
        ]);
        // 39. Nuova Guinea
        _territories["NuovaGuinea"].NeighboringTerritories.AddRange([
            _territories["Indonesia"],
            _territories["AustraliaOrientale"],
            _territories["AustraliaOccidentale"]
        ]);
        // 40. Australia Occidentale
        _territories["AustraliaOccidentale"].NeighboringTerritories.AddRange([
            _territories["Indonesia"],
            _territories["NuovaGuinea"],
            _territories["AustraliaOrientale"]
        ]);
        // 41. Australia Orientale
        _territories["AustraliaOrientale"].NeighboringTerritories.AddRange([
            _territories["AustraliaOccidentale"],
            _territories["NuovaGuinea"]
        ]);
    }   
}