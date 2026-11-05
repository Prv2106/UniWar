/// <summary>
/// Represents a continent and the territories it contains.
/// </summary>
public class Continent {
    public string Name {get;}
    public int NumTerritories {get;}
    public List<Territory> Territories {get;}

    /// <summary>
    /// Initializes a continent.
    /// </summary>
    /// <param name="name">Continent name.</param>
    /// <param name="numTerritories">Number of territories in the continent.</param>
    /// <param name="territories">Territories belonging to the continent.</param>
    public Continent(string name, int numTerritories, List<Territory> territories) {
        NumTerritories = numTerritories;
        Name = name;
        Territories = territories;
    }

}