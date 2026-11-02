using AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels;

namespace AlessandroGozzi.MioEco_Material.MobileGUI.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    private CancellationTokenSource? _holdCts;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCardsCommand.ExecuteAsync(null);
    }

    private void OnSettingsGearClicked(object sender, EventArgs e)
    {
        HoldProgressBar.Progress = 0;
        CaregiverOverlay.IsVisible = true;
    }

    private void OnClosePopupClicked(object sender, EventArgs e)
    {
        CancelHold();
        ResetHoldUI(null);
        CaregiverOverlay.IsVisible = false;
    }

    private async void OnCaregiverHoldPressed(object sender, EventArgs e)
    {
        var button = sender as Button;

        CancelHold();
        var cts = new CancellationTokenSource();
        _holdCts = cts;

        HoldProgressBar.Progress = 0;

        try
        {
            if (button != null)
                await button.ScaleToAsync(0.94, 100);

            const int totalDurationMs = 3000;
            const int intervalMs = 30;
            int elapsedMs = 0;

            while (elapsedMs < totalDurationMs)
            {
                await Task.Delay(intervalMs, cts.Token);
                elapsedMs += intervalMs;

                double currentProgress = Math.Min(1.0, (double)elapsedMs / totalDurationMs);
                HoldProgressBar.Progress = currentProgress;
            }

            if (button != null)
                await button.ScaleToAsync(1.0, 100);

            CaregiverOverlay.IsVisible = false;
            HoldProgressBar.Progress = 0;

            try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); } catch { }

            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }
        catch (OperationCanceledException)
        {
            ResetHoldUI(button);
        }
        catch (ObjectDisposedException)
        {
            ResetHoldUI(button);
        }
    }

    private void OnCaregiverHoldReleased(object sender, EventArgs e)
    {
        CancelHold();
        ResetHoldUI(sender as Button);
    }

    private void CancelHold()
    {
        if (_holdCts != null)
        {
            try
            {
                _holdCts.Cancel();
                _holdCts.Dispose();
            }
            catch (ObjectDisposedException) { }
            _holdCts = null;
        }
    }

    private void ResetHoldUI(Button? button)
    {
        HoldProgressBar.Progress = 0;
        if (button != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await button.ScaleToAsync(1.0, 100);
            });
        }
    }
}