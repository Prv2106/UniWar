#include "functions_lib.h"

// CODICE PER TESTARE LA CONDIZIONE DI VITTORIA

map<string, vector<string>> winnerMap1 = {
    {"Alaska", {"Alberta", "TerritoriDelNordOvest"}},
    {"Alberta", {"Alaska", "StatiUnitiOccidentali"}},
    {"StatiUnitiOccidentali", {"Alberta", "StatiUnitiOrientali"}},
    {"StatiUnitiOrientali", {"StatiUnitiOccidentali", "Ontario"}},
    {"AmericaCentrale", {"StatiUnitiOrientali", "Ontario"}},
    {"Ontario", {"StatiUnitiOrientali", "AmericaCentrale", "TerritoriDelNordOvest", "Groenlandia", "Quebec"}},
    {"TerritoriDelNordOvest", {"Ontario", "Alaska", "Groenlandia"}},
    {"Groenlandia", {"Ontario", "TerritoriDelNordOvest", "Quebec"}},
    {"Quebec", {"Ontario", "Groenlandia", "StatiUnitiOrientali"}},
    {"Venezuela", {"Perù", "Brasile", "Argentina"}},
    {"Perù", {"Venezuela", "Brasile", "Argentina"}},
    {"Brasile", {"Venezuela", "Perù", "Argentina", "Congo", "AfricaDelSud"}},
    {"Argentina", {"Venezuela", "Perù", "Brasile", "Congo"}},
    {"AfricaDelNord", {"Egitto", "AfricaOrientale", "Congo"}},
    {"Egitto", {"AfricaDelNord", "AfricaOrientale", "MedioOriente"}},
    {"AfricaOrientale", {"AfricaDelNord", "Egitto", "Congo", "Madagascar"}},
    {"Congo", {"AfricaDelNord", "Egitto", "Brasile", "AfricaOrientale", "AfricaDelSud"}},
    {"AfricaDelSud", {"Congo", "Brasile", "Madagascar"}},
    {"Madagascar", {"AfricaDelSud", "AfricaOrientale"}},
    {"Kamchatka", {"Jacuzia", "Giappone", "Cina"}},
    {"Jacuzia", {"Kamchatka", "Siberia"}},
    {"Cita", {"Giappone", "Cina", "Siam", "India"}},
    {"Giappone", {"Kamchatka", "Cita", "Cina"}},
    {"Cina", {"Giappone", "Kamchatka", "Siam", "India", "Urali"}},
    {"Siam", {"Cina", "India", "MedioOriente"}},
    {"India", {"Siam", "Cina", "MedioOriente", "Afghanistan"}},
    {"MedioOriente", {"India", "Cina", "Siam", "Afghanistan", "Urali", "AfricaOrientale"}},
    {"Afghanistan", {"India", "MedioOriente", "Urali", "Siberia"}},
    {"Urali", {"Cina", "MedioOriente", "Afghanistan", "Siberia", "Mongolia"}},
    {"Siberia", {"Jacuzia", "Afghanistan", "Urali", "Mongolia", "Mongolia"}},
    {"Mongolia", {"Urali", "Siberia", "Cina"}}
};

map<string, int> WinnerMap2 = {
    {"Alaska", 5},
    {"Alberta", 3},
    {"StatiUnitiOccidentali", 6},
    {"StatiUnitiOrientali", 4},
    {"AmericaCentrale", 2},
    {"Ontario", 7},
    {"TerritoriDelNordOvest", 3},
    {"Groenlandia", 5},
    {"Quebec", 4},
    {"Venezuela", 3},
    {"Perù", 2},
    {"Brasile", 4},
    {"Argentina", 3},
    {"AfricaDelNord", 5},
    {"Egitto", 2},
    {"AfricaOrientale", 3},
    {"Congo", 6},
    {"AfricaDelSud", 3},
    {"Madagascar", 2},
    {"Kamchatka", 4},
    {"Jacuzia", 3},
    {"Cita", 4},
    {"Giappone", 5},
    {"Cina", 7},
    {"Siam", 3},
    {"India", 6},
    {"MedioOriente", 4},
    {"Afghanistan", 3},
    {"Urali", 6},
    {"Siberia", 3},
    {"Mongolia", 4}
};



