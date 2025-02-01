public class Player {
    public Turn Turn {get; set;}
    public Goal Goal {get;}
    public List<Tank> Tanks {get;} // anche se c'è l'associazione con i Territori, è utile per il conteggio
    public Dictionary<string, Territory> Territories {get;}

    public Player() {
            
    }
}