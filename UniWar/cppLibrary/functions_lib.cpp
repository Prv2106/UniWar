#include "functions_lib.hpp"
#include "uniwar_lib.hpp"
using json = nlohmann::json;
using namespace std;


static string jsonResult;  // Static buffer to avoid memory leaks across calls
enum class AttackOutcome {
    NONE,  // No attack performed
    CONTINUE,  // Attack performed without conquering the territory
    CONQUERED  // Attack performed and territory conquered
};

/**
 * @brief Executes the CPU attack phase.
 *
 * Algorithm:
 * - The CPU evaluates its owned frontier territories in order and selects the
 *   first one that can attack. A territory can attack when it holds at least
 *   four tanks and borders an enemy territory whose tank count is lower than or
 *   equal to its own.
 * - Each battle rolls three dice for the CPU and one die per defending tank,
 *   up to three dice. Dice are compared pairwise in descending order; ties are
 *   won by the defender.
 * - A battle ends when the defending territory reaches zero tanks, when the
 *   attacker no longer outnumbers the defender, or when the attacker drops
 *   below four tanks.
 * - On conquest, all but one tank move to the conquered territory (strategic
 *   shift) and the win condition is evaluated.
 * - When the victory condition is not met, the CPU either starts another
 *   attack cycle or ends its turn after at most three battles.
 *
 * The returned JSON array contains, for each battle: the game context maps,
 * rolled dice, win flag, attacking and defending territories, battle index,
 * and losses for both players.
 *
 * @param jsonData JSON payload with the CPU first and the user second.
 * @return Null-terminated JSON string, ``"Pass"`` when no attack is possible,
 * or ``"{}"`` on error.
 */
