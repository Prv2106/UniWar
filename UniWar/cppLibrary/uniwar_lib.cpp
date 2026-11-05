#include "uniwar_lib.hpp"
#include "functions_lib.hpp"
using json = nlohmann::json;

using namespace std;

namespace uniwar{

/******************************************** Player implementation *************************************/
// Constructor using an initializer list.
Player::Player(const map<string, vector<string>> & neighbors, const  map<string, int> & tanks, const string & player): neighborsMap(neighbors), tankCountMap(tanks), playerId(player) {}

const  map<string, vector<string>>& Player::getNeighborsMap() const{
    return neighborsMap;
}

// Returns the neighbor list for a given territory.
const vector<string> & Player::getNeighbors(const string& territory){
    if((territory.empty()) || (tankCountMap.find(territory) == tankCountMap.end() || neighborsMap.find(territory) == neighborsMap.end())){
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste.");
    }
    return neighborsMap.at(territory);
}

const  map<string, int> & Player::getTanksMap() const {
    return tankCountMap;
}

const string & Player::getName() const{
    return playerId;
}

// Adds a territory together with its neighbors.
void Player::addTerritory(const string & territory, const vector<string> & neighbors, int count){
    if(territory.empty() || neighbors.empty()){
        throw std::invalid_argument("Errore: il territorio {" + territory + "} o la lista dei vicini è vuota.");
        return;
    }

    // Check whether an entry already exists for the given territory.
    /*
        In a std::map, .find(key) returns an iterator to the matching element
        when the key exists, or end() when it does not.
    */
    if((neighborsMap.find(territory) == neighborsMap.end())){ // Key absent
        neighborsMap[territory] = neighbors; // Create a new entry
    }
    else{
        throw std::runtime_error("Errore: il territorio {" + territory + "} è già presente in tankCountMap.");
    }
    
    if(tankCountMap.find(territory) == tankCountMap.end()){ // Key absent
        tankCountMap[territory] = count; // Create a new entry
    }
    else{
        throw std::runtime_error("Errore: il territorio {" + territory + "} è già presente in tankCountMap.");
    }

}

// Replaces the tank count of a territory.
void Player::modifyTankCount(const string & territory, int newCount){
    if((territory.empty()) || (tankCountMap.find(territory) == tankCountMap.end())){
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste.");
        return;
    }
    else{
        tankCountMap[territory] = newCount; 
    }

}

// Removes a territory from both maps.
void Player::removeTerritory(const string & territory){
    if((territory.empty()) || (tankCountMap.find(territory) == tankCountMap.end() || neighborsMap.find(territory) == neighborsMap.end())){
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste o è già stato rimosso.");
        return;
    }
    neighborsMap.erase(territory);
    tankCountMap.erase(territory);
}

// Returns the tank count of a territory.
const int Player::getTanksCount(const string &territory) const {
    if ((territory.empty()) || (neighborsMap.find(territory) == neighborsMap.end()) || (tankCountMap.find(territory) == tankCountMap.end())) {
        throw std::out_of_range("Errore: il territorio {" + territory + "} non esiste.");
        return -1;  
    }
    
    return tankCountMap.at(territory);  // .at() performs a bounds-checked lookup and throws when the key is missing.
}

/****** End of Player implementation */



// Continent mapping used by the win-condition check.
const map<string, vector<string>> continents = {
    {"AmericaDelNord", {"Alaska", "Alberta", "StatiUnitiOccidentali", "StatiUnitiOrientali","AmericaCentrale", "Ontario", "TerritoriDelNordOvest", "Groenlandia", "Quebec"}},
    {"AmericaDelSud", {"Venezuela", "Perù", "Brasile", "Argentina"}},
    {"Africa", {"AfricaDelNord", "Egitto", "AfricaOrientale", "Congo", "AfricaDelSud", "Madagascar"}},
    {"Asia", {"Kamchatka", "Jacuzia", "Cita", "Giappone", "Cina", "Siam", "India", "MedioOriente","Afghanistan", "Urali", "Siberia", "Mongolia"}},
    {"Europa", {"EuropaOccidentale", "EuropaMeridionale", "GranBretagna", "Islanda", "Ucraina", "EuropaSettentrionale", "Scandinavia"}},
    {"Oceania", {"AustraliaOccidentale", "NuovaGuinea", "Indonesia", "AustraliaOrientale"}}
};


/* Helper functions */

// Initializes the player context from the JSON payload received from C#.
vector<Player> initializePlayers(const char* jsonData){

    if (jsonData == nullptr || strlen(jsonData) == 0) {
        cerr << "Errore: il JSON passato è vuoto o nullo" << endl;
        throw std::invalid_argument("Json vuoto o nullo");
    }

    // Use nlohmann/json parse() to convert the input string into a JSON array.
    json parsedData = json::parse(jsonData);

    /*
        parsedData now has the following shape:
        [
            { "PlayerId": "Player1", "Tanks": {"A": 3, "B": 2}, "Neighbors": {"A": ["B", "C"], "B": ["A"]} },
            { "PlayerId": "Player2", "Tanks": {"C": 4}, "Neighbors": {"C": ["A"]} }
        ]
    */

    // Vector of player objects.
    vector<Player> players;
        
    // Iterate over each player in the JSON payload.
    // ``player`` is a reference to a JSON object; ``auto`` resolves the type.
    for (auto& player : parsedData) {
        // Verify that the required keys are present.
        if (!(player.contains("PlayerId") && player.contains("Neighbors") && player.contains("Tanks"))) {
            cerr << "Errore: JSON malformato" << endl;
            throw std::invalid_argument("Json Malformato");
        }
    
        // The get() member converts JSON values into standard C++ types.
        // In particular, retrieve the adjacency and tank-count maps.
        string playerName = player["PlayerId"].get<string>();
        auto neighborsMap = player["Neighbors"].get<map<string, vector<string>>>(); 
        auto tankCountsMap = player["Tanks"].get<map<string, int>>();

        // Create a stack-allocated player for the current iteration.
        Player newPlayer(neighborsMap, tankCountsMap, playerName); 

        // Append the player to the result vector.
        players.push_back(newPlayer);
    }
    return players;

    


}


// Extracts the frontier of territories not owned by a player from the adjacency map.
const set<string> getNotOwnedFrontier(const map<string, vector<string>> & map) {
    set<string> ownedTerritories = getTerritoriesFromMap(map);
    set<string> notOwnedTerritories;

    for(const auto& [_, neighbors]: map){
        for(const auto& territory: neighbors){
            // Insert territories that are not owned into the not-owned frontier.
            if(find(ownedTerritories.begin(), ownedTerritories.end(), territory)== ownedTerritories.end()){
                notOwnedTerritories.insert(territory);
            }
        }
    }

    return notOwnedTerritories;
}


// Extracts the frontier of territories owned by a player from the adjacency map.
const set<string> getOwnedFrontier(const map<string, vector<string>> & map) {
    set<string> ownedTerritories = getTerritoriesFromMap(map);
    set<string> ownedFrontier;

    for(const auto& [territory, neighbors]: map){
        bool frontier = false;

        // A territory belongs to the owned frontier when at least one neighbor
        // is not owned by the player.
        for(const auto& territory: neighbors){
            if(ownedTerritories.count(territory) == 0){
                frontier = true;
                break;
            }
        }
        if(frontier){
            ownedFrontier.insert(territory);
        }
    }

    return ownedFrontier;
}


// Returns true when the player satisfies the victory condition:
// at least 28 owned territories and at least 3 fully controlled continents.
bool win(const set<string> & territories){
    clog << "----------------------------------------------------------------------------" << endl;
    clog << "Inizio Algoritmo per determinare la condizione di vittoria" << endl;
    if(territories.size() < 28){
        clog << "Condizione di vittoria non rispettata: " << territories.size() << " territori conquistati" << " (devono essere conquistati almeno 28 territori)" << endl;
        return false;

    }

    // Track the fully controlled continents.
    set<string> completedContinents;

    for(const auto& [continent, continentTerritories]: continents){
        clog << "\nValutazione del completamento del continente " << continent << ":" << endl; 
                
        // Iterate over the continent's territories.
        bool completed = true;
        for(const auto & territory: continentTerritories){
           if (territories.count(territory) == 0){
                completed = false;
                clog << territory << " mancante" << endl;
                break; // Stop early: at least one territory is missing.
            }
            clog << territory << " presente" << endl;

        }

        // Mark the continent as completed when every territory is owned.
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
    // The player did not complete at least three continents.
    return false;
} 


// Simulates the dice rolls and sorts them in descending order.
int rollTheDice(int (&cpuAttackDice)[3], int (&userDefenseDice)[3], const int& userTanksCount){
    /* 
        The CPU always attacks with three tanks, so three dice are rolled
        (random values in the range 1-6). ``rand() % 6`` yields 0-5; adding 1
        shifts the range to 1-6.
    */

    // Assign the CPU dice values.
    cpuAttackDice[0] = (rand() %6) + 1;
    cpuAttackDice[1] = (rand() %6) + 1;
    cpuAttackDice[2] = (rand() %6) + 1;

    int defenseDiceCount = std::min(3, userTanksCount);

    // Roll the defender dice.
    for (int i = 0; i < defenseDiceCount; i++) {
        userDefenseDice[i] = (rand() %6) + 1;
    }

    // Sort both dice arrays in descending order.
    /*
        std::sort(begin, end, comparator) is used:
        - begin: iterator or pointer to the first element.
        - end: iterator or pointer past the last element.
        - comparator: optional ordering predicate (ascending by default).

        The arrays decay to pointers to their first element. ``greater<int>``
        is a functor whose call operator returns true when the first argument
        is greater than the second, producing a descending order.
    */
    sort(cpuAttackDice,cpuAttackDice + 3, greater<int>());
    sort(userDefenseDice,userDefenseDice + defenseDiceCount, greater<int>());
    
    return defenseDiceCount;

}



} // namespace uniwar