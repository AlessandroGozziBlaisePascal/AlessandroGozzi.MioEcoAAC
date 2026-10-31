using AlessandroGozzi.AppAAC.MobileGUI.Pages;

namespace AlessandroGozzi.AppAAC.MobileGUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ActivityPage), typeof(ActivityPage));
            Routing.RegisterRoute("ActionsPage", typeof(RoutineActionsPage));
            Routing.RegisterRoute(nameof(CongratulationsPage), typeof(CongratulationsPage));
            Routing.RegisterRoute(nameof(AddRoutinePage), typeof(AddRoutinePage));
            Routing.RegisterRoute(nameof(AddActivityPage), typeof(AddActivityPage));
            Routing.RegisterRoute(nameof(AddActionPage), typeof(AddActionPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
