using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI.Pages;

public partial class AddActivityPage : ContentPage
{
    public AddActivityPage(AddActivityViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}