using AlessandroGozzi.AppAAC.Application.Queries.GetRoutine;
using AlessandroGozzi.AppAAC.Infrastructure;
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddInfrastructure();

            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddTransient<IntroPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(typeof(AlessandroGozzi.AppAAC.Application.Queries.GetAllRoutines.GetAllRoutinesQuery).Assembly));

            var app = builder.Build();

            return app;
        }
    }
}
