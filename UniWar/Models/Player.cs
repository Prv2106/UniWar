/// <summary>
/// Represents a participant and the territories it controls.
/// </summary>
public class Player {
    public int TankColor {get; set;}

    public string Name {get; set;} 
    public Goal? Goal {get; set;}
    public Dictionary<string, Territory> Territories {get; set;}

    /// <summary>
    /// Initializes a player with an empty territory collection.
    /// </summary>
    /// <param name="name">Player name.</param>
    public Player(string name) {
        Name = name;
        Territories = [];
    }

    /// <summary>
    /// Gets the total number of tanks across all owned territories.
    /// </summary>
    public int GetNumTanks {
        get {
            int num = 0;
            foreach (var territory in Territories.Values) 
                num += territory.Tanks.Count;
            return num;
        }
    }



    /// <summary>
    /// Adds a territory to the player's collection.
    /// </summary>
    /// <param name="territory">Territory to add.</param>
    public void AddTerritory(Territory territory) {
        Territories.Add(territory.Name, territory);
    }

    /// <summary>
    /// Removes a territory from the player's collection.
    /// </summary>
    /// <param name="territory">Territory to remove.</param>
    public void RemoveTerritory(Territory territory) {
        Territories.Remove(territory.Name);
    }
    
    
}