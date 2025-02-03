public class Player {
    public Turn? Turn {get; set;}
    public Goal? Goal {get; set;}
    public List<Territory> Territories {get; set;}

    public bool IsCPU {get;} 

    public Player(bool isCpu) {
        Territories = [];
        IsCPU = isCpu;
    }

    public int GetNumTanks() {
        int num = 0;
        foreach (var territory in Territories) 
            num += territory.Tanks.Count();
        return num;
    }
}