public class Territory {
    public int Score {get;}
    public string Name {get;}
    

    // sotto gestiamo le associazioni e le visibilità per parametro
    public List<Tank> Tanks {get;}
    public List<Territory> NeighboringTerritories {get;}


    public Territory (string name, int score) {
        Score = score;
        Name = name;
        Tanks = [];
        NeighboringTerritories = [];
    }

    public string NameWithSpaces() {
        return Name;
    }


    
    public void AddTanks(int tankColor, int num = 1){
            for(int i=0; i<num; ++i){
                Tanks.Add(new Tank(tankColor));
            }
    }
    public void RemoveTanks(int num = 1){
            for(int i=0; i<num; ++i){
                if(Tanks.Count > 0){
                    Tanks.Remove(Tanks.Last());
                }
                else{
                    break;
                }
            }
    }




}