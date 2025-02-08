public class Round {
    /*
        L'idea è quella di avere uno storico dei turni avvenuti...
        In particolare, si potrebbe Aavere un dictionary con tante chiavi (int) quanti sono 
        stati i "giri" totali; ad ogni giro si avranno 2 valori user e cpu e, 
        per ciascuna player, una lista di Turni che rappresentano lo storico di quanto accaduto
        nell'intera partita
    */
    public int Id {get; set;}

    public List<Turn> Turns {get; set;}

    public Round(int id) {
        Id = id;
        Turns = [];
    }
}