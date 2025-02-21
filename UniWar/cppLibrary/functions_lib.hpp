#ifndef FUNCTION_LIB_H
#define FUNCTION_LIB_H

// includiamo le librerie necessarie
#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <set>
#include<algorithm>
#include "json.hpp"  // Libreria JSON per C++
#include "uniwar_lib.hpp"
#include <random>




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


// Funzione per il rinforzo dei territori
FUNCTION_LIB const char* reinforcement (const char* jsonData, int newTanks);

// Funzione per l'attacco della cpu
FUNCTION_LIB const char* cpuAttack (const char* jsonData);

// Funzione per determinare se il giocatore ha vinto
FUNCTION_LIB bool winCheck (const char* jsonData);


#ifdef __cplusplus 
    } // chiudiamo il blocco delle funzioni per il quale non deve essere applicato il name mangling
#endif


#endif // Fine della libreria