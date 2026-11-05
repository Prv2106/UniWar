/// <summary>
/// Tracks the active player, phase, and round number.
/// </summary>
public class Turn {
    public TurnPhases Phase {get; set;}

    public int IdRound {get; set;}
   
    public Player CurrentPlayer {get; set;}
    
    /// <summary>
    /// Initializes a turn for the specified player in the reinforcement phase.
    /// </summary>
    /// <param name="player">Player who starts the turn.</param>
    public Turn(Player player) {
        Phase = TurnPhases.Reinforcement;
        IdRound = 1;
        CurrentPlayer = player;
    }
}