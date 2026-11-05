/// <summary>
/// Describes the objective assigned to a player.
/// </summary>
public class Goal {
    public string Description {get;}
    
    /// <summary>
    /// Initializes a goal.
    /// </summary>
    /// <param name="descr">Human-readable goal description.</param>
    public Goal(string descr) {
        Description = descr;
    }
}