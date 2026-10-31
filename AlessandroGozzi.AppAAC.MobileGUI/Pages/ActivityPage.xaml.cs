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
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (ActivityViewModel.NeedsRefresh && BindingContext is ActivityViewModel vm && Guid.TryParse(vm.RoutineId, out var routineGuid))
        {
            ActivityViewModel.NeedsRefresh = false;
            _ = vm.LoadActivitiesAsync(routineGuid);
        }
    }
}
