using AlessandroGozzi.MioEco_Material.MobileGUI.Pages;

namespace AlessandroGozzi.MioEco_Material.MobileGUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(AddFlashCardPage), typeof(AddFlashCardPage));
            Routing.RegisterRoute(nameof(ChangeImagePage), typeof(ChangeImagePage));
        }
    }
}
