using System;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls;

namespace UniWar
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Importa la funzione C++ direttamente qui
        [DllImport("UniWar\\UniWarCppLibrary\\x64\\Debug\\UniWarCppLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Multiply(int a, int b);

        private void OnCppButtonClicked(object sender, EventArgs e)
        {
            // Legge i valori inseriti nei campi di input
            if (int.TryParse(FirstNumberEntry.Text, out int num1) &&
                int.TryParse(SecondNumberEntry.Text, out int num2))
            {
                // Richiama la funzione C++ per il calcolo
                int result = Multiply(num1, num2);

                // Mostra il risultato nella Label
                ResultLabel.Text = $"Risultato: {result}";
            }
            else
            {
                ResultLabel.Text = "Inserisci numeri validi.";
            }
        }
    }
}
