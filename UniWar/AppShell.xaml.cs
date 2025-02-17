namespace UniWar
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Eventuale registrazione di rotte aggiuntive
            Routing.RegisterRoute("signin", typeof(SignInUp));
        }
    }
}
