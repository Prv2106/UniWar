/*
In C#, la direttiva using System.Runtime.InteropServices; serve a importare il namespace System.Runtime.InteropServices, che contiene classi e metodi per interagire con il codice nativo (ad esempio, codice C o C++). 
Questo namespace è fondamentale quando si ha bisogno di fare interoperabilità tra il codice gestito (C#) e il codice non gestito (come quello C++).
*/
using System.Runtime.InteropServices;
using System.Text.Json;   


namespace UniWar {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
            // metodo che, durante la compilazione, collega la logica C# agli elementi XAML 
        }

        private async void OnNuovaPartitaButtonClicked(object sender, EventArgs e) {
            // PushAsync: metodo della proprietà Navigation per navigare verso la pagina Integration e aggiungerla alla stack di navigazione.
            // N.B: await è usato perché PushAsync è un metodo asincrono.
            await Navigation.PushAsync(new views.ChooseTankColor());
        }

       
        /************ CODICE DI TEST PER LE FUNZIONALITA' C++ ***************/


       
        /****************** Codice per testare la vittoria ******************/
        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void testWin();


        private async void ProvaVittoria(object sender, EventArgs e) {
            testWin();
        }


        /****************** Codice per testare la frontiera ******************/

        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void testFrontiers();


        private async void ProvaFrontiere(object sender, EventArgs e) {
            testFrontiers();
        }



        /***************** Codice per testare il rinforzo *****************/
        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void testRinforzi(int newCount);


        private async void TestRinforzo(object sender, EventArgs e) {
            Console.WriteLine("Prova test Rinforzo");
            testRinforzi(1);
        }



    /////////////////////////////// IMPLEMENTAZIONE FUNZIONI EFFETTIVE 

        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr reinforcement (string jsonData, int newTanks);

        private async void OnReinforcement(object sender, EventArgs e) {

            // Sintassi con Object Initializer (viene chiamato il costruttore senza parametri in modo implicito) 
            // TODO: SOSTITUIRE CON I VALORI EFFETTIVI DELLE CLASSI
            List<MapData> playersMaps = new List<MapData>
            {
                new MapData
                {
                    PlayerId = "cpuPlayer",
                    Neighbors = new Dictionary<string, List<string>>
                    {
                        { "Alaska", new List<string> { "Alberta", "TerritoriDelNordOvest" } },
                        { "Alberta", new List<string> { "Alaska", "StatiUnitiOccidentali" } },
                        { "StatiUnitiOccidentali", new List<string> { "Alberta", "StatiUnitiOrientali" } },
                        { "StatiUnitiOrientali", new List<string> { "StatiUnitiOccidentali", "Ontario" } },
                        { "AmericaCentrale", new List<string> { "StatiUnitiOrientali", "Ontario" } },
                        { "Ontario", new List<string> { "StatiUnitiOrientali", "StatiUnitiOrientali", "TerritoriDelNordOvest", "Groenlandia", "Quebec", "Alberta" } },
                        { "TerritoriDelNordOvest", new List<string> { "Ontario", "Alaska", "Groenlandia" } },
                        { "Groenlandia", new List<string> { "Ontario", "TerritoriDelNordOvest", "Quebec" } }
                    },
                    Tanks = new Dictionary<string, int>
                    {
                        { "Alaska", 3 },
                        { "Alberta", 2 },
                        { "StatiUnitiOccidentali", 4 },
                        { "StatiUnitiOrientali", 5 },
                        { "AmericaCentrale", 2 },
                        { "Ontario", 7 },
                        { "TerritoriDelNordOvest", 3 },
                        { "Groenlandia", 5 }
                    }
                }
            };



            // Converte l'oggetto playersMaps (una lista di oggetti MapData) in una stringa JSON formattata
            // WriteIndented = true opzione che formatta il JSON con spazi e indentazione per renderlo più leggibile
            string jsonData = JsonSerializer.Serialize(playersMaps, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("JSON inviato a C++:\n" + jsonData);

            // reinforcement restituisce un puntatore (char*) ma è un puntatore a memoria non gestita (cioè non gestita dal GC di .NET).
            // per questo usiamo IntPtr che rappresenta una struttura C# che viene utilizzata per memorizzare degli indirizzi di memoria (per la memoria non gestita).
            // su IntPtr possono essere utilizzati i metodi della classe Marshal di C#.

            IntPtr resultPtr = reinforcement(jsonData, 10); //Stiamo passando il contesto del giocatore della cpu insieme ai nuovi carri che ha a disposizione

            // In questo caso, usiamo Marshal.PtrToStringUTF8(resultPtr) per copiare la stringa della memoria non gestita (C++) in una stringa gestita dal GC di C#
            // Marshal.PtrToStringUTF8(resultPtr) può restituire null, quindi usiamo ?? (operatore di null-coalescing) per far si che in tal caso a resultJson venga assegnata una stringa vuota anziché null
            string resultJson = Marshal.PtrToStringUTF8(resultPtr) ?? string.Empty;



            
            // Nota: non ci occupiamo di deallocare la memoria non gestita perché nella funzione C++ usiamo una stringa statica 
            // che quindi viene allocata nella memoria statica e persiste per tutta la durata del rpogramma (non abbiamo problemi di memory leak).

            // JsonSerializer.Deserialize<MapData>(resultJson) converte (deserializza) quella stringa JSON in una mappa (MapData)
            // updatedMaps diventa quindi MapData (che quindi può essere utilizzata per aggiornare le classi in C#).
            var updatedMap = JsonSerializer.Deserialize<MapData>(resultJson);

            // Per il debug
            Console.WriteLine("JSON aggiornato:\n" + JsonSerializer.Serialize(updatedMap, new JsonSerializerOptions { WriteIndented = true }));

            // TODO: Aggiungere il codice per copiare gli aggiornamenti nelle classi C#


        }




        // Funzione di attacco


        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cpuAttack (string jsonData);

        private async void OnCpuAttack(object sender, EventArgs e) {

            // Sintassi con Object Initializer (viene chiamato il costruttore senza parametri in modo implicito) 
            // TODO: SOSTITUIRE CON I VALORI EFFETTIVI DELLE CLASSI
            List<MapData> playersMaps = new List<MapData>
            {
                new MapData
                {
                    PlayerId = "cpuPlayer",
                    Neighbors = new Dictionary<string, List<string>>
                    {
                        { "Alaska", new List<string> { "Alberta", "TerritoriDelNordOvest" } },
                        { "Alberta", new List<string> { "Alaska", "StatiUnitiOccidentali" } },
                        { "StatiUnitiOccidentali", new List<string> { "Alberta", "StatiUnitiOrientali" } },
                        { "StatiUnitiOrientali", new List<string> { "StatiUnitiOccidentali", "Ontario" } },
                        { "AmericaCentrale", new List<string> { "StatiUnitiOrientali", "Ontario" } },
                        { "Ontario", new List<string> { "StatiUnitiOrientali", "AmericaCentrale", "TerritoriDelNordOvest", "Groenlandia", "Quebec" } },
                        { "TerritoriDelNordOvest", new List<string> { "Ontario", "Alaska", "Groenlandia" } },
                        { "Groenlandia", new List<string> { "Ontario", "TerritoriDelNordOvest", "Quebec","Islanda" } }
                    },
                    Tanks = new Dictionary<string, int>
                    {
                        { "Alaska", 3 },
                        { "Alberta", 2 },
                        { "StatiUnitiOccidentali", 4 },
                        { "StatiUnitiOrientali", 5 },
                        { "AmericaCentrale", 2 },
                        { "Ontario", 7 },
                        { "TerritoriDelNordOvest", 3 },
                        { "Groenlandia", 5 }
                    }
                },
                new MapData
                {
                    PlayerId = "player",
                    Neighbors = new Dictionary<string, List<string>>
                    {
                        { "Venezuela", new List<string> { "Brasile", "Perù", "AmericaCentrale"} },
                        { "Brasile", new List<string> { "Venezuela", "Perù", "Argentina", "AfricaDelNord" } },
                        { "Perù", new List<string> { "Venezuela", "Brasile", "Argentina" } },
                        { "Quebec", new List<string> { "Ontario", "StatiUnitiOrientali", "Groenlandia" } },
                        { "Islanda", new List<string> { "Groenlandia", "Scandinavia", "GranBretagna" } },
                        { "Scandinavia", new List<string> { "Ucraina", "GranBretagna", "Islanda" } },
                        { "GranBretagna", new List<string> { "Islanda", "EuropaOccidentale", "EuropaSettentrionale" } }
                    },
                    Tanks = new Dictionary<string, int>
                    {
                        { "Venezuela", 4 },
                        { "Brasile", 30 },
                        { "Perù", 30 },
                        { "Quebec", 5 },
                        { "Islanda", 4 },
                        { "Scandinavia", 30 },
                        { "GranBretagna",30 }
                    }
                }

            };



            // Converte l'oggetto playersMaps (una lista di oggetti MapData) in una stringa JSON formattata
            // WriteIndented = true opzione che formatta il JSON con spazi e indentazione per renderlo più leggibile
            string jsonData = JsonSerializer.Serialize(playersMaps, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("JSON inviato a C++:\n" + jsonData);

            // cpuAttack restituisce un puntatore (char*) ma è un puntatore a memoria non gestita (cioè non gestita dal GC di .NET).
            // per questo usiamo IntPtr che rappresenta una struttura C# che viene utilizzata per memorizzare degli indirizzi di memoria (per la memoria non gestita).
            // su IntPtr possono essere utilizzati i metodi della classe Marshal di C#.

            IntPtr resultPtr = cpuAttack(jsonData); //Stiamo passando il contesto dei giocatori

            

            // In questo caso, usiamo Marshal.PtrToStringUTF8(resultPtr) per copiare la stringa della memoria non gestita (C++) in una stringa gestita dal GC di C#
            // Marshal.PtrToStringUTF8(resultPtr) può restituire null, quindi usiamo ?? (operatore di null-coalescing) per far si che in tal caso a resultJson venga assegnata una stringa vuota anziché null
            string resultJson = Marshal.PtrToStringUTF8(resultPtr) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(resultJson) || resultJson == "[]") {
                Console.WriteLine("La CPU ha deciso di non attaccare.");
                return; // TODO: fare qualcos'altro se necessario
            }
            else{
                    List<BattleResult> battleResults = JsonSerializer.Deserialize<List<BattleResult>>(resultJson);
                    // Per il debug
                    Console.WriteLine("JSON aggiornato:\n" + JsonSerializer.Serialize(battleResults, new JsonSerializerOptions { WriteIndented = true }));

            }

        }

        
        


        // Funzione per fare il check della vittoria

        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool winCheck (string jsonData);

        private async void OnWinCheck(object sender, EventArgs e) {

            // Sintassi con Object Initializer (viene chiamato il costruttore senza parametri in modo implicito) 
            // TODO: SOSTITUIRE CON I VALORI EFFETTIVI DELLE CLASSI
            List<MapData> winnerMap = new List<MapData>
            {
                new MapData
                {
                    PlayerId = "Player",
                    Neighbors = new Dictionary<string, List<string>>
                    {
                        { "Alaska", new List<string> { "Alberta", "TerritoriDelNordOvest" } },
                        { "Alberta", new List<string> { "Alaska", "StatiUnitiOccidentali" } },
                        { "StatiUnitiOccidentali", new List<string> { "Alberta", "StatiUnitiOrientali" } },
                        { "StatiUnitiOrientali", new List<string> { "StatiUnitiOccidentali", "Ontario" } },
                        { "AmericaCentrale", new List<string> { "StatiUnitiOrientali", "Ontario" } },
                        { "Ontario", new List<string> { "StatiUnitiOrientali", "AmericaCentrale", "TerritoriDelNordOvest", "Groenlandia", "Quebec" } },
                        { "TerritoriDelNordOvest", new List<string> { "Ontario", "Alaska", "Groenlandia" } },
                        { "Groenlandia", new List<string> { "Ontario", "TerritoriDelNordOvest", "Quebec" } },
                        { "Quebec", new List<string> { "Ontario", "Groenlandia", "StatiUnitiOrientali" } },
                        { "Venezuela", new List<string> { "Perù", "Brasile", "Argentina" } },
                        { "Perù", new List<string> { "Venezuela", "Brasile", "Argentina" } },
                        { "Brasile", new List<string> { "Venezuela", "Perù", "Argentina", "Congo", "AfricaDelSud" } },
                        { "Argentina", new List<string> { "Venezuela", "Perù", "Brasile", "Congo" } },
                        { "AfricaDelNord", new List<string> { "Egitto", "AfricaOrientale", "Congo" } },
                        { "Egitto", new List<string> { "AfricaDelNord", "AfricaOrientale", "MedioOriente" } },
                        { "AfricaOrientale", new List<string> { "AfricaDelNord", "Egitto", "Congo", "Madagascar" } },
                        { "Congo", new List<string> { "AfricaDelNord", "Egitto", "Brasile", "AfricaOrientale", "AfricaDelSud" } },
                        { "AfricaDelSud", new List<string> { "Congo", "Brasile", "Madagascar" } },
                        { "Madagascar", new List<string> { "AfricaDelSud", "AfricaOrientale" } },
                        { "Kamchatka", new List<string> { "Jacuzia", "Giappone", "Cina" } },
                        { "Jacuzia", new List<string> { "Kamchatka", "Siberia" } },
                        { "Cita", new List<string> { "Giappone", "Cina", "Siam", "India" } },
                        { "Giappone", new List<string> { "Kamchatka", "Cita", "Cina" } },
                        { "Cina", new List<string> { "Giappone", "Kamchatka", "Siam", "India", "Urali" } },
                        { "Siam", new List<string> { "Cina", "India", "MedioOriente" } },
                        { "India", new List<string> { "Siam", "Cina", "MedioOriente", "Afghanistan" } },
                        { "MedioOriente", new List<string> { "India", "Cina", "Siam", "Afghanistan", "Urali", "AfricaOrientale" } },
                        { "Afghanistan", new List<string> { "India", "MedioOriente", "Urali", "Siberia" } },
                        { "Urali", new List<string> { "Cina", "MedioOriente", "Afghanistan", "Siberia", "Mongolia" } },
                        { "Siberia", new List<string> { "Jacuzia", "Afghanistan", "Urali", "Mongolia" } },
                        { "Mongolia", new List<string> { "Urali", "Siberia", "Cina" } }
                    },
                    Tanks = new Dictionary<string, int>
                    {
                        { "Alaska", 5 },
                        { "Alberta", 3 },
                        { "StatiUnitiOccidentali", 6 },
                        { "StatiUnitiOrientali", 4 },
                        { "AmericaCentrale", 2 },
                        { "Ontario", 7 },
                        { "TerritoriDelNordOvest", 3 },
                        { "Groenlandia", 5 },
                        { "Quebec", 4 },
                        { "Venezuela", 3 },
                        { "Perù", 2 },
                        { "Brasile", 4 },
                        { "Argentina", 3 },
                        { "AfricaDelNord", 5 },
                        { "Egitto", 2 },
                        { "AfricaOrientale", 3 },
                        { "Congo", 6 },
                        { "AfricaDelSud", 3 },
                        { "Madagascar", 2 },
                        { "Kamchatka", 4 },
                        { "Jacuzia", 3 },
                        { "Cita", 4 },
                        { "Giappone", 5 },
                        { "Cina", 7 },
                        { "Siam", 3 },
                        { "India", 6 },
                        { "MedioOriente", 4 },
                        { "Afghanistan", 3 },
                        { "Urali", 6 },
                        { "Siberia", 3 },
                        { "Mongolia", 4 }
                    }
                }
            };


            List<MapData> loserMap = new List<MapData>
            {
                new MapData
                {
                    PlayerId = "Player",
                    Neighbors = new Dictionary<string, List<string>>
                    {
                        { "Alaska", new List<string> { "Alberta", "TerritoriDelNordOvest" } },
                        { "Alberta", new List<string> { "Alaska", "StatiUnitiOccidentali" } },
                        { "StatiUnitiOccidentali", new List<string> { "Alberta", "StatiUnitiOrientali" } },
                        { "StatiUnitiOrientali", new List<string> { "StatiUnitiOccidentali", "Ontario" } },
                        { "AmericaCentrale", new List<string> { "StatiUnitiOrientali", "Ontario" } },
                        { "Ontario", new List<string> { "StatiUnitiOrientali", "AmericaCentrale", "TerritoriDelNordOvest", "Groenlandia", "Quebec" } },
                        { "TerritoriDelNordOvest", new List<string> { "Ontario", "Alaska", "Groenlandia" } },
                        { "Groenlandia", new List<string> { "Ontario", "TerritoriDelNordOvest", "Quebec" } },
                        { "Quebec", new List<string> { "Ontario", "Groenlandia", "StatiUnitiOrientali" } },
                        { "Venezuela", new List<string> { "Perù", "Brasile", "Argentina" } },
                        { "Perù", new List<string> { "Venezuela", "Brasile", "Argentina" } },
                        { "Brasile", new List<string> { "Venezuela", "Perù", "Argentina", "Congo", "AfricaDelSud" } },
                        { "Argentina", new List<string> { "Venezuela", "Perù", "Brasile", "Congo" } },
                        { "Congo", new List<string> { "Brasile", "AfricaDelSud", "AfricaOrientale", "AfricaDelNord" } },
                        { "AfricaDelSud", new List<string> { "Congo", "Brasile", "Madagascar" } },
                        { "Madagascar", new List<string> { "AfricaDelSud", "AfricaOrientale" } },
                        { "Kamchatka", new List<string> { "Jacuzia", "Giappone", "Cina" } },
                        { "Jacuzia", new List<string> { "Kamchatka", "Siberia" } },
                        { "Cina", new List<string> { "Giappone", "Kamchatka", "Siam", "India", "Urali" } },
                        { "Siam", new List<string> { "Cina", "India", "MedioOriente" } },
                        { "India", new List<string> { "Siam", "Cina", "MedioOriente", "Afghanistan" } },
                        { "MedioOriente", new List<string> { "India", "Cina", "Siam", "Afghanistan", "Urali", "AfricaOrientale" } },
                        { "Afghanistan", new List<string> { "India", "MedioOriente", "Urali", "Siberia" } },
                        { "Urali", new List<string> { "Cina", "MedioOriente", "Afghanistan", "Siberia", "Mongolia" } },
                        { "Siberia", new List<string> { "Jacuzia", "Afghanistan", "Urali", "Mongolia" } },
                        { "Mongolia", new List<string> { "Urali", "Siberia", "Cina" } },
                        { "GranBretagna", new List<string> { "EuropaOccidentale", "EuropaMeridionale", "EuropaSettentrionale" } },
                        { "EuropaSettentrionale", new List<string> { "GranBretagna", "EuropaOccidentale", "Islanda", "Scandinavia" } },
                        { "Islanda", new List<string> { "GranBretagna", "EuropaSettentrionale" } }
                    },
                    Tanks = new Dictionary<string, int>
                    {
                        { "Alaska", 3 },
                        { "Alberta", 2 },
                        { "StatiUnitiOccidentali", 4 },
                        { "StatiUnitiOrientali", 5 },
                        { "AmericaCentrale", 2 },
                        { "Ontario", 7 },
                        { "TerritoriDelNordOvest", 3 },
                        { "Groenlandia", 5 },
                        { "Quebec", 4 },
                        { "Venezuela", 3 },
                        { "Perù", 2 },
                        { "Brasile", 4 },
                        { "Argentina", 3 },
                        { "Congo", 6 },
                        { "AfricaDelSud", 3 },
                        { "Madagascar", 2 },
                        { "Kamchatka", 4 },
                        { "Jacuzia", 3 },
                        { "Cina", 7 },
                        { "Siam", 3 },
                        { "India", 6 },
                        { "MedioOriente", 4 },
                        { "Afghanistan", 3 },
                        { "Urali", 6 },
                        { "Siberia", 3 },
                        { "Mongolia", 4 },
                        { "GranBretagna", 2 },
                        { "EuropaSettentrionale", 5 },
                        { "Islanda", 3 }
                    }
                }
            };



            // Converte l'oggetto playersMaps (una lista di oggetti MapData) in una stringa JSON formattata
            // WriteIndented = true opzione che formatta il JSON con spazi e indentazione per renderlo più leggibile
            string jsonData = JsonSerializer.Serialize(winnerMap, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("JSON inviato a C++:\n" + jsonData);

            // reinforcement restituisce un puntatore (char*) ma è un puntatore a memoria non gestita (cioè non gestita dal GC di .NET).
            // per questo usiamo IntPtr che rappresenta una struttura C# che viene utilizzata per memorizzare degli indirizzi di memoria (per la memoria non gestita).
            // su IntPtr possono essere utilizzati i metodi della classe Marshal di C#.

            bool isWin = winCheck(jsonData); 

            Console.WriteLine("Il giocatore ha vinto? -> {0}", isWin);

            
            jsonData = JsonSerializer.Serialize(loserMap, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("JSON inviato a C++:\n" + jsonData);

            // reinforcement restituisce un puntatore (char*) ma è un puntatore a memoria non gestita (cioè non gestita dal GC di .NET).
            // per questo usiamo IntPtr che rappresenta una struttura C# che viene utilizzata per memorizzare degli indirizzi di memoria (per la memoria non gestita).
            // su IntPtr possono essere utilizzati i metodi della classe Marshal di C#.

            isWin = winCheck(jsonData); 

            Console.WriteLine("Il giocatore ha vinto? -> {0}", isWin);
         


        }



    } //fine classe
}