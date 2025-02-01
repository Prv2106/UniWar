public class Continent {
    public string Name {get;}
    public int NumTerritories {get;}
    public List<Territory> Territories {get; set;}

    public Continent(string name, int numTerritories, List<Territory> territories) {
        NumTerritories = numTerritories;
        Name = name;
        Territories = territories;
    }

}