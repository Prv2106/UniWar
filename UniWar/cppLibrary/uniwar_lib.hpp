#ifndef UNIWAR_LIB_H
#define UNIWAR_LIB_H

#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <set>
using namespace std;

#define NODISCARD [[nodiscard]]

namespace uniwar{

    class Player{

    // Nota: gli attributi di una classe di default sono private
    string playerId;

    // Mappa per i territori vicini
    map<string, vector<string>> neighborsMap; 

    // Mappa per i carri armati associati a ciascun territorio
    map<string, int> tankCountMap;

    public:
        // costruttore
        // I parametri sono passati per riferimento costante, quindi i valori passati non possono essere modificati
        Player(const map<string, vector<string>> & neighbors, const  map<string, int> & tanks, const string & player);

        // I getter sono definiti in modo tale che il valore restituito non è modificabile e che i getter stessi non possono modificare gli attributi della classe Player
        // Restituiamo un riferimento costante
        NODISCARD const  map<string, vector<string>> & getNeighborsMap() const;
        
        NODISCARD const  map<string, int> & getTanksMap() const;

        NODISCARD const vector<string> & getNeighbors(const string& territory);

        NODISCARD const string& getName() const;

        NODISCARD const int getTanksCount(const string & territory) const;


        // metodo per aggiungere un territorio e i suoi vicini
        void addTerritory(const string & territory, const vector<string> & neighbors, int count);

        void modifyTankCount(const string & territory, int newCount);

        void removeTerritory(const string & territory);

};


// In questo caso, extern indica al compilatore che la definizione si trova in un altro file
/*
    Senza extern, la mappa verrebbe ridefinita in ogni file che include l'header, causando errori di linker (multiple definition error).
    extern dice al compilatore che la variabile esiste da qualche parte (in un file .cpp), quindi non la ridefinisce.
*/
extern  const map<string, vector<string>> continents;


// Funzione che si occupa di estrarre la lista dei territori a partire da una mappa
template <typename T>
NODISCARD const set<string> getTerritoriesFromMap(const map<string, T> & map) {
    set<string> territories;
    for (const auto& pair : map) {
        territories.insert(pair.first);
    }
    return territories;
}

NODISCARD int rollTheDice(int (&cpuAttackDice)[3], int (&userDefenseDice)[3], const int& userTanksCount);


inline void compareDice(const int (&cpuAttackDice)[3], const int (&userDefenseDice)[3],const int& defenseDiceCount ,int& cpuLosses, int& userLosses){
    clog << "----------------------------------------------------------------------------" << endl;
    for(int i=0; i < defenseDiceCount; i++){
        clog << "Dado " << i << " della CPU = " << cpuAttackDice[i] << " , dado " << i << " del giocatore = " << userDefenseDice[i] << endl; // debug
        if(cpuAttackDice[i] > userDefenseDice[i]){
            userLosses++;
        }
        else {
            cpuLosses++;
        }
        clog << "Perdite CPU = " << cpuLosses << ", Perdite giocatore = " << userLosses << endl; // debug
    }
    clog << "----------------------------------------------------------------------------" << endl;

}

NODISCARD bool win(const set<string> & territories);

NODISCARD vector<uniwar::Player> initializePlayers(const char* jsonData);

NODISCARD const set<string> getNotOwnedFrontier(const map<string, vector<string>> & map);

NODISCARD const set<string> getOwnedFrontier(const map<string, vector<string>> & map);


} // fine del namespace uniwar

#endif // Fine della libreria