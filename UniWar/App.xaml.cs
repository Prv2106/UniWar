namespace UniWar
{
    public partial class App : Application {
        public App() {
            InitializeComponent();
            MainPage = new AppShell();    
        }

        protected override void OnSleep() {
            // L’app è passata in background
            Console.WriteLine("App in background");
        }
    }
}
