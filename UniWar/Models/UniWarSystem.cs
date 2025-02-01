public class UniWarSystem {
    private readonly Dictionary<string, Continent> _continents; // collezione di tutti i territori gestiti dal gioco
    private readonly Dictionary<int, Goal> _goals; // collezione di tutti gli obiettivi

    public bool IsPlaying {get; set;} // gestiamo l'associazione con la classe Player


    public UniWarSystem(Dictionary<string, Continent> continents, Dictionary<int, Goal> goals) {
        _continents = continents;
        _goals = goals;
        IsPlaying = false;
    }
}