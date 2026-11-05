namespace UniWar
{
    /// <summary>
    /// Application entry point. Creates the initial shell.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the application and sets the main page to the shell.
        /// </summary>
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
