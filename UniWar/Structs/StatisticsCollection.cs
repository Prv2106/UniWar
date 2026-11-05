/// <summary>
/// Aggregated statistics sent to the gRPC server after a battle phase.
/// </summary>
public struct StatisticsCollection {
    
    public string Username {get; set;}
    public int RoundId {get; set;}
    public int GameId {get; set;}

    public bool UserTurn {get; set;} 

    public int UserOwnedTanks {get; set;}
    public int CpuOwnedTanks {get; set;}

    /// <summary>Tanks lost per attacking territory.</summary>
    public Dictionary<string, int> AttackingTerritories {get; set;}

    /// <summary>Tanks lost per defending territory.</summary>
    public Dictionary<string, int> DefendingTerritories {get; set;} 

    public List<string>? LostTerritories {get; set;}
    public List<string> UserOwnedTerritories {get; set;}
    public List<string> CpuOwnedTerritories {get; set;}

}