#include "functions_lib.h"



using json = nlohmann::json;
using namespace std;



/****** Implementazione di Player */


// Implementazione del costruttore (con lista di inizializzazione)
Player::Player(const map<string, vector<string>> & neighbors, const  map<string, int> & tanks, const string & player): neighborsMap(neighbors), tankCountMap(tanks), playerId(player) {}

// Implementazione del getter della mappa dei territori vicini
const  map<string, vector<string>>& Player::getNeighbors() const{
    return neighborsMap;
}

// Implementazione del getter della mappa con il numero di carri armati
const  map<string, int> & Player::getTanks() const {
    return tankCountMap;
}

const string & Player::getName() const{
    return playerId;
}

// Implementazione del metodo per aggiungere un territorio e i suoi vicini
void Player::addTerritory(const string & territory, const vector<string> & neighbors, int count){
    if(territory.empty() || neighbors.empty()){
        clog << "Territorio non valido" << endl;
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
        clog << "Il territorio è già presente in neighborsMap" << endl;
    }
    
    if(tankCountMap.find(territory) == tankCountMap.end()){ // la chiave non esiste
        tankCountMap[territory] = count; // crea una nuova entry
    }
    else{
        clog << "Il territorio è già presente in tankCountMap" << endl;
    }

}


// Implementazione del metodo per modificare il numero di carri armati di un territorio
void Player::modifyTankCount(const string & territory, int newCount){
    if(territory.empty()){
        clog << "La stringa territory è vuota" << endl;
        return;
    }
    if(tankCountMap.find(territory) == tankCountMap.end()){
        clog << "Il giocatore non possiede il territorio " << territory << endl;
        return;
    }

    else{
        tankCountMap[territory] = newCount; 
    }

}

// Implementazione del metodo per rimuovere un territorio
void Player::removeTerritory(const string & territory){
    if(territory.empty()){
        clog << "La stringa territory è vuota" << endl;
        return;
    }

    if(tankCountMap.find(territory) == tankCountMap.end() || neighborsMap.find(territory) == neighborsMap.end()){
        clog << "Il giocatore non possiede il territorio " << territory << endl;
        return;
    }

    neighborsMap.erase(territory);
    tankCountMap.erase(territory);
}

// Implementazione di printData
void Player::printData() const{
    // non usiamo endl adesso per evitare di svuotare il buffer per ogni stampa ma lo facciamo alla fine
    cout << "Lista dei territori con i vicini:\n"; 

    // iteriamo sulla mappa neighborsMap utilizzando il for range
    // con auto il tipo è determinato in modo implicito, il tipo sarà <const string, vector<string>>
    for(const auto & pair: neighborsMap){
        // stampiamo il nome del territorio posseduto dal giocatore
        cout << pair.first << ""; // stampiamo ogni suo vicino separato da uno spazio
        for (const auto & neighbor: pair.second){
            cout << neighbor << "";
        }
        cout << "\n"; // andiamo a capo dopo aver stampato tutti i vicini di un territorio
    }
    cout << "Lista dei territori posseduti da "<< playerId<< " insieme all'indicazione relativa al numero di carri armati:\n"; 

    for(const auto & pair : tankCountMap){
        cout << pair.first << ": " << pair.second << "\n";

    }
    cout << "=============" << endl; // forziamo lo svuotamento del buffer


}

/****** Fine implementazione di Player */

/* Inizializzazione di continents*/

const map<string, vector<string>> continents = {

    {"AmericaDelNord", {"Alaska", "Alberta", "StatiUnitiOccidentali", "StatiUnitiOrientali","AmericaCentrale", "Ontario", "TerritoriDelNordOvest", "Groenlandia", "Quebec"}},
    {"AmericaDelSud", {"Venezuela", "Perù", "Brasile", "Argentina"}},
    {"Africa", {"AfricaDelNord", "Egitto", "AfricaOrientale", "Congo", "AfricaDelSud", "Madagascar"}},
    {"Asia", {"Kamchatka", "Jacuzia", "Cita", "Giappone", "Cina", "Siam", "India", "MedioOriente","Afghanistan", "Urali", "Siberia", "Mongolia"}},
    {"Europa", {"EurpoaOccidentale", "EuropaMeridionale", "GranBretagna", "Islanda", "Ucraina", "EuropaSettentrionale", "Scandinavia"}},
    {"Oceania", {"AustraliaOccidentale", "NuovaGuinea", "Indonesia", "AustraliaOrientale"}}

};




