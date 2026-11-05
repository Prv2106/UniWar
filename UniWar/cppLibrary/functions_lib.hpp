#ifndef FUNCTION_LIB_H
#define FUNCTION_LIB_H

// Required standard and third-party headers.
#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <set>
#include<algorithm>
#include "json.hpp"  // JSON library for C++
#include "uniwar_lib.hpp"
#include <random>




using namespace std;

#ifdef __cplusplus  // Guard for C++ compilation
    // extern "C" disables C++ name mangling so the exported symbols can be
    // resolved by C# through [DllImport]. Without it, the compiler rewrites
    // function names and the native entry points become unavailable.
    extern "C" {
#endif


/**
 * __declspec (declaration specifier) is a Microsoft Visual C++ (MSVC)
 * storage-class attribute used to control how functions and variables are
 * imported from or exported to a DLL.
 */

#ifdef BUILD_MY_DLL
    #define FUNCTION_LIB __declspec(dllexport)  // Export when building the DLL
#else
    #define FUNCTION_LIB __declspec(dllimport)  // Import when consuming the DLL
#endif


/**
 * @brief Reinforces the CPU territories with new tanks.
 *
 * @param jsonData JSON payload describing the game state.
 * @param newTanks Number of tanks available for reinforcement.
 * @return Serialized JSON string with the updated CPU state.
 */
FUNCTION_LIB const char* reinforcement (const char* jsonData, int newTanks);

/**
 * @brief Simulates the CPU attack phase.
 *
 * @param jsonData JSON payload describing both players.
 * @return Serialized JSON string with battle results, or ``"Pass"``.
 */
FUNCTION_LIB const char* cpuAttack (const char* jsonData);

/**
 * @brief Checks whether the player satisfies the victory condition.
 *
 * @param jsonData JSON payload describing the player state.
 * @return ``true`` if the player has won.
 */
FUNCTION_LIB bool winCheck (const char* jsonData);


#ifdef __cplusplus 
    } // Close the block that disables name mangling
#endif


#endif // FUNCTION_LIB_H