using System;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UniWar
{
    public partial class MainPage : ContentPage
    {
        [DllImport("C:\\Users\\provi\\Documents\\GitHub\\UniWar\\UniWar\\CppLibrary\\UniWarCppLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetCppMessage();

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCppButtonClicked(object sender, EventArgs e)
        {
            IntPtr resultPtr = GetCppMessage();
            string result = Marshal.PtrToStringAnsi(resultPtr);
            ResultLabel.Text = "Risultato da C++: " + result;
        }

    }
}