const char* cpuAttack (const char* jsonData){
    clog << "----------------------------------------------------------------------------" << endl;
    clog << "Funzione cpuAttack:" << endl;
    try{
        srand(time(0)); // Initialize the seed
        vector<uniwar::Player> players = uniwar::initializePlayers(jsonData);
        uniwar::Player cpuPlayer = players[0]; // The CPU is assumed to be first
        uniwar::Player userPlayer = players[1];
        vector<json> battleResults;
        bool attackChecking = true;
        int ciclo = 0; // Debug counter
        AttackOutcome attackOutcome = AttackOutcome::NONE;

        while(true){
            clog << "Ciclo di attacco : " << ++ciclo << endl; // Debug output
            attackOutcome = AttackOutcome::NONE;

            // Retrieve the frontier territories owned by the CPU.
            set<string> ownedFrontiers = uniwar::getOwnedFrontier(cpuPlayer.getNeighborsMap()); 

            for(const auto & cpuTerritory: ownedFrontiers){
                clog << "Valutazione del territorio della cpu:  " << cpuTerritory << " come potenziale territorio di attacco" << endl; // Debug output

                if(cpuPlayer.getTanksCount(cpuTerritory) < 4){ // At least four tanks are required to attack
                    clog << "Il territorio " << cpuTerritory << " non è idoneo per attaccare (" << cpuPlayer.getTanksCount(cpuTerritory) << ") carri armati" <<endl; // Debug output
                    continue; 
                }
                clog << "Il territorio " << cpuTerritory << " è idoneo per attaccare (" << cpuPlayer.getTanksCount(cpuTerritory) << ") carri armati" <<endl; // Debug output

                // A frontier territory with at least four tanks is a potential
                // attacker. Verify whether a neighboring territory satisfies
                // the attack condition.
                for(const auto & neighborTerritory: cpuPlayer.getNeighbors(cpuTerritory)){
                    // Skip territories already owned by the CPU.
                    if(cpuPlayer.getTanksMap().count(neighborTerritory)) continue; // 1 when present, 0 otherwise

                    // Verify the attack condition.
                    int userTanksCount = userPlayer.getTanksCount(neighborTerritory);
                    int cpuTanksCount = cpuPlayer.getTanksCount(cpuTerritory);
                    
                    // Continue only while the defender does not outnumber the
                    // attacker and the attacker still has enough tanks.
                    while((userTanksCount <= cpuTanksCount) && (cpuTanksCount >= 4) && (cpuTanksCount > 3)){ 
                        clog << "Il Territorio del giocatore: " << neighborTerritory << " è idoneo per essere attaccato ("<< userTanksCount << ") carri armati" << endl; // Debug output
                        clog << "Ciclo while di battaglia: numero carri armati della cpu = " << cpuTanksCount << ", numero carri armati del giocatore = "<< userTanksCount << endl; // Debug output
                        
                        /** BATTLE START **/
                        attackOutcome = AttackOutcome::CONTINUE;
                        int cpuAttackDice[3];
                        int userDefenseDice[3] = {0,0,0};
                        int defenseDiceCount = uniwar::rollTheDice(cpuAttackDice,userDefenseDice,userTanksCount);

                        // Compare the dice.
                        int cpuLosses = 0, userLosses = 0;
                        uniwar::compareDice(cpuAttackDice,userDefenseDice,defenseDiceCount,cpuLosses,userLosses);

                        clog << "Numero carri armati per il territorio attaccante della cpu (prima dell'attacco) = " << cpuTanksCount << endl; // Debug output
                        clog << "Numero carri armati per il territorio attaccato del giocatore (prima dell'attacco) = " << userTanksCount << endl; // Debug output

                        // Update the tank counts after the attack.
                        cpuPlayer.modifyTankCount(cpuTerritory,cpuTanksCount - cpuLosses);
                        userPlayer.modifyTankCount(neighborTerritory,userTanksCount - userLosses);

                        // Refresh the local tank counts.
                        userTanksCount = userPlayer.getTanksCount(neighborTerritory);
                        cpuTanksCount= cpuPlayer.getTanksCount(cpuTerritory);

                        clog << "Numero carri armati per il territorio attaccante della cpu (dopo dell'attacco) = " << cpuTanksCount << endl; // Debug output
                        clog << "Numero carri armati per il territorio attaccato del giocatore (dopo dell'attacco) = " << userTanksCount << endl; // Debug output

                        bool win = false;

                        // Check whether the defending territory has fallen; if so,
                        // update the game state and evaluate the victory condition.
                        if(userPlayer.getTanksCount(neighborTerritory) == 0){
                            clog << userPlayer.getName() << " ha perso il terriotio " << neighborTerritory << endl; // Debug output

                            // Assign the conquered territory to the CPU and perform
                            // the strategic shift: the new territory receives all
                            // attacker tanks minus one, which remains behind.
                            cpuPlayer.addTerritory(neighborTerritory,userPlayer.getNeighbors(neighborTerritory), cpuPlayer.getTanksCount(cpuTerritory) -1);
                            // Leave exactly one tank in the attacking territory.
                            cpuPlayer.modifyTankCount(cpuTerritory,1);
                            // Remove the lost territory from the player.
                            userPlayer.removeTerritory(neighborTerritory);
                            attackOutcome = AttackOutcome::CONQUERED;

                            clog << "Dopo lo spostamento strategico, il territorio della cpu  " << cpuTerritory << " ha " << cpuPlayer.getTanksCount(cpuTerritory) << " carri armati" << endl; // Debug output

                            if(uniwar::win(uniwar::getTerritoriesFromMap(cpuPlayer.getTanksMap()))){
                                win = true;
                            }
                            
                        
                        } // End of territory-loss check

                        clog << "Aggiorniamo il json relativo alla battaglia " << battleResults.size() + 1 << endl;
                        // Build the JSON object returned to C#.
                        json battleJson;
                        battleJson["Battle"] = battleResults.size() + 1;
                        battleJson["AttackingPlayer"] = cpuPlayer.getName();
                        battleJson["DefendingPlayer"] = userPlayer.getName();
                        battleJson["Win"] = win;
                        battleJson["DiceCPU"] = {cpuAttackDice[0], cpuAttackDice[1], cpuAttackDice[2]};
                        battleJson["DicePlayer"] = {userDefenseDice[0], userDefenseDice[1], userDefenseDice[2]}; // 0 marks a die not rolled
                        battleJson["AttackingTerritory"] = cpuTerritory;  
                        battleJson["DefendingTerritory"] = neighborTerritory;
                        battleJson["AttackingNeighborsMap"] = cpuPlayer.getNeighborsMap();
                        battleJson["DefendingNeighborsMap"] = userPlayer.getNeighborsMap();
                        battleJson["AttackingTanksCountMap"] = cpuPlayer.getTanksMap();
                        battleJson["DefendingTanksCountMap"] = userPlayer.getTanksMap();
                        battleJson["LossesCPU"] = cpuLosses;
                        battleJson["LossesPlayer"] = userLosses;
                        // Append the battle object; multiple battles may occur per turn.
                        battleResults.push_back(battleJson);

                        if(win || (battleResults.size() == 3)){ 
                            jsonResult = json(battleResults).dump();
                            return jsonResult.c_str();
                        }

                        if(attackOutcome == AttackOutcome::CONQUERED){ // Re-evaluate attacks after a conquest.
                            break;
                        }

                    } // End of per-territory battle loop

                    if(attackOutcome == AttackOutcome::CONQUERED){ // Restart the outer loop after a conquest.
                        break;
                    }
                } // End of neighbor loop
                if(attackOutcome == AttackOutcome::CONQUERED){ // Restart the outer loop after a conquest.
                    break;
                }
            } // End of potential-attacker loop
            
            // Without a conquest the CPU cannot continue attacking, so the phase ends.
            if(attackOutcome == AttackOutcome::NONE || attackOutcome == AttackOutcome::CONTINUE){
                break;
            }
        } // End of while(true)

        // The CPU decided not to attack.
        if (attackOutcome == AttackOutcome::NONE){
            return "Pass";
        }
        
        // The CPU attacked but did not win: return the battle results.
        jsonResult = json(battleResults).dump();
        return jsonResult.c_str();

    }catch(const invalid_argument &e){
        cerr << "Errore critico in cpuAttack: " << e.what() << endl;
        return "{}";  // Empty JSON fallback
    } catch(const out_of_range &e){
        cerr << "Errore critico in cpuAttack: " << e.what() << endl;
        return "{}";  // Empty JSON fallback
    } catch (const exception& e) {
        cerr << "Errore critico in cpuAttack: " << e.what() << endl;
        return "{}";  // Empty JSON fallback
    }
}





