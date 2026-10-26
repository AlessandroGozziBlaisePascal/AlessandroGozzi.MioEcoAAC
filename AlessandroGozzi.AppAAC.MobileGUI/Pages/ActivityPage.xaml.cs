using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI.Pages;

public partial class ActivityPage : ContentPage
{
    private readonly ActivityViewModel _viewModel;

    public ActivityPage(ActivityViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
