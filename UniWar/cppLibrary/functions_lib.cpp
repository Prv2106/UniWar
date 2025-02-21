#include "functions_lib.hpp"
#include "uniwar_lib.hpp"
using json = nlohmann::json;
using namespace std;


static string jsonResult; // Variabile statica per evitare memory leak
enum class AttackOutcome {
    NONE, // Nessun Attacco
    CONTINUE, // Attacco effettuato ma senza conquistare i terriotri 
    CONQUERED // Attacco con conquista del territorio
};
// Funzione di attacco della cpu
/* La funzione di attacco della cpu segue questa logica: 
    - il cpu player attacca uno alla volta i territori del giocatore scegliendo il primo dei territori alla frontiera non posseduti che rispetta il seguente criterio (condizione di attacco):
        - il territorio deve avere un numero di carri armati inferiore o uguale rispetto al territorio della cpu (chiave della lista neighbors in cui si trova il territorio di frontiera)
    - nella "battaglia" la cpu attacca sempre con 3 carri armati 
      (questo significa che deciderà di non effettuare un attacco se non esiste un territorio che rispetta il criterio di sopra e che al contempo possegga 4 o più carri armati)

    - Se viene ingaggiato un attacco:
        - vengono lanciati 3 dati (generati 3 numeri casuali da 1 a 6) per la cpu e per quanto riguarda il giocatore un numero di dadi pari al numero di carri che possiede,
          nel territorio attaccato, fino a un massimo di 3 (si può difendere al massimo con 3 carri)
        - il lancio di ciascun dato della CPU è confrontato con quello dei dati del giocatore, si confrontano i valori maggiori ottenuti dado per dado:
            - es 1. se la cpu ottiene [4,5,5] e il giocatore ottiene [6,4,3] il confronto sarà [6 > 5, 4 < 5, 3 < 4] e la CPU perderà 1 carro armato mentre il giocatore 2
            - es 2. se la cpu lancia 3 dati e il giocatore 2: -> cpu [4,5,5] e giocatore [6,5] allora in questo caso il giocatore non perde alcun carro armato mentre la cpu ne perde 2

    - Una battaglia finisce se si verifica una delle seguenti condizioni:
        - il numero di carri armati associati al territorio attaccato diventa 0
        - la cpu decide di non continuare l'attacco (perché il numero di carri armati posseduti nel territorio con il quale ha effettuato l'attacco è diventato inferiore al numero di carri del territorio attaccato)

    - al termine della battaglia, in caso di conquista del territorio la cpu sposta il massimo numero possibile di carri nel territorio conquistato (il territorio con cui ha effettuato l'attacco resterà con 1 solo carro armato
      , di conseguenza almeno 1 carro sarà spostato nel territorio conquistato) questo è lo SPOSTAMENTO STRATEGICO
    - finito lo spostamento strategico la cpu invocherà la funzione "win" per capire se la condizione di vittoria è o meno soddisfatta
    - se win restituisce true allora viene restituito un json a C# con lo stato del terreno aggiornato e con un indicazione circa il fatto che la cpu ha vinto
    - se win restituisce false, la cpu può scegliere 2 strade:
        - continuare ad attaccare (ricomincia il ciclo di valutazione dei territori da attaccare da capo)
        - termina il proprio turno restituendo un json a C# contenente le informazioni sulla battaglia (se ciclando tra i vari territori non è possibile effettuare un altro attacco): 
            - viene creato un vettore di json in cui avremo un campo numerato "Battaglia" che permetterà di capire a quale battaglia si riferisce ciascun oggetto json,
              questo perché la cpu potrebbe ingaggiare più battaglie nello stesso turno
            - in ciascun oggetto json rappresentante una battaglia vengono memorizzati:
                - contesto di gioco (cioè le mappe)
                - risultati dei dadi lanciati (sia dalla CPU che dal giocatore, per permettere poi alla UI di riprodurre l'andamento della battaglia)
                - condizione di vittoria (booleano)
                - territorio attaccato
                - territorio attaccante
                - numero della battaglia
                - nomi dei gioatori coinvolti
    - possono essere effettuati al massimo 3 attacchi per turno
*/

