public class Territory {
    public int Score {get;}
    public string Name {get;}
    

    // sotto gestiamo le associazioni e le visibilità per parametro
    public List<Tank> Tanks {get;}
    public Dictionary<string, Territory> NeighboringTerritories {get;}


    public Territory (string name, int score) {
        Score = score;
        Name = name;
        Tanks = [];
        NeighboringTerritories = [];
    }

    public string NameWithSpaces() {
        return Name;
    }

    
}