using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI.Pages;

public partial class AddRoutinePage : ContentPage
{
    public AddRoutinePage(AddRoutineViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}