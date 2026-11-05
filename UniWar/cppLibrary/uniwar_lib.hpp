#ifndef UNIWAR_LIB_H
#define UNIWAR_LIB_H

#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <set>
using namespace std;

#define NODISCARD [[nodiscard]]

/**
 * @brief Core game model and helpers shared by the C++ library.
 *
 * The namespace contains the player representation used across the native
 * boundary, territory utilities, dice logic, and the win-condition check.
 */
namespace uniwar{

    /**
     * @brief Represents a player's territory graph and tank counts.
     *
     * Territory names are used as keys. The class is intentionally limited to
     * value semantics so instances can be copied across the native boundary.
     */
    class Player{

    // Class members are private by default.
    string playerId;

    // Adjacency map: territory -> neighboring territories.
    map<string, vector<string>> neighborsMap; 

    // Tank count per territory.
    map<string, int> tankCountMap;

    public:
        /**
         * @brief Constructs a player from adjacency and tank-count maps.
         *
         * @param neighbors Adjacency map.
         * @param tanks Tank count per territory.
         * @param player Player identifier.
         */
        Player(const map<string, vector<string>> & neighbors, const  map<string, int> & tanks, const string & player);

        /**
         * @brief Returns the adjacency map.
         * @return Const reference to the adjacency map.
         */
        NODISCARD const  map<string, vector<string>> & getNeighborsMap() const;
        
        /**
         * @brief Returns the tank-count map.
         * @return Const reference to the tank-count map.
         */
        NODISCARD const  map<string, int> & getTanksMap() const;

        /**
         * @brief Returns the neighbors of a territory.
         *
         * @param territory Territory name.
         * @return Const reference to the neighbor list.
         * @throw std::out_of_range If the territory is unknown.
         */
        NODISCARD const vector<string> & getNeighbors(const string& territory);

        /**
         * @brief Returns the player identifier.
         * @return Const reference to the identifier.
         */
        NODISCARD const string& getName() const;

        /**
         * @brief Returns the tank count of a territory.
         *
         * @param territory Territory name.
         * @return Number of tanks.
         * @throw std::out_of_range If the territory is unknown.
         */
        NODISCARD const int getTanksCount(const string & territory) const;


        /**
         * @brief Adds a territory with its neighbors and initial tank count.
         *
         * @param territory Territory name.
         * @param neighbors Neighbor list.
         * @param count Initial tank count.
         * @throw std::invalid_argument If the territory or neighbor list is empty.
         * @throw std::runtime_error If the territory already exists.
         */
        void addTerritory(const string & territory, const vector<string> & neighbors, int count);

        /**
         * @brief Replaces the tank count of an existing territory.
         *
         * @param territory Territory name.
         * @param newCount New tank count.
         * @throw std::out_of_range If the territory is unknown.
         */
        void modifyTankCount(const string & territory, int newCount);

        /**
         * @brief Removes a territory from both maps.
         *
         * @param territory Territory name.
         * @throw std::out_of_range If the territory is unknown.
         */
        void removeTerritory(const string & territory);

};


/**
 * @brief Continent-to-territory mapping used by the win condition.
 *
 * ``extern`` declares the variable without defining it here; the definition
 * lives in ``uniwar_lib.cpp`` so the map is not duplicated in every
 * translation unit.
 */
extern  const map<string, vector<string>> continents;


/**
 * @brief Extracts the territory names from a generic map.
 *
 * @tparam T Value type of the source map.
 * @param map Source map keyed by territory name.
 * @return Set containing the territory names.
 */
template <typename T>
NODISCARD const set<string> getTerritoriesFromMap(const map<string, T> & map) {
    set<string> territories;
    for (const auto& [territory, _] : map) {
        territories.insert(territory);
    }
    return territories;
}

/**
 * @brief Rolls attack and defense dice and sorts them in descending order.
 *
 * The CPU always rolls three dice. The defender rolls one die per tank, up to
 * a maximum of three.
 *
 * @param cpuAttackDice Output array of three attacker dice.
 * @param userDefenseDice Output array of three defender dice.
 * @param userTanksCount Number of tanks available to the defender.
 * @return Number of defense dice actually rolled.
 */
NODISCARD int rollTheDice(int (&cpuAttackDice)[3], int (&userDefenseDice)[3], const int& userTanksCount);


/**
 * @brief Compares dice pairs and accumulates losses.
 *
 * A tie is won by the defender.
 *
 * @param cpuAttackDice Attacker dice in descending order.
 * @param userDefenseDice Defender dice in descending order.
 * @param defenseDiceCount Number of dice pairs to compare.
 * @param cpuLosses Output accumulator for attacker losses.
 * @param userLosses Output accumulator for defender losses.
 */
inline void compareDice(const int (&cpuAttackDice)[3], const int (&userDefenseDice)[3],const int& defenseDiceCount ,int& cpuLosses, int& userLosses){
    clog << "----------------------------------------------------------------------------" << endl;
    for(int i=0; i < defenseDiceCount; i++){
        clog << "Dado " << i << " della CPU = " << cpuAttackDice[i] << " , dado " << i << " del giocatore = " << userDefenseDice[i] << endl; // Debug output
        if(cpuAttackDice[i] > userDefenseDice[i]){
            userLosses++;
        }
        else {
            cpuLosses++;
        }
        clog << "Perdite CPU = " << cpuLosses << ", Perdite giocatore = " << userLosses << endl; // Debug output
    }
    clog << "----------------------------------------------------------------------------" << endl;

}

/**
 * @brief Evaluates the victory condition.
 *
 * The condition requires at least 28 territories and at least three fully
 * controlled continents.
 *
 * @param territories Territories owned by the player.
 * @return ``true`` if the victory condition is satisfied.
 */
NODISCARD bool win(const set<string> & territories);

/**
 * @brief Builds player instances from a JSON payload received from C#.
 *
 * @param jsonData Null-terminated JSON array describing the players.
 * @return Vector of initialized players.
 * @throw std::invalid_argument If the payload is empty or malformed.
 */
NODISCARD vector<uniwar::Player> initializePlayers(const char* jsonData);

/**
 * @brief Returns enemy territories adjacent to the player's territories.
 *
 * @param map Adjacency map of the player's own territories.
 * @return Set of not-owned frontier territories.
 */
NODISCARD const set<string> getNotOwnedFrontier(const map<string, vector<string>> & map);

/**
 * @brief Returns owned territories that border at least one enemy territory.
 *
 * @param map Adjacency map of the player's own territories.
 * @return Set of owned frontier territories.
 */
NODISCARD const set<string> getOwnedFrontier(const map<string, vector<string>> & map);


} // namespace uniwar

#endif // UNIWAR_LIB_H