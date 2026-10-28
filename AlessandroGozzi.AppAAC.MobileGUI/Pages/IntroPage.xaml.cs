using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.MobileGUI.Services;
using Plugin.Maui.Audio;

namespace AlessandroGozzi.AppAAC.MobileGUI;

public partial class IntroPage : ContentPage
{
    private readonly IRoutineRepository _routineRepository;
    private readonly IAudioManager _audioManager;
    private readonly ITextToSpeechService _ttsService;
    private bool _isSparkling = true;

    public IntroPage(
        IRoutineRepository routineRepository,
        IAudioManager audioManager,
        ITextToSpeechService ttsService)
    {
        InitializeComponent();
        _routineRepository = routineRepository;
        _audioManager = audioManager;
        _ttsService = ttsService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(200);

        // 1. Avvia l'animazione di scintillio delle stelline laterali
        _ = SparkleLoopAsync(StarLeft1, 1.3, 750);
        _ = SparkleLoopAsync(StarLeft2, 1.4, 900);
        _ = SparkleLoopAsync(StarLeft3, 1.2, 800);
        _ = SparkleLoopAsync(StarRight1, 1.3, 850);
        _ = SparkleLoopAsync(StarRight2, 1.4, 700);
        _ = SparkleLoopAsync(StarRight3, 1.2, 950);

        // 2. Animazione combinata di comparsa centrale (Logo + Nome)
        if (LogoContainer != null)
        {
            _ = LogoContainer.FadeTo(1, 700, Easing.CubicIn);
            _ = LogoContainer.ScaleTo(1.0, 700, Easing.CubicOut);
        }

        // 3. Riproduzione del suono dell'arpa
        try
        {
            var audioStream = await FileSystem.OpenAppPackageFileAsync("intro_sound.mp3");
            var player = _audioManager.CreatePlayer(audioStream);
            player.Play();

            await Task.Delay(1200);
        }
        catch
        {
            // Fallback: se il file sonoro manca, prosegue senza bloccare l'app
        }

        // 4. Sintesi vocale in italiano: "MioEco"
        await _ttsService.SpeakItalianAsync("MioEco");

        // Breve pausa dopo il parlato per stabilità visiva
        await Task.Delay(400);

        // Ferma le animazioni
        _isSparkling = false;

        // 5. Passaggio all'interfaccia principale Shell
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        });
    }

    private async Task SparkleLoopAsync(View star, double targetScale, uint duration)
    {
        while (_isSparkling)
        {
            await Task.WhenAll(
                star.FadeTo(0.9, duration, Easing.SinInOut),
                star.ScaleTo(targetScale, duration, Easing.SinInOut),
                star.RelRotateTo(30, duration, Easing.SinInOut)
            );

            if (!_isSparkling) break;

            await Task.WhenAll(
                star.FadeTo(0.2, duration, Easing.SinInOut),
                star.ScaleTo(0.6, duration, Easing.SinInOut),
                star.RelRotateTo(30, duration, Easing.SinInOut)
            );
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isSparkling = false;
    }
}