using AlessandroGozzi.AppAAC.MobileGUI.Pages;
using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;

namespace AlessandroGozzi.AppAAC.MobileGUI;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    private CancellationTokenSource? _holdCts;
    private const int HoldDurationMs = 3000;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HomeViewModel vm)
        {
            await vm.LoadRoutinesAsync();
        }
    }
    private async void OnHoldButtonPressed(object sender, EventArgs e)
    {
        _holdCts = new CancellationTokenSource();
        var token = _holdCts.Token;

        if (BindingContext is HomeViewModel vm)
        {
            vm.HoldButtonText = "Resta premuto...";
            var startTime = DateTime.UtcNow;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    vm.HoldProgress = Math.Clamp(elapsed / HoldDurationMs, 0.0, 1.0);

                    if (elapsed >= HoldDurationMs)
                    {
                        // Sblocco riuscito: chiude la modale e naviga alla SettingsPage
                        vm.IsParentalGateOpen = false;
                        vm.HoldProgress = 0;
                        vm.HoldButtonText = "Tieni premuto per 3s";

                        await Shell.Current.GoToAsync(nameof(SettingsPage));
                        break;
                    }

                    await Task.Delay(30, token);
                }
            }
            catch (TaskCanceledException)
            {
                // Pressione rilasciata prima dei 3 secondi
            }
        }
    }

    private void OnHoldButtonReleased(object sender, EventArgs e)
    {
        _holdCts?.Cancel();

        if (BindingContext is HomeViewModel vm && vm.HoldProgress < 1.0)
        {
            vm.HoldProgress = 0;
            vm.HoldButtonText = "Tieni premuto per 3s";
        }
    }
}