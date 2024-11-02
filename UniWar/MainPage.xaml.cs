using System.Runtime.InteropServices;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json; // IMPORTANTE



namespace UniWar
{

    [DataContract]
    public class ResponseModel {
        [DataMember(Name = "result")]
        public int Result { get; set; }
    }



    public partial class MainPage : ContentPage {
        private readonly HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        // Importa la funzione C++ direttamente qui
        [DllImport("UniWar\\UniWarCppLibrary\\x64\\Debug\\UniWarCppLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Multiply(int a, int b);

        private void OnCppButtonClicked(object sender, EventArgs e)
        {
            // Legge i valori inseriti nei campi di input
            if (int.TryParse(FirstNumberEntry.Text, out int num1) &&
                int.TryParse(SecondNumberEntry.Text, out int num2)) {
                // Richiama la funzione C++ per il calcolo
                int result = Multiply(num1, num2);

                // Mostra il risultato nella Label
                ResultLabel.Text = $"Risultato dalla libreria C++: {result}";
            } else {
                ResultLabel.Text = "Inserisci numeri validi.";
            }
        }

        private async void OnPythonButtonClicked(object sender, EventArgs e) {
            // Legge i valori inseriti nei campi di input
            if (int.TryParse(FirstNumberEntry.Text, out int num1) &&
                int.TryParse(SecondNumberEntry.Text, out int num2)) {

                // richiesta HTTP al server python
                string url = $"https://diverse-cobra-nicoville-52f48d81.koyeb.app/add/?a={num1}&b={num2}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                // 'content' è il json RAW del tipo {"risultato": valore}

                try {
                    var resultObject = DeserializeJson<ResponseModel>(content);
                    if (resultObject != null) {
                        ResultLabel2.Text = $"Result: {resultObject.Result}"; // Stampa il risultato
                    } else {
                        ResultLabel2.Text = "Error: Failed to get result";
                    }
                } catch (Exception ex) {
                    ResultLabel2.Text = $"Error: {ex.Message}";
                }
                
            } else {
                ResultLabel2.Text = "Inserisci numeri validi.";
            }
        }


        // Metodo helper per deserializzare
        private static T? DeserializeJson<T>(string json)  where T : class {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            var serializer = new DataContractJsonSerializer(typeof(T)) ?? throw new InvalidOperationException("Serializer could not be created.");
            return serializer.ReadObject(stream) as T;
        }

    }
}
