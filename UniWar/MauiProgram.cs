using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;


namespace UniWar {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureLifecycleEvents(events => {
				events.AddWindows(window => {
					window.OnWindowCreated(w => {
						var appWindow = w.GetAppWindow(); 
						if (appWindow != null) {
							int width = 650;  // Larghezza finestra
							int height = 1200; // Altezza finestra
							appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));							
						}
					});
				});
			});

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
