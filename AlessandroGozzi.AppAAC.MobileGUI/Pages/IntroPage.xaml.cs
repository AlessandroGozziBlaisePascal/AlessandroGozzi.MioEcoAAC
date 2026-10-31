using AlessandroGozzi.AppAAC.Domain.Interfaces;
using AlessandroGozzi.AppAAC.MobileGUI.Services;
using Plugin.Maui.Audio;

namespace AlessandroGozzi.AppAAC.MobileGUI;

public partial class IntroPage : ContentPage
{
    private readonly IRoutineRepository _routineRepository;
    private readonly IAudioManager _audioManager;
    private readonly ITextToSpeechService _ttsService;

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

        // 1. Animazione sincronizzata di comparsa (LogoContainer e AuthorLabel)
        var animations = new List<Task>();

        if (LogoContainer != null)
        {
            animations.Add(LogoContainer.FadeTo(1, 700, Easing.CubicIn));
            animations.Add(LogoContainer.ScaleTo(1.0, 700, Easing.CubicOut));
        }

        if (AuthorLabel != null)
        {
            animations.Add(AuthorLabel.FadeTo(1, 700, Easing.CubicIn));
            animations.Add(AuthorLabel.ScaleTo(1.0, 700, Easing.CubicOut));
        }

        if (animations.Count > 0)
        {
            await Task.WhenAll(animations);
        }

        // 2. Riproduzione del suono dell'arpa
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

        // 3. Sintesi vocale in italiano: "MioEco"
        await _ttsService.SpeakItalianAsync("MioEco");

        // Breve pausa dopo il parlato per stabilità visiva
        await Task.Delay(400);

        // 4. Passaggio all'interfaccia principale Shell
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        });
    }
}