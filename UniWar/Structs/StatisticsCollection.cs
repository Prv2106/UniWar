public struct StatisticsCollection{
    
    public string PlayerId {get; set;}

    public int RoundId {get; set;}

    public bool UserTurn {get; set;} 

    public Dictionary<string, int> AttackedTerritories {get; set;} // {"territorio": numero_carri_persi}



}