const char* cpuAttack (const char* jsonData){
    clog << "----------------------------------------------------------------------------" << endl;
    clog << "Funzione cpuAttack:" << endl;

    vector<uniwar::Player> players = uniwar::initializePlayers(jsonData);
    uniwar::Player cpuPlayer = players[0]; // Si assume che il cpu player sia il primo
    uniwar::Player userPlayer = players[1];
    vector<json> battleResults;
    bool attackChecking = true;
    int ciclo = 0; // per il debug 
    AttackOutcome attackOutcome = AttackOutcome::NONE;

    while(true){
        clog << "Ciclo di attacco : " << ++ciclo << endl; // Debug
        // Recuperiamo i terriori di frontiera posseduti dalla cpu
        set<string> ownedFrontiers = uniwar::getOwnedFrontier(cpuPlayer.getNeighborsMap()); 

        for(const auto & cpuTerritory: ownedFrontiers){
            clog << "Valutazione del territorio della cpu:  " << cpuTerritory << " come potenziale territorio di attacco" << endl; // debug

            if(cpuPlayer.getTanksCount(cpuTerritory) < 4){ // Deve avere almeo 4 territori per attaccare
                clog << "Il territorio " << cpuTerritory << " non è idoneo per attaccare (" << cpuPlayer.getTanksCount(cpuTerritory) << ") carri armati" <<endl; // debug
                continue; 
            }
            clog << "Il territorio " << cpuTerritory << " è idoneo per attaccare (" << cpuPlayer.getTanksCount(cpuTerritory) << ") carri armati" <<endl; // debug

            // Se il territorio di frontiera posseduto dalla cpu ha almeno 4 carri armati significa che potenzialmente potrebbe attaccare. 
            // Adesso si deve verificare se esiste un territorio vicino per il quale valga la condizone di attacco
            // Scorriamo la lista dei vicini del potenziale territorio attaccante
            for(const auto & neighborTerritory: cpuPlayer.getNeighbors(cpuTerritory)){
                // Se il territorio appartiene alla cpu lo saltiamo
                if(cpuPlayer.getTanksMap().count(neighborTerritory)) continue; // count verifica se la chiave è presente nella mappa (restituisce 1 se presente, 0 altrimenti)

                // adesso dobbiamo verificare la condizione di attacco
                int userTanksCount = userPlayer.getTanksCount(neighborTerritory);
                int cpuTanksCount = cpuPlayer.getTanksCount(cpuTerritory);
                
                // Se il territorio del giocatore ha più carri armati del territorio attaccante della cpu allora non è soddisfatta la condizione di attacco e dobbiamo passare al vicino successivo
                while((userTanksCount <= cpuTanksCount) && (cpuTanksCount >= 4)){ 
                    clog << "Il Territorio del giocatore: " << neighborTerritory << " è idoneo per essere attaccato ("<< userTanksCount << ") carri armati" << endl; // debug
                    clog << "Ciclo while di battaglia: numero carri armati della cpu = " << cpuTanksCount << ", numero carri armati del giocatore = "<< userTanksCount << endl; // debug
                    
                    /** INIZIO DELLA BATTAGLIA **/
                    int cpuAttackDice[3];
                    int userDefenseDice[3] = {0,0,0};
                    int defenseDiceCount = uniwar::rollTheDice(cpuAttackDice,userDefenseDice,userTanksCount);

                    // confrontiamo i dadi 
                    int cpuLosses = 0, userLosses = 0;
                    uniwar::compareDice(cpuAttackDice,userDefenseDice,defenseDiceCount,cpuLosses,userLosses);

                    clog << "Numero carri armati per il territorio attaccante della cpu (prima dell'attacco) = " << cpuTanksCount << endl; //debug
                    clog << "Numero carri armati per il territorio attaccato del giocatore (prima dell'attacco) = " << userTanksCount << endl; //debug

                    // A questo punto bisogna aggiornare il numero di carri armati della cpu e del giocatore dopo l'attacco
                    cpuPlayer.modifyTankCount(cpuTerritory,cpuTanksCount - cpuLosses);
                    userPlayer.modifyTankCount(neighborTerritory,userTanksCount - userLosses);

                    // Aggiorniamo il numero di carri armati della cpu e quello del giocatore
                    userTanksCount = userPlayer.getTanksCount(neighborTerritory);
                    cpuTanksCount= cpuPlayer.getTanksCount(cpuTerritory);

                    clog << "Numero carri armati per il territorio attaccante della cpu (dopo dell'attacco) = " << cpuTanksCount << endl; //debug
                    clog << "Numero carri armati per il territorio attaccato del giocatore (dopo dell'attacco) = " << userTanksCount << endl; //debug

                    bool win = false;

                    // Verifichiamo se il giocatore ha perso il territorio e in caso affermativo aggiorniamo il contesto di gioco e verifichiamo se la cpu ha vinto
                    if(userPlayer.getTanksCount(neighborTerritory) == 0){
                        clog << userPlayer.getName() << " ha perso il terriotio " << neighborTerritory << endl; // debug

                        // Assegnamo il territorio conquistato alla cpu ed effettuiamo lo SPOSTAMENTO STRATEGICO
                        // In particolare, al nuovo territorio assegnamo come numero di carri armati il numero di carri armati del territorio attaccante -1 (perché al territorio attaccante deve restare 1 carro armato)
                        cpuPlayer.addTerritory(neighborTerritory,userPlayer.getNeighbors(neighborTerritory), cpuPlayer.getTanksCount(cpuTerritory) -1);
                        // aggiorniamo il numero di carri armati del territorio attaccante
                        cpuPlayer.modifyTankCount(cpuTerritory,1);
                        // rimuoviamo il territorio perso dal giocatore
                        userPlayer.removeTerritory(neighborTerritory);
                        attackOutcome = AttackOutcome::CONQUERED;
                        clog << "Dopo lo spostamento strategico, il territorio della cpu  " << cpuTerritory << " ha " << cpuPlayer.getTanksCount(cpuTerritory) << " carri armati" << endl; // debug

                        if(uniwar::win(uniwar::getTerritoriesFromMap(cpuPlayer.getTanksMap()))){
                            win = true;
                        }
                        
                       
                    } // Fine if per verificare la condizione di perdita del territorio

                    clog << "Aggiorniamo il json relativo alla battaglia " << battleResults.size() + 1 << endl;
                    // Aggiorniamo il json da restituire a C#
                    json battleJson;
                    battleJson["Battle"] = battleResults.size() + 1;
                    battleJson["AttackingPlayer"] = cpuPlayer.getName();
                    battleJson["DefendingPlayer"] = userPlayer.getName();
                    battleJson["Win"] = win;
                    battleJson["DiceCPU"] = {cpuAttackDice[0], cpuAttackDice[1], cpuAttackDice[2]};
                    battleJson["DicePlayer"] = {userDefenseDice[0], userDefenseDice[1], userDefenseDice[2]}; // si assume 0 per dado non lanciato
                    battleJson["AttackingTerritory"] = cpuTerritory;  
                    battleJson["DefendingTerritory"] = neighborTerritory;
                    battleJson["AttackingNeighborsMap"] = cpuPlayer.getNeighborsMap();
                    battleJson["DefendingNeighborsMap"] = userPlayer.getNeighborsMap();
                    battleJson["AttackingTanksCountMap"] = cpuPlayer.getTanksMap();
                    battleJson["DefendingTanksCountMap"] = userPlayer.getTanksMap();
                    battleJson["LossesCPU"] = cpuLosses;
                    battleJson["LossesPlayer"] = userLosses;
                    // Aggiungiamo il json della battaglia nel vettore di json (perché potrebbero essere ingaggiate anche più battaglie)
                    battleResults.push_back(battleJson);

                    if(win || (battleResults.size() == 3)){ 
                        jsonResult = json(battleResults).dump();
                        return jsonResult.c_str();
                    }

                    if(attackOutcome == AttackOutcome::CONQUERED){ // Siccome una conquista è stata fatta usciamo dal while della battaglia per poter valutare un nuovo attacco considerando anche il nuovo territorio
                        break;
                    }

                } // fine while battaglia con territorio

                if(attackOutcome == AttackOutcome::CONQUERED){ // Se è stato conquistato un territorio bisogna ripetere il ciclo for più esterno perché bisogna riconsiderare l'assetto della cpu
                    break;
                }
            } // fine for per scorrere i vicini
            if(attackOutcome == AttackOutcome::CONQUERED){ // Se è stato conquistato un territorio bisogna ripetere il ciclo for più esterno perché bisogna riconsiderare l'assetto della cpu
                break;
            }
        } // Fine for per trovare i territori potenziali attaccanti
        
        // Se non sono stati conquistati territori, alla fine del for più esterno finirà anche la fase di attacco della cpu perché vorrà dire che non può effettuare altri attacchi
        if(attackOutcome == AttackOutcome::NONE || attackOutcome == AttackOutcome::CONTINUE){
            break;
        }
    }

    // Se la cpu non ha vinto restituisce il risultato della battaglia qui (potrebbe anche non aver attaccato affatto)
    jsonResult = json(battleResults).dump();
    return jsonResult.c_str();
}