/**
 * @brief Reinforces the CPU frontier territories.
 *
 * Tanks are distributed with three round-robin passes to keep the allocation
 * balanced:
 * - First pass: one tank at a time to frontier territories holding one tank.
 * - Second pass: one tank at a time to frontier territories below the maximum
 *   tank count found among frontier territories.
 * - Third pass: a pure round-robin over all frontier territories.
 *
 * The next pass starts only when tanks remain after the previous one.
 *
 * @param jsonData JSON payload describing the CPU state.
 * @param newTanks Number of tanks available for reinforcement.
 * @return Null-terminated JSON string with the updated CPU state, or ``"{}"``
 * on error.
 */
const char* reinforcement (const char* jsonData, int newTanks){
    clog << "----------------------------------------------------------------------------" << endl;
    clog << "Funzione reinforcement:" << endl;
    try{
        vector<uniwar::Player> players = uniwar::initializePlayers(jsonData);    
        uniwar::Player& cpuPlayer = players[0]; 
        set<string> ownedFrontiers = uniwar::getOwnedFrontier(cpuPlayer.getNeighborsMap());
        int max = 0;
        // Determine the maximum tank count among frontier territories.
        for(const auto& territory: ownedFrontiers){
            if(cpuPlayer.getTanksCount(territory) > max)
                max = cpuPlayer.getTanksCount(territory);
        }
    
        clog << "Nuovi carri a disposizione per la cpu: " << newTanks << endl;
        clog << "\nPrimo round robin (precedenza ai territori con 1 carro armato):\n" <<endl;
        for(auto & territory: ownedFrontiers){
            int tankCount = cpuPlayer.getTanksCount(territory);
            if(tankCount== 1 && newTanks > 0){
                clog << "Numero carri (Prima della modifica) per territorio " << territory << ": " << tankCount << endl;
                cpuPlayer.modifyTankCount(territory, tankCount + 1);
                newTanks--;
                clog << "Numero carri (Dopo la modifica) per territorio " << territory << ": " << cpuPlayer.getTanksCount(territory) << endl;
                clog << "Nuovi carri ancora da assegnare: " << newTanks << endl;
            }
            if(newTanks == 0)
                break;
            
        }
    
        clog << "\nSecondo round robin:\n" <<endl;
        for(auto & territory: ownedFrontiers){
            int tankCount = cpuPlayer.getTanksCount(territory);
            if((tankCount < max) && newTanks > 0){
                clog << "Numero carri (Prima della modifica) per territorio " << territory << ": " << tankCount << endl;
                cpuPlayer.modifyTankCount(territory, tankCount + 1);
                newTanks--;
                clog << "Numero carri (Dopo la modifica) per territorio " << territory << ": " << cpuPlayer.getTanksCount(territory) << endl;
                clog << "Nuovi carri ancora da assegnare: " << newTanks << endl;
            }
            if(newTanks == 0)
                break;
        }
    
        clog << "\nRound robin con while:\n" <<endl;
        while(newTanks > 0){
            for(auto & territory: ownedFrontiers){
                int tankCount = cpuPlayer.getTanksCount(territory);
                clog << "Numero carri (Prima della modifica) per territorio " << territory << ": " << tankCount << endl;
                cpuPlayer.modifyTankCount(territory, tankCount + 1);
    
                newTanks--;
                clog << "Numero carri (Dopo la modifica) per territorio " << territory << ": " << cpuPlayer.getTanksCount(territory) << endl;
                clog << "Nuovi carri ancora da assegnare: " << newTanks << endl;
                if(newTanks == 0)
                    break;
            }
    
        }
    
        // Serialize the updated context back to C#.
        json updatedPlayerData; 
        updatedPlayerData["PlayerId"] = cpuPlayer.getName();
        updatedPlayerData["Neighbors"] = cpuPlayer.getNeighborsMap();
        updatedPlayerData["Tanks"] = cpuPlayer.getTanksMap();
    
        // Convert the JSON object to a string.
        jsonResult = json(updatedPlayerData).dump();
    
        // c_str() returns a pointer to the string's character buffer.
        return jsonResult.c_str();
    

    } catch(const invalid_argument &e){
        cerr << "Errore critico in reinforcement: " << e.what() << endl;
        return "{}";  // Empty JSON fallback
    } catch(const out_of_range &e){
        cerr << "Errore critico in reinforcement: " << e.what() << endl;
        return "{}";  // Empty JSON fallback
    } catch (const exception& e) {
        cerr << "Errore critico in reinforcement: " << e.what() << endl;
        return "{}";  // Empty JSON fallback
    }
}

/**
 * @brief Checks the victory condition for the player.
 *
 * @param jsonData JSON payload describing the player state.
 * @return ``true`` if the player has won, ``false`` otherwise or on error.
 */
bool winCheck (const char* jsonData) {
    try {
        vector<uniwar::Player> players = uniwar::initializePlayers(jsonData);
        return uniwar::win(uniwar::getTerritoriesFromMap(players[0].getTanksMap()));        
    } catch(const invalid_argument &e){
        cerr << "Errore critico in winCheck: " << e.what() << endl;
        return false;  
    } catch (const exception& e) {
        cerr << "Errore critico in winCheck: " << e.what() << endl;
        return false; 
    }
}