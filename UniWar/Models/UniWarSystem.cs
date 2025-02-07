using Windows.System;

public class UniWarSystem { // singleton
    private static UniWarSystem? _instance;
    private readonly Dictionary<string, Continent> _continents; // collezione di tutti i continenti gestiti dal gioco
    private readonly Dictionary<string, Territory> _territories; // collezione di tutti i continenti gestiti dal gioco

    private readonly List<Goal> _goals; // collezione di tutti gli obiettivi
    private Turn? CurrentTurn {get; set;}
    public Player? User {get; set;}
    public Player? Cpu {get; set;}

    private static Random gen = new Random();
    // dichiarato come static perchè lo usiamo in più metodi!


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
    public void InitializeGame() {
        /* 
            è, sostanzialmente, il metodo che inizializza la partita...
            - sceglie un colore per i carri armati per i partecipanti (l’utente e il sistema stesso… in maniera random tra i colori previsti dal risiko).
            - “mischia” le “carte” dei territori e le distribuisce equamente tra i partecipanti.
            - assegna un obiettivo ai partecipanti.
            - sceglie chi inizia il turno tra cpu e utente

        */

        // qui il sistema deve distribuire equamente i territori ai due giocatori
        User = new Player("UserPlayer");
        Cpu = new Player("CpuPlayer");

        // recuperiamo tutti i territori
        List<Territory> allTerritories = [];
        foreach (Territory t in _territories.Values) 
                allTerritories.Add(t);
        
        // eseguiamo uno shuffle (metodo implementato nell'extension)
        allTerritories.Shuffle();

        // 21 territori a User e 21 alla CPU
        var firstHalf = allTerritories.Take(21).ToList(); // restituisce i primi 21 elementi
        var secondHalf = allTerritories.Skip(21).ToList(); // restituisce i restanti elementi dopo i primi 21
        foreach (var ter in firstHalf)
            User.AddTerritory(ter);

        foreach (var ter in secondHalf) 
            Cpu.AddTerritory(ter);
        
        // scegliamo due colori random, i colori sono 6
        int colorForUser = gen.Next(6);
        int colorForCpu;
        do { // vogliamo assolutamente evitare che i colori siano uguali
            colorForCpu = gen.Next(6);
        } while (colorForUser == colorForCpu);
           
        // per ogni territorio del'utente, associamo 3 carri armati 
        foreach (Territory territory in User.Territories.Values){
            territory.AddTanks(colorForUser,3);
        }
        
        // per ogni territorio della CPU, associamo 3 carri armati 
        foreach (Territory territory in Cpu.Territories.Values){
            territory.AddTanks(colorForCpu,3);
        }
            
        // obiettivo ai partecipanti
        User.Goal = _goals[0];
        Cpu.Goal = _goals[0];

        // Necessario per l'inserimento di nuovi carri armati
        User.TankColor = colorForUser;
        Cpu.TankColor = colorForCpu;


        // TODO: implementare la casualità del turno (Lo facciamo solo dopo che si vede che funziona tutto)

        /*
        if (gen.Next(2)==0) 
            user.Turn = new Turn(TurnPhases.Attack);
        else 
            cpu.Turn = new Turn(TurnPhases.Attack);
        */

        User.Turn = new Turn(TurnPhases.Reinforcement);
        CurrentTurn = User.Turn;
        CurrentTurn.currentPlayer = User;
    }


    // OPERAZIONE DI SISTEMA UC6 -> mostra elenco territori confinanti "attaccabili"
    // invocata dalla pagina "TablePage"
    public List<string> AttackableTerritories(string territoryName) {
        /*
            Dato il nome di un territorio, ne restituisce quelli confinanti appartenenti all'avversario.
        */

        List<Territory> attackableNeighboringTerritories = [];
        attackableNeighboringTerritories.AddRange(_territories[territoryName].NeighboringTerritories);
        // dobbiamo restituire quelli che non appartengono all'utente
        
        if (User != null) {
            attackableNeighboringTerritories.RemoveAll(territory => User.Territories.ContainsValue(territory));
        }
        
        List<string> attackableNeighboringTerritoriesNames = [];
        foreach (var item in attackableNeighboringTerritories) {
            attackableNeighboringTerritoriesNames.Add(item.Name);
        }

        return attackableNeighboringTerritoriesNames;
    }


    // OPERAZIONE DI SISTEMA UC6 --> attacco di un territorio da parte dell'utente
    // invocata dalla pagina "AttackableTerritoriesPage"
    //TODO: questa va spostate in TablePage
    public (List<int>, List<int>, string result) AttackTerritory(string attackingTerritory, string attackedTerritory) {
        // come prima cosa, dobbiamo verificare se il territorio di partenza ha almeno 2 carri armati 
        Territory from = User!.Territories[attackingTerritory];
        int numTanksAttacker = from.Tanks.Count;
        if (numTanksAttacker > 1) { // possiamo procedere con l'attacco... 
            Territory to = Cpu!.Territories[attackedTerritory];
            int numTanksDefender = to.Tanks.Count;
            List<int> userDice;
            List<int> cpuDice;
            // lanciamo i dadi
            RollTheDice(out userDice, out cpuDice, numTanksAttacker, numTanksDefender);
            // aggiorniamo gli oggetti User e CPU (num di carri armati dei territori coinvolti)
            string result = CompareDiceAndRemoveTanks(in userDice, in cpuDice, from, to);
            // Dopo un attacco, verifichiamo se il territorio avversario è stato conquistato 
            if (true) {
                
            }
            
            return (userDice, cpuDice, result);
            
        } else { // l'utente ha solo un carro armato
            throw new Exception("Non puoi attaccare un territorio da uno in cui hai un solo carro armato!");
        }
    }

    

    private void RollTheDice(out List<int> userDice, out List<int> cpuDice, in int numTanksAttacker, in int numTanksDefender) {
        userDice = [];
        cpuDice = [];
        // simuliamo il lancio dei dadi dell'attaccante: un dado per ogni carro armato - 1
        int counter = 1;
        for (int i = 0; i < numTanksAttacker - 1; i++) {
            userDice.Add(gen.Next(6)+1); 
            if (counter == 3) break; // non si possono lanciare più di 3 dadi
            counter++;
        }
        // simuliamo il lancio dei dadi della difesa: un dado per ogni carro armato
        counter = 0;
        for (int i = 0; i < numTanksDefender; i++) {
            cpuDice.Add(gen.Next(6)+1); 
            if (counter == 3) break; // non si possono lanciare più di 3 dadi
            counter++;
        }
        // ordiniamo i dadi in modo decrescente:
        userDice.Sort((a,b) => b.CompareTo(a));
        cpuDice.Sort((a,b) => b.CompareTo(a));
    }

    private string CompareDiceAndRemoveTanks(in List<int> userDice, in List<int> cpuDice, Territory attacking, Territory defending) {
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
        return $"l'utente ha perso {counterForUser} carri armati, mentre la CPU ne ha persi {counterForCpu}";
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
            _territories["Urali"],
            _territories["Cina"],
            _territories["India"],
            _territories["MedioOriente"]
        ]);

        // 34. Cina
        _territories["Cina"].NeighboringTerritories.AddRange([
            _territories["Urali"],
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