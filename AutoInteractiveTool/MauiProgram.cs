using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace AutoInteractiveTool
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
                    fonts.AddFont("Segoe UI.ttf", "SegoeUI");
                });

            // Register services using the DI extensions
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<IExcelProcessor, ExcelProcessor>();
            builder.Services.AddSingleton<IHttpService, HttpService>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
