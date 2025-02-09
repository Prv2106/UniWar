public class Turn {
    public TurnPhases Phase {get; set;}

    /* potremmo usare questo attributo per implementare la logica del giro completato 
    (incrementandola man mano che tutti hanno eseguito il turno)*/
    public int IdRound {get; set;}
   
    public Player currentPlayer {get; set;}
    
    public Turn(Player player) {
        Phase = TurnPhases.Reinforcement;
        IdRound = 1;
        currentPlayer = player;
    }
}