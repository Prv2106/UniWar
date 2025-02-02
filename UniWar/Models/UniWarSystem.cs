public class UniWarSystem { // singleton
    private static UniWarSystem? _instance;
    private readonly Dictionary<string, Continent> _continents; // collezione di tutti i territori gestiti dal gioco
    private readonly Dictionary<int, Goal> _goals; // collezione di tutti gli obiettivi
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
        _goals = new Dictionary<int, Goal>();
        _players = [];
        InitializeAll();
    }

    private void InitializeAll() {
        // qui gestiamo la creazione dei territori ecc..
        _continents.Add("AmericaDelNord", new Continent("AmericaDelNord", 9, new List<Territory>() {
            new Territory("Alaska",2),
            new Territory("Alberta",2),
            new Territory("StatiUnitiOccidentali",3),
            new Territory("StatiUnitiOrientali",3),
            new Territory("AmericaCentrale",3),
            new Territory("Ontario",3),
            new Territory("TerritoriDelNordOvest",3),
            new Territory("Groenlandia",3),
            new Territory("Quebec",3),
        }));
        _continents.Add("AmericaDelSud", new Continent("AmericaDelSud", 4, new List<Territory>() {
            new Territory("Venezuela",3),
            new Territory("Perù",3),
            new Territory("Brasile",4),
            new Territory("Argentina",2),
        }));
        _continents.Add("Africa", new Continent("Africa", 6, new List<Territory>() {
            new Territory("AfricaDelNord",3),
            new Territory("Egitto",3),
            new Territory("AfricaOrientale",4),
            new Territory("Congo",2),
            new Territory("AfricaDelSud",2),
            new Territory("Madagascar",2),
        }));
        _continents.Add("Asia", new Continent("Asia", 12, new List<Territory>() {
            new Territory("Kamchatka",3),
            new Territory("Jacuzia",3),
            new Territory("Cita",4),
            new Territory("Giappone",2),
            new Territory("Cina",2),
            new Territory("Siam",2),
            new Territory("India",3),
            new Territory("MedioOriente",3),
            new Territory("Afghanistan",4),
            new Territory("Urali",2),
            new Territory("Siberia",2),
            new Territory("Mongolia",2),
        }));
         _continents.Add("Europa", new Continent("Africa", 7, new List<Territory>() {
            new Territory("EurpoaOccidentale",3),
            new Territory("EuropaMeridionale",3),
            new Territory("GranBretagna",4),
            new Territory("Islanda",2),
            new Territory("Ucraina",2),
            new Territory("EuropaSettentrionale",2),
            new Territory("Scandinavia",2),
        }));
        _continents.Add("Oceania", new Continent("Oceania", 4, new List<Territory>() {
            new Territory("AustraliaOccidentale",3),
            new Territory("NuovaGuinea",3),
            new Territory("Indonesia",4),
            new Territory("AustraliaOrientale",2),
        }));


       
    }

    public (Player, Player) InitializeGame() {
        /* 
            è, sostanzialmente, il metodo che inizializza la partita...
            - sceglie un colore per i carri armati per i partecipanti (l’utente e il sistema stesso… in maniera random tra i colori previsti dal risiko).
            - “mischia” le “carte” dei territori e le distribuisce equamente tra i partecipanti.
            - assegna un obiettivo ai partecipanti.

        */

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

        // eseguiamo uno shuffle
        allTerritories.Shuffle();

        // 21 territori a User e 21 alla CPU
        var firstHalf = allTerritories.Take(21).ToList();
        var secondHalf = allTerritories.Skip(21).ToList();
        user.Territories = firstHalf;
        cpu.Territories = secondHalf;

        // per ogni territorio del'utente, associamo 3 carri armati verdi
        foreach (Territory territory in user.Territories)
            territory.Tanks.AddRange([new Tank(TankColors.Yellow), new Tank(TankColors.Yellow), new Tank(TankColors.Yellow)]);
        
        // per ogni territorio della CPU, associamo 3 carri armati viola
        foreach (Territory territory in user.Territories)
            territory.Tanks.AddRange([new Tank(TankColors.Red), new Tank(TankColors.Red), new Tank(TankColors.Red)]);
        
        return (user, cpu);
    }
}