/* Funzioni */


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
            if (!player.contains("PlayerId") || player["PlayerId"].is_null()) {
                cerr << "Errore: Il campo 'PlayerId' è mancante o nullo." << endl;
                continue;
            }
            string playerName = player["PlayerId"].get<string>();

            if (!player.contains("Neighbors") || player["Neighbors"].is_null()) {
                cerr << "Errore nel giocatore " << playerName << ": Manca la chiave 'Neighbors'" << endl;
                continue;
            }

            if (!player.contains("Tanks") || player["Tanks"].is_null()) {
                cerr << "Errore nel giocatore " << playerName << ": Manca la chiave 'Tanks'" << endl;
                continue;
            }


            // Sfruttiamo la funzione membro "get" per convertire i valori dell'oggetto json in tipi di dati standard di C++.
            // In particolare, recuperiamo le mappe di vicini e carri armati
            auto neighborsMap = player["Neighbors"].get<map<string, vector<string>>>();
            auto tankCountsMap = player["Tanks"].get<map<string, int>>();

            // Creiamo un oggetto player (allocato nello stack che verrà deallocato aalla fine del for)
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

// Funzione di test (da rimpiazzare poi con la logica di attacco e con il posizionamento delle truppe)
const char* process_map(const char* jsonData) {
    static string resultStr;  // Variabile statica per evitare memory leak
    vector<Player> players = initializePlayers(jsonData);

    // Ora 'players' contiene tutte le istanze di Player create dal JSON

    // Possimao modificare players
    // Per esempio eliminiamo un territorio per player1
    players[0].removeTerritory("TerritorioA");

    // Creiamo un vettore di oggetti JSON per serializzare i dati
    vector<json> updatedPlayersData;

    // Popoliamo il vettore con i dati dei giocatori
    for (const auto& player : players) {
        json playerJson;
        playerJson["PlayerId"] = player.getName();  
        playerJson["Neighbors"] = player.getNeighbors();
        playerJson["Tanks"] = player.getTanks();

        updatedPlayersData.push_back(playerJson);
    }

    // Convertiamo il vettore di oggetti JSON in una stringa JSON
    resultStr = json(updatedPlayersData).dump();  // Serializziamo i dati in formato stringa

    // La funzione c_str() della classe std::string restituisce un puntatore a carattere (const char*)
    return resultStr.c_str();
}







// Funzione che si occupa di estrarre la lista dei territori a partire da una mappa
const vector<string> getTerritoriesFromMap(const map<string, vector<string>> & map){
    vector<string> territories;

    for(const auto& pair: map){
        territories.push_back(pair.first);
    }

    return territories;
}

const vector<string> getTerritoriesFromMap(const map<string, int> & map){
    vector<string> territories;

    for(const auto& pair: map){
        territories.push_back(pair.first);
    }

    return territories;
}




// Funzione che restituisce un booleano:
// - true se il giocatore ha vinto 
// - false se il giocatore non ha vinto
// Affinchè il gioocatore vinca la sua lista di territori deve comprendere ALMENO 3 continenti e deve comprendere ALMENO 28 territori
bool win(const vector<string> & territories){
    if(territories.size() < 28){
        return false;

    }
    // Criamo un set di stringhe per tenere traccia dei continenti completati
    set<string> completedContinents;

    for(const auto & pair: continents){
        // Usiamo i riferimenti per evitare di allocare altra memoria
        const auto& continent = pair.first;
        const auto& continentTerritories = pair.second;
        
        // cicliamo la lista di territori
        bool completed = true;
        for(const auto & territory: continentTerritories){
            /*
                La funzione find cerca l'elemento territory tra gli elementi che vanno da territories.begin() fino a territories.end(). 
                La funzione restituisce un iteratore che punta al primo elemento trovato uguale a territory, oppure a territories.end() se l'elemento non viene trovato.
            */
            if(find(territories.begin(), territories.end(), territory) == territories.end()){
                completed = false;
                break; // è inutile continuare il ciclo perché manca almeno 1 territorio per completare il continente
            }
        }

        // se il giocatore possiede tutti i territori del continente allora aggiungiamo il continente tra quelli completati
        if(completed){
            completedContinents.insert(continent);
        }

        if(completedContinents.size() > 2){
            cout << "Vittoria!\nContinenti completati: ";
    
            // Stampiamo tutti i continenti completati
            for (const auto& continent : completedContinents) {
                cout << continent << " ";
            }          
            cout << endl;
            return true;
        }
    }
    
    // se il giocatore non ha completato almeno 3 continenti 
    return false;


}












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





void testWin(){
    cout << "Prova 1 (dovrebbe essere true)\n" << endl;
    cout << win(getTerritoriesFromMap(winnerMap1)) << "\n" << endl;
    cout << "Prova 1 (dovrebbe essere true)\n" << endl;
    cout << win(getTerritoriesFromMap(WinnerMap2)) << "\n" << endl;

    cout << "Prova 2 (dovrebbe essere false)\n" << endl;
    cout << win(getTerritoriesFromMap(neighborsMapIncomplete)) << "\n" << endl;
    
    cout << "Prova 2 (dovrebbe essere false)\n" << endl;
    cout << win(getTerritoriesFromMap(tanksMapIncomplete)) << "\n" << endl;
}

