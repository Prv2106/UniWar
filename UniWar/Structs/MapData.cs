
public struct MapData {
    public string PlayerId {get; set;}
    public Dictionary<string, List<string>> Neighbors {get; set;}

    public Dictionary<string, int> Tanks {get; set;}

}