map<string, vector<string>> neighborsMapIncomplete = {
    {"Alaska", {"Alberta", "TerritoriDelNordOvest"}},
    {"Alberta", {"Alaska", "StatiUnitiOccidentali"}},
    {"StatiUnitiOccidentali", {"Alberta", "StatiUnitiOrientali"}},
    {"StatiUnitiOrientali", {"StatiUnitiOccidentali", "Ontario"}},
    {"AmericaCentrale", {"StatiUnitiOrientali", "Ontario"}},
    {"Ontario", {"StatiUnitiOrientali", "AmericaCentrale", "TerritoriDelNordOvest", "Groenlandia", "Quebec"}},
    {"TerritoriDelNordOvest", {"Ontario", "Alaska", "Groenlandia"}},
    {"Groenlandia", {"Ontario", "TerritoriDelNordOvest", "Quebec"}},
    {"Quebec", {"Ontario", "Groenlandia", "StatiUnitiOrientali"}},
    {"Venezuela", {"Perù", "Brasile", "Argentina"}},
    {"Perù", {"Venezuela", "Brasile", "Argentina"}},
    {"Brasile", {"Venezuela", "Perù", "Argentina", "Congo", "AfricaDelSud"}},
    {"Argentina", {"Venezuela", "Perù", "Brasile", "Congo"}},
    {"Congo", {"Brasile", "AfricaDelSud", "AfricaOrientale", "AfricaDelNord"}},
    {"AfricaDelSud", {"Congo", "Brasile", "Madagascar"}},
    {"Madagascar", {"AfricaDelSud", "AfricaOrientale"}},
    {"Kamchatka", {"Jacuzia", "Giappone", "Cina"}},
    {"Jacuzia", {"Kamchatka", "Siberia"}},
    {"Cina", {"Giappone", "Kamchatka", "Siam", "India", "Urali"}},
    {"Siam", {"Cina", "India", "MedioOriente"}},
    {"India", {"Siam", "Cina", "MedioOriente", "Afghanistan"}},
    {"MedioOriente", {"India", "Cina", "Siam", "Afghanistan", "Urali", "AfricaOrientale"}},
    {"Afghanistan", {"India", "MedioOriente", "Urali", "Siberia"}},
    {"Urali", {"Cina", "MedioOriente", "Afghanistan", "Siberia", "Mongolia"}},
    {"Siberia", {"Jacuzia", "Afghanistan", "Urali", "Mongolia"}},
    {"Mongolia", {"Urali", "Siberia", "Cina"}},
    {"GranBretagna", {"EuropaOccidentale", "EuropaMeridionale", "EuropaSettentrionale"}},
    {"EuropaSettentrionale", {"GranBretagna", "EuropaOccidentale", "Islanda", "Scandinavia"}},
    {"Islanda", {"GranBretagna", "EuropaSettentrionale"}}
};

map<string, int> tanksMapIncomplete = {
    {"Alaska", 3},
    {"Alberta", 2},
    {"StatiUnitiOccidentali", 4},
    {"StatiUnitiOrientali", 5},
    {"AmericaCentrale", 2},
    {"Ontario", 7},
    {"TerritoriDelNordOvest", 3},
    {"Groenlandia", 5},
    {"Quebec", 4},
    {"Venezuela", 3},
    {"Perù", 2},
    {"Brasile", 4},
    {"Argentina", 3},
    {"Congo", 6},
    {"AfricaDelSud", 3},
    {"Madagascar", 2},
    {"Kamchatka", 4},
    {"Jacuzia", 3},
    {"Cina", 7},
    {"Siam", 3},
    {"India", 6},
    {"MedioOriente", 4},
    {"Afghanistan", 3},
    {"Urali", 6},
    {"Siberia", 3},
    {"Mongolia", 4},
    {"GranBretagna", 2},
    {"EuropaSettentrionale", 5},
    {"Islanda", 3}
};


//  MAPPE PER TESTARE LA FRONTIERA  

// La frontiera dei territori non posseduti per questa mappa è costituita da:
// {"Perù", "AmericaCentrale", "AfricaDelNord"}
map<string,vector<string>> frontierTestMap = {
    {"Venezuela", {"Brasile", "Perù", "AmericaCentrale"}},
    {"Brasile", {"Perù", "Venezuela", "AfricaDelNord"}}
};




// TEST PER LA FASE DI RINFORZO


// In questo caso la frontiera dei territori posseduti è costituita da:
// {"Ontario", "Groenlandia"}
map<string, vector<string>> cpuPlayerNeighborsMap = {
    {"Alaska", {"Alberta", "TerritoriDelNordOvest"}},
    {"Alberta", {"Alaska", "StatiUnitiOccidentali"}},
    {"StatiUnitiOccidentali", {"Alberta", "StatiUnitiOrientali"}},
    {"StatiUnitiOrientali", {"StatiUnitiOccidentali", "Ontario"}},
    {"AmericaCentrale", {"StatiUnitiOrientali", "Ontario"}},
    {"Ontario", {"StatiUnitiOrientali", "AmericaCentrale", "TerritoriDelNordOvest", "Groenlandia", "Quebec"}},
    {"TerritoriDelNordOvest", {"Ontario", "Alaska", "Groenlandia"}},
    {"Groenlandia", {"Ontario", "TerritoriDelNordOvest", "Quebec"}},
    
};

map<string, int> cpuPlayerTankCountMap = {
    {"Alaska", 3},
    {"Alberta", 2},
    {"StatiUnitiOccidentali", 4},
    {"StatiUnitiOrientali", 5},
    {"AmericaCentrale", 2},
    {"Ontario", 7},
    {"TerritoriDelNordOvest", 3},
    {"Groenlandia", 5},

   
};
