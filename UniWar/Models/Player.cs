public class Player {
    public Turn? Turn {get; set;}
    public Goal? Goal {get; set;}
    public List<Territory> Territories {get; set;}

    public bool IsCPU {get;} 

    public Player(bool isCpu) {
        Territories = [];
        IsCPU = isCpu;
    }
}