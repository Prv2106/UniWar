public class Round {
    public int Id {get; set;}

    public List<Turn> Turns {get; set;}

    public Round(int id) {
        Id = id;
        Turns = [];
    }
}