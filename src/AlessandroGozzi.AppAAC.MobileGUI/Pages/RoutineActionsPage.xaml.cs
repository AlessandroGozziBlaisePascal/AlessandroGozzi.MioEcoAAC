using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI.Pages;

public partial class RoutineActionsPage : ContentPage
{
    public RoutineActionsPage(ActionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ActionsViewModel vm && Guid.TryParse(vm.ActivityId, out var activityGuid))
        {
            ActionsViewModel.NeedsRefresh = false;
            _ = vm.CaricaAzioniAsync(activityGuid);
        }
    }
}