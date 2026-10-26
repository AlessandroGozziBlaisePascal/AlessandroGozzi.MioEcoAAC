using AlessandroGozzi.AppAAC.MobileGUI.Pages;

namespace AlessandroGozzi.AppAAC.MobileGUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ActivityPage), typeof(ActivityPage));
        }
    }
}
