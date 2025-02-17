public struct StatisticsCollection {
    
    public string PlayerId {get; set;}

    public int RoundId {get; set;}

    public bool UserTurn {get; set;} 
    public bool? UserWin {get; set;} 

    public int OwnedTanks {get; set;}

    public Dictionary<string, int> AttackingTerritories {get; set;} // {"territorio": numero_carri_persi}

    public Dictionary<string, int> DefendingTerritories {get; set;} 

    public List<string>? LostTerritories {get; set;}
    public List<string> OwnedTerritories {get; set;}

}