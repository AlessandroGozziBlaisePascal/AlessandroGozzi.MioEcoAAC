using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI.Pages;

public partial class CongratulationsPage : ContentPage
{
    private readonly CongratulationsViewModel _viewModel;

    public CongratulationsPage(CongratulationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.PronunciaMessaggioAsync();
    }
}