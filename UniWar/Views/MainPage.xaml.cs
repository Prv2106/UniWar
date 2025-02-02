/*
In C#, la direttiva using System.Runtime.InteropServices; serve a importare il namespace System.Runtime.InteropServices, che contiene classi e metodi per interagire con il codice nativo (ad esempio, codice C o C++). 
Questo namespace è fondamentale quando si ha bisogno di fare interoperabilità tra il codice gestito (C#) e il codice non gestito (come quello C++).
*/
using System.Runtime.InteropServices;
using System.Collections.Generic;
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

       

    
        public struct MapData{
            public string PlayerId {set; get;} 
            public Dictionary<string, List<string>> Neighbors { get; set; }
            public Dictionary<string, int> Tanks { get; set; }

        }
        
        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr process_map(string jsonData);

        private async void OnTryLibrary(object sender, EventArgs e) {

            // Sintassi con Object Initializer (viene chiamato il costruttore senza parametri in modo implicito)
            List<MapData> playersMaps = new List<MapData> {
                new MapData {
                    PlayerId = "Player1",
                    Neighbors = new Dictionary<string, List<string>> {
                        { "TerritorioA", new List<string> { "TerritorioB", "TerritorioC" } },
                        { "TerritorioB", new List<string> { "TerritorioA" } },
                        { "TerritorioC", new List<string> { "TerritorioA" } }
                    },
                    Tanks = new Dictionary<string, int> {
                        { "TerritorioA", 10 },
                        { "TerritorioB", 5 },
                        { "TerritorioC", 7 }
                    }
                },
                new MapData {
                    PlayerId = "Player2",
                    Neighbors = new Dictionary<string, List<string>> {
                        { "TerritorioX", new List<string> { "TerritorioY", "TerritorioZ" } },
                        { "TerritorioY", new List<string> { "TerritorioX" } },
                        { "TerritorioZ", new List<string> { "TerritorioX" } }
                    },
                    Tanks = new Dictionary<string, int> {
                        { "TerritorioX", 8 },
                        { "TerritorioY", 6 },
                        { "TerritorioZ", 4 }
                    }
                }
            };

            // Converte l'oggetto playersMaps (una lista di oggetti MapData) in una stringa JSON formattata
            // WriteIndented = true opzione che formatta il JSON con spazi e indentazione per renderlo più leggibile
            string jsonData = JsonSerializer.Serialize(playersMaps, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("JSON inviato a C++:\n" + jsonData);

            // process_map restituisce un puntatore (char*) ma è un puntatore a memoria non gestita (cioè non gestita dal GC di .NET).
            // per questo usiamo IntPtr che rappresenta una struttura C# che viene utilizzata per memorizzare degli indirizzi di memoria (per la memoria non gestita).
            // su IntPtr possono essere utilizzati i metodi della classe Marshal di C#.
            IntPtr resultPtr = process_map(jsonData);
            // In questo caso, usiamo Marshal.PtrToStringAnsi(resultPtr) per copiare la stringa della memoria non gestita (C++) in una stringa gestita dal GC di C#
            string resultJson = Marshal.PtrToStringAnsi(resultPtr);
            
            // Nota: non ci occupiamo di deallocare la memoria non gestita perché nella funzione C++ usiamo una stringa statica 
            // che quindi viene allocata nella memoria statica e persiste per tutta la durata del rpogramma (non abbiamo problemi di memory leak).

            // JsonSerializer.Deserialize<List<MapData>>(resultJson) converte (deserializza) quella stringa JSON in una Lista di oggetti MapData.
            // updatedMaps diventa quindi una List<MapData>, cioè una lista di oggetti MapData.
            var updatedMaps = JsonSerializer.Deserialize<List<MapData>>(resultJson);


        

           Console.WriteLine("JSON aggiornato: " + JsonSerializer.Serialize(updatedMaps, new JsonSerializerOptions { WriteIndented = true }));
        }
    
       
        /*Codice per testare la vittoria*/
        [DllImport("cppLibrary\\functions_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void testWin();


        private async void ProvaVittoria(object sender, EventArgs e) {
            testWin();
        }



    } //fine classe
}
