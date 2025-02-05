public class Player {
    public Turn? Turn {get; set;}
    public Goal? Goal {get; set;}
    public Dictionary<string, Territory> Territories {get; set;}

    public Player() {
        Territories = [];
    }

    public int GetNumTanks() {
        int num = 0;
        foreach (var territory in Territories.Values) 
            num += territory.Tanks.Count;
        return num;
    }
}