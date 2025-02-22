public class Turn {
    public TurnPhases Phase {get; set;}

    public int IdRound {get; set;}
   
    public Player CurrentPlayer {get; set;}
    
    public Turn(Player player) {
        Phase = TurnPhases.Reinforcement;
        IdRound = 1;
        CurrentPlayer = player;
    }
}