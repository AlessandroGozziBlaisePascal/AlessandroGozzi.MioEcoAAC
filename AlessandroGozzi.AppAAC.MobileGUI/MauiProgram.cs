using AlessandroGozzi.AppAAC.Domain.Interfaces;
using AlessandroGozzi.AppAAC.Infrastructure;
using AlessandroGozzi.AppAAC.Infrastructure.Service;
using AlessandroGozzi.AppAAC.MobileGUI.Pages;
using AlessandroGozzi.AppAAC.MobileGUI.Services;
using AlessandroGozzi.AppAAC.MobileGUI.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace AlessandroGozzi.AppAAC.MobileGUI
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

            // 1.Percorso locale del DB su cartella isolata dell'app
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseConstants.DatabaseFilename);

            // 2. Registrazione DB Context, Repository e Servizi Esterni
            builder.Services.AddSingleton(sp => new AppDbContext(dbPath));
            builder.Services.AddSingleton<IRoutineRepository>(sp => new RoutineRepository(dbPath));
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<ITextToSpeechService, TextToSpeechService>();
            builder.Services.AddHttpClient<IArasaacService, ArasaacService>();

            // 3. ViewModels
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<ActivityViewModel>();
            builder.Services.AddTransient<ActionsViewModel>();
            builder.Services.AddTransient<CongratulationsViewModel>();
            builder.Services.AddTransient<AddRoutineViewModel>();
            builder.Services.AddTransient<AddActivityViewModel>();
            builder.Services.AddTransient<AddActionViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // 4. Pagine
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ActivityPage>();
            builder.Services.AddTransient<IntroPage>();
            builder.Services.AddTransient<RoutineActionsPage>();
            builder.Services.AddTransient<CongratulationsPage>();
            builder.Services.AddTransient<AddRoutinePage>();
            builder.Services.AddTransient<AddActivityPage>();
            builder.Services.AddTransient<AddActionPage>();
            builder.Services.AddTransient<SettingsPage>();

            var app = builder.Build();

            // 5. Inizializzazione e Seeding del DB tramite AppDbContext prima dell'avvio della UI
            Task.Run(async () =>
            {
                var dbContext = app.Services.GetRequiredService<AppDbContext>();
                await dbContext.InitializeAsync();
            }).GetAwaiter().GetResult();

            return app;
        }
    }
}
