public class Player {
    public int TankColor {get; set;}

    public string Name {get; set;} 
    public Goal? Goal {get; set;}
    public Dictionary<string, Territory> Territories {get; set;}

    public Player(string name) {
        Name = name;
        Territories = [];
    }

    public int GetNumTanks {
        get {
            int num = 0;
            foreach (var territory in Territories.Values) 
                num += territory.Tanks.Count;
            return num;
        }
    }



    public void AddTerritory(Territory territory) {
        Territories.Add(territory.Name, territory);
    }

    public void RemoveTerritory(Territory territory) {
        Territories.Remove(territory.Name);
    }
    
    
}