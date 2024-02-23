using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace OEMInfo_MAUI_Sample
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

            MainPage.ModifyEntry();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }




    }
}
