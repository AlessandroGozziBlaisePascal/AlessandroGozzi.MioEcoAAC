using AlessandroGozzi.AppAAC.Application.Interfaces;
using Plugin.Maui.Audio;

namespace AlessandroGozzi.AppAAC.MobileGUI;

public partial class IntroPage : ContentPage
{
    private readonly IDatabaseInitializer _dbInitializer;
    private readonly IAudioManager _audioManager;

    public IntroPage(IDatabaseInitializer dbInitializer, IAudioManager audioManager)
    {
        InitializeComponent();
        _dbInitializer = dbInitializer;
        _audioManager = audioManager;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(250);

        if (LogoContainer != null)
        {
            _ = LogoContainer.FadeTo(1, 800, Easing.CubicIn);
            _ = LogoContainer.ScaleTo(1.0, 800, Easing.CubicOut);
        }

        _ = Task.Run(async () =>
        {
            try
            {
                var audioStream = await FileSystem.OpenAppPackageFileAsync("intro_sound.mp3");
                var player = _audioManager.CreatePlayer(audioStream);
                player.Play();
            }
            catch { }
        });

        await _dbInitializer.InitializeAndSeedAsync();

        await Task.Delay(2000);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Window != null)
            {
                Window.Page = new AppShell();
            }
        });
    }
}