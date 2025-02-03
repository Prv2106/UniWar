#include "functions_lib.h"
#include "uniwar_lib.h"
#include "test.h"
using json = nlohmann::json;
using namespace std;




static string jsonResult; // Variabile statica per evitare memory leak



// Funzione per il rinforzo dei territori della CPU
const char* reinforcement (const char* jsonData, int newTanks){
    vector<uniwar::Player> players = uniwar::initializePlayers(jsonData);
    if (players.empty()) 
        return "Nessun Contesto fornito";

    uniwar::Player& cpuPlayer = players[0]; 

    set<string> ownedFrontiers = uniwar::getOwnedFrontier(cpuPlayer.getNeighbors());
    cout << "Nuovi carri a disposizione: " << newTanks << endl;
    for(auto & territory: ownedFrontiers){
        int tankCount = cpuPlayer.getTanksCount(territory);
        cout << "numero carri (Prima della modifica) per territorio " << territory << ": " << tankCount << endl;
        if(tankCount== 1 && newTanks > 0){
            cpuPlayer.modifyTankCount(territory, tankCount + 1);
            newTanks--;
            cout << "numero carri (Dopo la modifica) per territorio " << territory << ": " << cpuPlayer.getTanksCount(territory) << endl;
            cout << "Nuovi carri a disposizione: " << newTanks << endl;
        }
        if(newTanks == 0){
            printTestRinforzo(cpuPlayer);
            break;
        }

    }

    while(newTanks > 0){
        for(auto & territory: ownedFrontiers){
        int tankCount = cpuPlayer.getTanksCount(territory);

        cout << "numero carri (Prima della modifica) per territorio " << territory << ": " << tankCount << endl;
        cpuPlayer.modifyTankCount(territory, tankCount + 1);

        newTanks--;
        cout << "numero carri (Dopo la modifica) per territorio " << territory << ": " << cpuPlayer.getTanksCount(territory) << endl;
        cout << "Nuovi carri a disposizione: " << newTanks << endl;

        if(newTanks == 0){
            printTestRinforzo(cpuPlayer);
            break;
        }
            
        }
    }

    // Una volta aggiornato il contesto restituiamo il risultato a C# andando a creare il nuovo json

    // Creiamo un oggetto json e inseriamo i campi
    json updatedPlayerData; 

    updatedPlayerData["PlayerId"] = cpuPlayer.getName();
    updatedPlayerData["Neighbors"] = cpuPlayer.getNeighbors();
    updatedPlayerData["Tanks"] = cpuPlayer.getTanks();


    // Convertiamo l'oggetto JSON in una stringa JSON
    jsonResult = json(updatedPlayerData).dump();  // Serializziamo i dati in formato stringa

    // La funzione c_str() della classe std::string restituisce un puntatore a carattere (const char*)
    return jsonResult.c_str();

}









/************************* TEST  *************************/

/*  La funzione di rinforzo segue questa logica: il cpu player assegna i nuovi carri armati tramite un roundrobin a ciascuno dei suoi territori di frontiera.
    In particolare, da precedenza ai territori di frontiera con 1 solo carro armato per poi applicare un round robin su tutti.
    - per prima cosa si recuperano i territori di frontiera posseduti
    - a questo punto si fa un primo ciclo dove si assegna a tali territori 1 carro armato alla volta 
      (prima solamente a quelli che hanno soltanto 1 carro armato e poi se restano altri carri da assegnare si assegnano con un round robin)

*/
void testRinforzi(int newTanks){
    cout << "TEST RINFORZO\n" << endl; 
    uniwar::Player cpuPlayer(cpuPlayerNeighborsMap, cpuPlayerTankCountMap, "cpuPlayer");
    set<string> ownedFrontiers = uniwar::getOwnedFrontier(cpuPlayer.getNeighbors());
    cout << "Nuovi carri a disposizione: " << newTanks << endl;
    for(auto & territory: ownedFrontiers){
        int tankCount = cpuPlayer.getTanksCount(territory);
        cout << "numero carri (Prima della modifica) per territorio " << territory << ": " << tankCount << endl;
        if(tankCount== 1 && newTanks > 0){
            cpuPlayer.modifyTankCount(territory, tankCount + 1);
            newTanks--;
            cout << "numero carri (Dopo la modifica) per territorio " << territory << ": " << cpuPlayer.getTanksCount(territory) << endl;
            cout << "Nuovi carri a disposizione: " << newTanks << endl;
        }
        if(newTanks == 0){
            printTestRinforzo(cpuPlayer);
            return;
        }

    }

    while(newTanks > 0){
        for(auto & territory: ownedFrontiers){
        int tankCount = cpuPlayer.getTanksCount(territory);

        cout << "numero carri (Prima della modifica) per territorio " << territory << ": " << tankCount << endl;
        cpuPlayer.modifyTankCount(territory, tankCount + 1);

        newTanks--;
        cout << "numero carri (Dopo la modifica) per territorio " << territory << ": " << cpuPlayer.getTanksCount(territory) << endl;
        cout << "Nuovi carri a disposizione: " << newTanks << endl;

        if(newTanks == 0){
            printTestRinforzo(cpuPlayer);
            return;
        }
            
        }
    }

}


void printTestRinforzo(const uniwar::Player & cpuPlayer){
    for(const auto & territory: cpuPlayer.getTanks()){
        cout << "\n " << territory.first << ": " << cpuPlayer.getTanksCount(territory.first) << "\n" << endl;
    }
}


void testWin(){
    cout << "Prova 1 (dovrebbe essere true)\n" << endl;
    cout << uniwar::win(uniwar::getTerritoriesFromMap(winnerMap1)) << "\n" << endl;
    cout << "Prova 1 (dovrebbe essere true)\n" << endl;
    cout << uniwar::win(uniwar::getTerritoriesFromMap(WinnerMap2)) << "\n" << endl;

    cout << "Prova 2 (dovrebbe essere false)\n" << endl;
    cout << uniwar::win(uniwar::getTerritoriesFromMap(neighborsMapIncomplete)) << "\n" << endl;
    
    cout << "Prova 2 (dovrebbe essere false)\n" << endl;
    cout << uniwar::win(uniwar::getTerritoriesFromMap(tanksMapIncomplete)) << "\n" << endl;
}

void testFrontiers(){
    cout << "Test frontiere NON POSSEDUTE: (risultato atteso -> {\"Perù\", \"AmericaCentrale\", \"AfricaDelNord\"}) \n" << endl;
    
    set<string> result = uniwar::getNotOwnedFrontier(frontierTestMap);

    
    cout << "{";
    for (auto it = result.begin(); it != result.end(); ++it) {
        cout << "\"" << *it << "\"";
        if (next(it) != result.end()) {
            cout << ", ";  
        }
    }
    cout << "}" << endl;

    cout << "Test frontiere POSSEDUTE: (risultato atteso -> {\"Ontario\", \"Groenlandia\"}) \n" << endl;

    result = uniwar::getOwnedFrontier(cpuPlayerNeighborsMap);

    cout << "{";
    for (auto it = result.begin(); it != result.end(); ++it) {
        cout << "\"" << *it << "\"";
        if (next(it) != result.end()) {
            cout << ", ";  
        }
    }
    cout << "}" << endl;



}