// Funzione per il rinforzo dei territori della CPU
/*  La funzione di rinforzo segue questa logica: il cpu player assegna i nuovi carri armati tramite un round robin a ciascuno dei suoi territori di frontiera.
    In particolare, da precedenza ai territori di frontiera con 1 solo carro armato per poi applicare un round robin a tutti quelli che hanno un numero di carri armati inferiore al numero massimo di carri assegnati ad un territorio
    - per prima cosa si recuperano i territori di frontiera posseduti
    - a questo punto si fa un primo ciclo dove si assegna un carro armato alla volta ai territori di frontiera che ne possiedono solo 1
    - poi si fa un ulteriore ciclo nel quale si assegna un carro armato alla volta ai territori di frontiera il cui numero di carri armati è inferiore al numero massimo di carri posseduti da un territorio di frontiera
    - infine, si fa un round robin finale in cui viene assegnato un carro armato ciascuno
    Nota: i vari cicli vengono fatti fino a quando i nuovi carri disponibili terminano

*/
const char* reinforcement (const char* jsonData, int newTanks){
    clog << "----------------------------------------------------------------------------" << endl;
    clog << "Funzione reinforcement:" << endl;
    vector<uniwar::Player> players = uniwar::initializePlayers(jsonData);
    
    uniwar::Player& cpuPlayer = players[0]; 
    set<string> ownedFrontiers = uniwar::getOwnedFrontier(cpuPlayer.getNeighborsMap());
    int max = 0;
    // Recuperiamo il numero massimo di carri armati
    for(const auto& territory: ownedFrontiers){
        if(cpuPlayer.getTanksCount(territory) > max)
            max = cpuPlayer.getTanksCount(territory);
    }

    clog << "Nuovi carri a disposizione per la cpu: " << newTanks << endl;
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

    // Una volta aggiornato il contesto restituiamo il risultato a C# andando a creare il nuovo json
    // Creiamo un oggetto json e inseriamo i campi
    json updatedPlayerData; 
    updatedPlayerData["PlayerId"] = cpuPlayer.getName();
    updatedPlayerData["Neighbors"] = cpuPlayer.getNeighborsMap();
    updatedPlayerData["Tanks"] = cpuPlayer.getTanksMap();

    // Convertiamo l'oggetto JSON in una stringa JSON
    jsonResult = json(updatedPlayerData).dump();  // Serializziamo i dati in formato stringa

    // La funzione c_str() della classe std::string restituisce un puntatore a carattere (const char*)
    return jsonResult.c_str();

}

// Funzione che permette a C# di verificare se il giocatore ha vinto o meno
bool winCheck (const char* jsonData){
    vector<uniwar::Player> players = uniwar::initializePlayers(jsonData);
    return uniwar::win(uniwar::getTerritoriesFromMap(players[0].getTanksMap()));
}