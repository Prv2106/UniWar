namespace UniWar
{
    /// <summary>
    /// Shell container that hosts the application pages.
    /// </summary>
    public partial class AppShell : Shell
    {
        /// <summary>
        /// Initializes the shell and registers application routes.
        /// </summary>
        public AppShell()
        {
            InitializeComponent();
            // Register the sign-in route.
            Routing.RegisterRoute("signin", typeof(SignInUp));
        }
    }
}
