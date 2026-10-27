using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI.Pages;

public partial class RoutineActionsPage : ContentPage
{
    public RoutineActionsPage(ActionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}