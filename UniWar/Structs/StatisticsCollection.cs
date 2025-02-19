public struct StatisticsCollection {
    
    public string PlayerId {get; set;}
    public int RoundId {get; set;}
    public int GameId {get; set;}

    public bool UserTurn {get; set;} 

    public int UserOwnedTanks {get; set;}
    public int CpuOwnedTanks {get; set;}

    public Dictionary<string, int> AttackingTerritories {get; set;} // {"territorio": numero_carri_persi}

    public Dictionary<string, int> DefendingTerritories {get; set;} 

    public List<string>? LostTerritories {get; set;}
    public List<string> UserOwnedTerritories {get; set;}
    public List<string> CpuOwnedTerritories {get; set;}

}