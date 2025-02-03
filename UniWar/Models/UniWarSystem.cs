public class UniWarSystem { // singleton
    private static UniWarSystem? _instance;
    private readonly Dictionary<string, Continent> _continents; // collezione di tutti i continenti gestiti dal gioco
    private readonly Dictionary<string, Territory> _territories; // collezione di tutti i continenti gestiti dal gioco

    private readonly List<Goal> _goals; // collezione di tutti gli obiettivi
    private List<Player> _players; 


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
        _continents = new Dictionary<string, Continent>();
        _goals = [new Goal("Conquista almeno 3 continenti e almeno 28 territori")];
        _players = [];
        InitializeAll();
    }

    private void InitializeAll() {
        // creiamo i continenti
        CreateContinents();

        // impostiamo, per ogni territorio, i territori confinanti
        SetsNeighboringTerritories();
        
        


       
    }

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
            new Territory("EurpoaOccidentale",4),
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


    // OPERAZIONE DI SISTEMA UC1 -> inizializza la partita
    public (Player, Player) InitializeGame() {
        /* 
            è, sostanzialmente, il metodo che inizializza la partita...
            - sceglie un colore per i carri armati per i partecipanti (l’utente e il sistema stesso… in maniera random tra i colori previsti dal risiko).
            - “mischia” le “carte” dei territori e le distribuisce equamente tra i partecipanti.
            - assegna un obiettivo ai partecipanti.
            - sceglie chi inizia il turno tra cpu e utente

        */

        Random gen = new Random();

        // qui il sistema deve distribuire equamente i territori ai due giocatori
        Player user = new Player(isCpu: false);
        Player cpu = new Player(isCpu: true);
        _players.AddRange([cpu, user]);

        // recuperiamo tutti i territori
        List<Territory> allTerritories = new List<Territory>();
        foreach (Continent c in _continents.Values) {
            foreach (Territory ter in c.Territories) {
                allTerritories.Add(ter);
            }
        }

        // eseguiamo uno shuffle (metodo implementato nell'extension)
        allTerritories.Shuffle();

        // 21 territori a User e 21 alla CPU
        var firstHalf = allTerritories.Take(21).ToList();
        var secondHalf = allTerritories.Skip(21).ToList();
        user.Territories = firstHalf;
        cpu.Territories = secondHalf;

        // scegliamo due colori random, i colori sono 6
        int colorForUser = gen.Next(6);
        int colorForCpu;
        do { // vogliamo assolutamente evitare che i colori siano uguali
            colorForCpu = gen.Next(6);
        } while (colorForUser == colorForCpu);
           
        // per ogni territorio del'utente, associamo 3 carri armati 
        foreach (Territory territory in user.Territories)
            territory.Tanks.AddRange([new Tank(colorForUser), new Tank(colorForUser), new Tank(colorForUser)]);
        
        // per ogni territorio della CPU, associamo 3 carri armati 
        foreach (Territory territory in cpu.Territories)
            territory.Tanks.AddRange([new Tank(colorForCpu), new Tank(colorForCpu), new Tank(colorForCpu)]);

        // obiettivo ai partecipanti
        user.Goal = _goals[0];
        cpu.Goal = _goals[0];

        // turno
        if (gen.Next(2)==0) 
            user.Turn = new Turn(TurnPhases.Attack);
        else 
            cpu.Turn = new Turn(TurnPhases.Attack);
        
        
        return (user, cpu);
    }

    // OPERAZIONE DI SISTEMA UC6 -> mostra elenco territori confinanti "attaccabili"
    public List<Territory> AttackableTerritories(string territoryName) {
        /*
            dato il nome di un territorio
        */

        return [];
    }






    // metodi per impostare i territori confinanti
    private void AmericaDelNord() {
         // 1. Alaska
        _territories["Alska"].NeighboringTerritories.AddRange([
            _territories["Kamchatka"], 
            _territories["TerritoriDelNordOvest"], 
            _territories["Alberta"],]);
        // 2. Alberta
         _territories["Alberta"].NeighboringTerritories.AddRange([
            _territories["Alska"], 
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
            _territories["Peru"]
        ]);

        // 11. Brasile
        _territories["Brasile"].NeighboringTerritories.AddRange([
            _territories["Venezuela"],
            _territories["Peru"],
            _territories["Argentina"],
            _territories["AfricaDelNord"]
        ]);

        // 12. Perù
        _territories["Peru"].NeighboringTerritories.AddRange([
            _territories["Venezuela"],
            _territories["Brasile"],
            _territories["Argentina"]
        ]);

        // 13. Argentina
        _territories["Argentina"].NeighboringTerritories.AddRange([
            _territories["Peru"],
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
            _territories["Ural"],
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
            _territories["MedioOriente"],
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
        // 27. Ural
        _territories["Ural"].NeighboringTerritories.AddRange([
            _territories["Ucraina"],
            _territories["Afghanistan"],
            _territories["Siberia"],
            _territories["Cina"]
        ]);

        // 28. Siberia
        _territories["Siberia"].NeighboringTerritories.AddRange([
            _territories["Ural"],
            _territories["Cina"],
            _territories["Mongolia"],
            _territories["Jacuzia"]
        ]);

        // 29. Jacuzia
        _territories["Jacuzia"].NeighboringTerritories.AddRange([
            _territories["Siberia"],
            _territories["Mongolia"],
            _territories["Kamchatka"]
        ]);

        // 30. Kamchatka
        _territories["Kamchatka"].NeighboringTerritories.AddRange([
            _territories["Jacuzia"],
            _territories["Mongolia"],
            _territories["Giappone"],
            _territories["Alaska"]
        ]);

        // 31. Mongolia
        _territories["Mongolia"].NeighboringTerritories.AddRange([
            _territories["Siberia"],
            _territories["Jacuzia"],
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
            _territories["Ural"],
            _territories["Cina"],
            _territories["India"],
            _territories["MedioOriente"]
        ]);

        // 34. Cina
        _territories["Cina"].NeighboringTerritories.AddRange([
            _territories["Ural"],
            _territories["Siberia"],
            _territories["Mongolia"],
            _territories["Afghanistan"],
            _territories["India"],
            _territories["Siam"]
        ]);

        // 35. Medio Oriente
        _territories["MedioOriente"].NeighboringTerritories.AddRange([
            _territories["Egitto"],
            _territories["EuropaMeridionale"],
            _territories["Ucraina"],
            _territories["Afghanistan"],
            _territories["India"],
            _territories["AfricaOrientale"]
        ]);

        // 36. India
        _territories["India"].NeighboringTerritories.AddRange([
            _territories["MedioOriente"],
            _territories["Afghanistan"],
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