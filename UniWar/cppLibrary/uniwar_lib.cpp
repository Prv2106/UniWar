#include "uniwar_lib.hpp"
#include "functions_lib.hpp"
using json = nlohmann::json;

using namespace std;

namespace uniwar{

/******************************************** Implementazione di Player *************************************/

// Costruttore (con lista di inizializzazione)
Player::Player(const map<string, vector<string>> & neighbors, const  map<string, int> & tanks, const string & player): neighborsMap(neighbors), tankCountMap(tanks), playerId(player) {}

const  map<string, vector<string>>& Player::getNeighborsMap() const{
    return neighborsMap;
}

// Getter della lista di vicini per un dato territorio
const vector<string> & Player::getNeighbors(const string& territory){
    static const vector<string> emptyVector;  // Evita dangling reference

    if((territory.empty()) || (tankCountMap.find(territory) == tankCountMap.end() || neighborsMap.find(territory) == neighborsMap.end())){
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste.");
        return emptyVector;
    }
    return neighborsMap.at(territory);

}

const  map<string, int> & Player::getTanksMap() const {
    return tankCountMap;
}

const string & Player::getName() const{
    return playerId;
}

// Metodo per aggiungere un territorio e i suoi vicini
void Player::addTerritory(const string & territory, const vector<string> & neighbors, int count){
    if(territory.empty() || neighbors.empty()){
        throw std::invalid_argument("Errore: il territorio {" + territory + "} o la lista dei vicini è vuota.");
        return;
    }

    // verifichiamo se esiste già una chiave per il territorio indicato
    /* Nota:
        In una std::map, il metodo .find(chiave) restituisce un iteratore che punta all'elemento corrispondente alla chiave, se questa esiste.
        Se la chiave non esiste, restituisce end(), che rappresenta la fine della mappa.
    */
    if((neighborsMap.find(territory) == neighborsMap.end())){ // la chiave non esiste
        neighborsMap[territory] = neighbors; // crea una nuova entry
    }
    else{
        throw std::runtime_error("Errore: il territorio {" + territory + "} è già presente in tankCountMap.");
    }
    
    if(tankCountMap.find(territory) == tankCountMap.end()){ // la chiave non esiste
        tankCountMap[territory] = count; // crea una nuova entry
    }
    else{
        throw std::runtime_error("Errore: il territorio {" + territory + "} è già presente in tankCountMap.");
    }

}

// Metodo per modificare il numero di carri armati di un territorio
void Player::modifyTankCount(const string & territory, int newCount){
    if((territory.empty()) || (tankCountMap.find(territory) == tankCountMap.end())){
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste.");
        return;
    }
    else{
        tankCountMap[territory] = newCount; 
    }

}

// Metodo per rimuovere un territorio
void Player::removeTerritory(const string & territory){
    if((territory.empty()) || (tankCountMap.find(territory) == tankCountMap.end() || neighborsMap.find(territory) == neighborsMap.end())){
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste o è già stato rimosso.");
        return;
    }
    neighborsMap.erase(territory);
    tankCountMap.erase(territory);
}

// Metodo per ottenere il numero di carri armati dato il territorio
const int Player::getTanksCount(const string &territory) const {
    if ((territory.empty()) || (neighborsMap.find(territory) == neighborsMap.end()) || (tankCountMap.find(territory) == tankCountMap.end())) {
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste.");
        return -1;  
    }
    
    return tankCountMap.at(territory);  // .at() è un metodo degli std::map (e anche di std::vector, std::unordered_map, ecc.) che permette di accedere a un elemento in modo sicuro, lanciando un'eccezione se la chiave non esiste.
}

/****** Fine implementazione di Player */



// Questa mappa viene utilizzata dalla funzione "win" per verificare se il giocatore ha vinto
const map<string, vector<string>> continents = {

    {"AmericaDelNord", {"Alaska", "Alberta", "StatiUnitiOccidentali", "StatiUnitiOrientali","AmericaCentrale", "Ontario", "TerritoriDelNordOvest", "Groenlandia", "Quebec"}},
    {"AmericaDelSud", {"Venezuela", "Perù", "Brasile", "Argentina"}},
    {"Africa", {"AfricaDelNord", "Egitto", "AfricaOrientale", "Congo", "AfricaDelSud", "Madagascar"}},
    {"Asia", {"Kamchatka", "Jacuzia", "Cita", "Giappone", "Cina", "Siam", "India", "MedioOriente","Afghanistan", "Urali", "Siberia", "Mongolia"}},
    {"Europa", {"EuropaOccidentale", "EuropaMeridionale", "GranBretagna", "Islanda", "Ucraina", "EuropaSettentrionale", "Scandinavia"}},
    {"Oceania", {"AustraliaOccidentale", "NuovaGuinea", "Indonesia", "AustraliaOrientale"}}

};


/* Funzioni di supporto */

// Funzione che inizializza il contesto di gioco per i giocatori in base al json ottenuto da C#
vector<Player> initializePlayers(const char* jsonData){
    try {
        // usiamo la funzione parse() della libreria nlohmann/json per convertire la stringa JSON in un oggetto JSON che rappresenta un array JSON.
        json parsedData = json::parse(jsonData);

        /*
            Adesso parsedData contiene un oggetto json con la seguente struttura: 
            [
                { "PlayerId": "Player1", "Tanks": {"A": 3, "B": 2}, "Neighbors": {"A": ["B", "C"], "B": ["A"]} },
                { "PlayerId": "Player2", "Tanks": {"C": 4}, "Neighbors": {"C": ["A"]} }
            ]
        */

        // Creiamo un vettore di oggetti Player
        vector<Player> players;
         
        // Cicliamo su ogni giocatore nel JSON
        // player è un riferimento ad un oggetto json (auto permette di determinare il tipo dinamicamente)
        for (auto& player : parsedData) {
            // verifichiamo l'esistenza delle chiavi "Neighbors" e "Tanks" per ogni giocatore
            if (!(player.contains("PlayerId") && player.contains("Neighbors") && player.contains("Tanks"))) {
                cerr << "Errore: JSON malformato" << endl;
                continue;
            }
        
            // Sfruttiamo la funzione membro "get" per convertire i valori dell'oggetto json in tipi di dati standard di C++.
            // In particolare, recuperiamo le mappe di vicini e carri armati
            string playerName = player["PlayerId"].get<string>();
            auto neighborsMap = player["Neighbors"].get<map<string, vector<string>>>(); 
            auto tankCountsMap = player["Tanks"].get<map<string, int>>();

            // Creiamo un oggetto player (allocato nello stack che verrà deallocato alla fine del for)
            Player newPlayer(neighborsMap, tankCountsMap, playerName); 

            // Aggiungiamo il player creato al vettore di player
            players.push_back(newPlayer);
        }
        return players;

    }catch (const exception& e) {
        cerr << "Errore JSON: " << e.what() << endl;
        return {}; // vettore vuoto in caso di errore
    }


}


// Funzione responsabile di estrarre la frontiera dei territori non posseduti da un giocatore (a partire dalla mappa dei vicini)
const set<string> getNotOwnedFrontier(const map<string, vector<string>> & map) {
    set<string> ownedTerritories = getTerritoriesFromMap(map);
    set<string> notOwnedTerritories;

    for(const auto& pair: map){
        const auto& neighbors = pair.second;
        for(const auto& territory: neighbors){
            // Se il territorio non appartiene a quelli posseduti lo inseriamo nella frontiera dei territori non posseduti
            // Cerchiamo il territorio nel range che va dall'inizio alla fine di ownedTerritories
            if(find(ownedTerritories.begin(), ownedTerritories.end(), territory)== ownedTerritories.end()){
                notOwnedTerritories.insert(territory);
            }
        }
    }

    return notOwnedTerritories;

}


// Funzione responsabile di estrarre la frontiera dei territori posseduti da un giocatore (a partire dalla mappa dei vicini)
const set<string> getOwnedFrontier(const map<string, vector<string>> & map) {
    set<string> ownedTerritories = getTerritoriesFromMap(map);
    set<string> ownedFrontier;

    for(const auto& pair: map){
        const auto& neighbors = pair.second;
        bool frontier = false;

        // Se ALMENO 1 territorio tra i vicini non appartiene ai territori posseduti allora il territorio chiave della mappa è un territorio di frontiera posseduto 
        for(const auto& territory: neighbors){
            if(find(ownedTerritories.begin(), ownedTerritories.end(), territory) == ownedTerritories.end()){
                frontier = true;
                break;
            }
        }
        if(frontier){
            ownedFrontier.insert(pair.first);
        }
    }

    return ownedFrontier;
}


// Funzione che restituisce un booleano:
// - true se il giocatore ha vinto 
// - false se il giocatore non ha vinto
// Affinchè il gioocatore vinca la sua lista di territori deve comprendere ALMENO 3 continenti e deve comprendere ALMENO 28 territori
bool win(const set<string> & territories){
    clog << "----------------------------------------------------------------------------" << endl;
    clog << "Inizio Algoritmo per determinare la condizione di vittoria" << endl;
    if(territories.size() < 28){
        clog << "Condizione di vittoria non rispettata: " << territories.size() << " territori conquistati" << " (devono essere conquistati almeno 28 territori)" << endl;
        return false;

    }

    // Criamo un set di stringhe per tenere traccia dei continenti completati
    set<string> completedContinents;

    for(const auto & pair: continents){
        clog << "\nValutazione del completamento del continente " << pair.first << ":" << endl; 
        // Usiamo i riferimenti per evitare di allocare altra memoria
        const auto& continent = pair.first;
        const auto& continentTerritories = pair.second;
        
        // cicliamo la lista di territori
        bool completed = true;
        for(const auto & territory: continentTerritories){
           if (territories.count(territory) == 0){
                completed = false;
                clog << territory << " mancante" << endl;
                break; // è inutile continuare il ciclo perché manca almeno 1 territorio per completare il continente
            }
            clog << territory << " presente" << endl;

        }

        // se il giocatore possiede tutti i territori del continente allora aggiungiamo il continente tra quelli completati
        if(completed){
            clog << "Continente " << continent << " completato" << endl;
            completedContinents.insert(continent);
        }

        if(completedContinents.size() > 2){
            clog << "Vittoria!\nContinenti completati: ";
            for (const auto& continent : completedContinents) {
                clog << continent << " ";
            }          
            clog << endl;
            return true;
        }
    }
    

    clog << "Condizione di vittoria non rispetatta, continenti completati: " << completedContinents.size() << " (devono essere completati almeno 3 continenti)" << endl;
    // se il giocatore non ha completato almeno 3 continenti 
    return false;

} 


// Funzione che simula il lancio dei dadi e che li ordina in modo decrescente
int rollTheDice(int (&cpuAttackDice)[3], int (&userDefenseDice)[3], const int& userTanksCount){
    /* 
        La CPU attacca sempre con 3 carri armati, quindi simuliamo il lancio di 3 dadi (numero casuale tra 1 e 6)
        In particolare:
        rand() % 6 restituisce un numero tra 0 e 5 (prendiamo il resto della divisione per 6).
        + 1 sposta l'intervallo a 1-6, simulando il lancio di un dado. 
    */

    // Assegniamo i valori dei dadi alla CPU
    cpuAttackDice[0] = (rand() %6) + 1;
    cpuAttackDice[1] = (rand() %6) + 1;
    cpuAttackDice[2] = (rand() %6) + 1;

    int defenseDiceCount = std::min(3, userTanksCount);

    // Simuliamo il lancio dei dadi per il giocatore
    for (int i = 0; i < defenseDiceCount; i++) {
        userDefenseDice[i] = (rand() %6) + 1;
    }

    // Adesso ordiniamo i dadi della cpu e del giocatore in ordine decrescente
    /* per fare questo sfruttiamo la funzione sort della libreria standard, la quale ha questa sintassi: std::sort(inizio, fine, criterio);
        - inizio -> Iteratore o puntatore al primo elemento da ordinare.
        - fine -> Iteratore o puntatore alla posizione successiva all'ultimo elemento.
        - criterio (opzionale) -> Funzione o oggetto che specifica l'ordinamento (default crescente)

        In questo caso, passiamo il nome del vettore statico (puntatore al primo elemento) e utilizziamo la condizione greater<int>().
        greater è un funtore, quindi la classe Greater ha ridefinito l'operatore () in modo tale che ritorni un booleano sulla base di un confronto di disuguaglianza:
            - se il primo elemento è maggiore del secondo ritorna true, altrimenti false
    
    */
    sort(cpuAttackDice,cpuAttackDice + 3, greater<int>());
    sort(userDefenseDice,userDefenseDice + defenseDiceCount, greater<int>());
    
    return defenseDiceCount;

}



} // fine del namespace uniwar 