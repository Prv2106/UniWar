public class Turn {
    /*
        Utile per determinare la fine della fase di attacco,
    */
    public TurnPhases Phase {get; set;}
    public int IdRound {get; set;}
    /* potremmo usare questo attributo per implementare la logica del giro completato 
    (incrementandola man mano che tutti hanno eseguito il turno)*/


    public Turn(TurnPhases phase) {
        Phase = phase;
    }
}