using AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels;

namespace AlessandroGozzi.MioEco_Material.MobileGUI.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}