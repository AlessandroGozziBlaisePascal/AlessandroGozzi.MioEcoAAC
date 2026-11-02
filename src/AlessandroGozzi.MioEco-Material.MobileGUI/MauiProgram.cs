using AlessandroGozzi.MioEco_Material.Domain.Interfaces;
using AlessandroGozzi.MioEco_Material.Infrastructure;
using AlessandroGozzi.MioEco_Material.MobileGUI.Pages;
using AlessandroGozzi.MioEco_Material.MobileGUI.Services;
using AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace AlessandroGozzi.MioEco_Material.MobileGUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ==========================================
            // 1. INFRASTRUCTURE & DATABASE
            // ==========================================
            // AppDbContext come Singleton per condividere la connessione SQLite
            builder.Services.AddSingleton<AppDbContext>();

            // Repository FlashCard per le operazioni CRUD
            builder.Services.AddSingleton<IFlashCardRepository, SqliteFlashCardRepository>();

            // ==========================================
            // 2. SERVIZI NATIVI (Audio & Text-To-Speech)
            // ==========================================
            // Plugin audio per il suono della intro
            builder.Services.AddSingleton(AudioManager.Current); 
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddTransient<IArasaacService, ArasaacService>();

            // Text To Speech per la sintesi vocale delle tessere
            builder.Services.AddSingleton<ITextToSpeechService, TextToSpeechService>();

            // ==========================================
            // 3. VIEWMODELS
            // ==========================================
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<AddFlashCardViewModel>();
            builder.Services.AddTransient<ChangeImageViewModel>();

            // ==========================================
            // 4. PAGES
            // ==========================================
            builder.Services.AddTransient<IntroPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<AddFlashCardPage>();
            builder.Services.AddTransient<ChangeImagePage>();

            return builder.Build();
        }
    }
}
