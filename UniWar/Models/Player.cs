public class Player {
    
    static private int PlayerCount {get; set;} = 0;
    public int TankColor {get; set;}

    public string Name {get; set;} 
    public Turn? Turn {get; set;}
    public Goal? Goal {get; set;}
    public Dictionary<string, Territory> Territories {get; set;}

    public Player() {
        Name = $"Player {PlayerCount++}";
        Territories = [];
    }

    public Player(string name) : this() {
        Name = name;
    }

    public int GetNumTanks() {
        int num = 0;
        foreach (var territory in Territories.Values) 
            num += territory.Tanks.Count;
        return num;
    }
}