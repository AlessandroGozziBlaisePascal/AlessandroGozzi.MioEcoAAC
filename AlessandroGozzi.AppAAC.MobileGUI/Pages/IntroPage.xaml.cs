using AlessandroGozzi.AppAAC.Domain.Repositories;
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

        // 1. Animazione combinata di comparsa (Logo + Nome)
        if (LogoContainer != null)
        {
            _ = LogoContainer.FadeTo(1, 700, Easing.CubicIn);
            _ = LogoContainer.ScaleTo(1.0, 700, Easing.CubicOut);
        }

        // 2. Riproduzione del suono dell'arpa
        try
        {
            var audioStream = await FileSystem.OpenAppPackageFileAsync("intro_sound.mp3");
            var player = _audioManager.CreatePlayer(audioStream);
            player.Play();

            // Pausa per lasciare risuonare l'arpa prima di avviare la voce
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