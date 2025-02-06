public class Turn {
    public TurnPhases Phase {get; set;}
    public int IdRound {get; set;}
    /* potremmo usare questo attributo per implementare la logica del giro completato 
    (incrementandola man mano che tutti hanno eseguito il turno)*/


    public int NumTanksToAddInReinforcementPhase {get; set;} = 0;
    
    public Player? currentPlayer {get; set;}
    
    public Turn(TurnPhases phase) {
        Phase = phase;
    }
}