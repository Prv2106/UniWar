#ifndef FUNCTION_LIB_H
#define FUNCTION_LIB_H

// includiamo le librerie necessarie
#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <set>
#include "json.hpp"  // Libreria JSON per C++


using namespace std;

#ifdef __cplusplus  // Serve a controllare se il codice è compilato con un compilatore C++
    // Per evitare che le funzioni definite in C abbiano nomi diversi nella DLL o nel file oggetto, si usa extern "C" per disabilitare il name mangling.
    // Se non usiamo extern "C", il compilatore C++ modifica il nome della funzione (name mangling), rendendo impossibile chiamarla direttamente da C# con [DllImport].
    // extern "C" in C++: Indica che le funzioni all'interno devono usare il linkage C, ovvero senza name mangling.

    extern "C" { //  Se il codice è compilato in C++, indica al compilatore di non applicare name mangling alle funzioni dichiarate all'interno.
#endif


/**
 * __declspec (abbreviazione di declaration specifier) è un specificatore di dichiarazione usato nei compilatori Microsoft Visual C++ (MSVC) 
 * per modificare il comportamento di variabili, funzioni, classi e oggetti durante la compilazione. 
 * In particolare, __declspec permette di controllare come funzioni e variabili vengono importate/esportate da una DLL, allocate in memoria o ottimizzate dal compilatore.
 */

#ifdef BUILD_MY_DLL
    #define FUNCTION_LIB __declspec(dllexport) // Esporta funzione se compiliamo la DLL
#else
    #define FUNCTION_LIB __declspec(dllimport) // Importa funzione se usiamo la DLL
#endif


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

        // I getter sono entrambi definiti in modo tale che il valore restituito non è modificabile e che i getter stessi non possono modificare gli attributi della classe Player
        // Restituiamo un riferimento costante
        const  map<string, vector<string>> & getNeighbors() const;
        
        const  map<string, int> & getTanks() const;

        const string & getName() const;

        // metodo per aggiungere un territorio e i suoi vicini
        void addTerritory(const string & territory, const vector<string> & neighbors, int count);

        void modifyTankCount(const string & territory, int newCount);

        void removeTerritory(const string & territory);

        // funzione per il debug
        void printData() const;

};

// In questo caso, extern indica al compilatore che la definizione si trova in un altro file
extern  const map<string, vector<string>> continents;


// Funzione che riceve una stringa JSON e restituisce un'altra stringa JSON
FUNCTION_LIB const char* process_map(const char* jsonData);


FUNCTION_LIB void testWin();


#ifdef __cplusplus 
    } // chiudiamo il blocco delle funzioni per il quale non deve essere applicato il name mangling
#endif


#endif // Fine della libreria