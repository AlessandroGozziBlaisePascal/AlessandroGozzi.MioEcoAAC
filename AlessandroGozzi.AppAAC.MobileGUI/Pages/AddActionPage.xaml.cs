using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI.Pages;

public partial class AddActionPage : ContentPage
{
	public AddActionPage(AddActionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}