using AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels;

namespace AlessandroGozzi.MioEco_Material.MobileGUI.Pages;

public partial class ChangeImagePage : ContentPage
{
    public ChangeImagePage(ChangeImageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}