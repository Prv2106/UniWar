// Struttura per rappresentare una battaglia della CPU
public struct BattleResult {
    public int Battle { get; set; }
    public string AttackingPlayer { get; set; }
    public string DefendingPlayer { get; set; }
    public bool Win { get; set; }
    public List<int> DiceCPU { get; set; }
    public List<int> DicePlayer { get; set; }
    public string AttackingTerritory { get; set; }
    public string DefendingTerritory { get; set; }
    public Dictionary<string, List<string>> AttackingNeighborsMap { get; set; }
    public Dictionary<string, List<string>> DefendingNeighborsMap { get; set; }
    public Dictionary<string, int> AttackingTanksCountMap { get; set; }
    public Dictionary<string, int> DefendingTanksCountMap { get; set; }
    public int LossesCPU { get; set; }
    public int LossesPlayer { get; set; }
}
