/// <summary>
/// Represents a territory, its strategic score, garrison, and neighbors.
/// </summary>
public class Territory {
    public int Score {get;}
    public string Name {get;}
    

    public List<Tank> Tanks {get;}
    public List<Territory> NeighboringTerritories {get;}


    /// <summary>
    /// Initializes a territory with an empty garrison and adjacency list.
    /// </summary>
    /// <param name="name">Territory name.</param>
    /// <param name="score">Strategic score used for final scoring.</param>
    public Territory (string name, int score) {
        Score = score;
        Name = name;
        Tanks = [];
        NeighboringTerritories = [];
    }

    /// <summary>
    /// Adds tanks of the specified color to the territory.
    /// </summary>
    /// <param name="tankColor">Numeric tank color index.</param>
    /// <param name="num">Number of tanks to add.</param>
    public void AddTanks(int tankColor, int num = 1) {
        for (int i = 0; i < num; ++i) 
            Tanks.Add(new Tank(tankColor));
    }

    /// <summary>
    /// Removes up to the specified number of tanks from the territory.
    /// No exception is thrown when the garrison is smaller than requested.
    /// </summary>
    /// <param name="num">Number of tanks to remove.</param>
    public void RemoveTanks(int num = 1){
        for (int i = 0; i < num; ++i) {
            if (Tanks.Count > 0) 
                Tanks.Remove(Tanks.Last());
            else
                break;
        }
    }


}