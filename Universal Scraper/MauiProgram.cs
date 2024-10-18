using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Universal_Scraper.Services;
using Universal_Scraper.ViewModels;
using Universal_Scraper.Models;

namespace Universal_Scraper
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<JSInjection>();
            builder.Services.AddSingleton<Selector>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
