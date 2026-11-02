using AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels;

namespace AlessandroGozzi.MioEco_Material.MobileGUI.Pages;

public partial class AddFlashCardPage : ContentPage
{
    public AddFlashCardPage(AddFlashCardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}