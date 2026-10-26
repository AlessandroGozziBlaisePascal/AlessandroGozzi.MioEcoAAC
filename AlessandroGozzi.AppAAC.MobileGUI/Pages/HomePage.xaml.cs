using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRoutinesAsync();
    }
}