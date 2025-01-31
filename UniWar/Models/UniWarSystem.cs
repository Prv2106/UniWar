public class UniWarSystem {
    private readonly Dictionary<string, Territory> _territories; // collezione di tutti i territori gestiti dal gioco
    private readonly Dictionary<int, Goal> _goals; // collezione di tutti gli obiettivi

    public bool IsPlaying {get; set;} // gestiamo l'associazione con la classe Player


    public UniWarSystem(Dictionary<string, Territory> territories, Dictionary<int, Goal> goals) {
        _territories = territories;
        _goals = goals;
        IsPlaying = false;
    }
}