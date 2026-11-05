using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;


namespace UniWar {
    /// <summary>
    /// Configures fonts, logging, and the initial Windows window geometry.
    /// </summary>
    public static class MauiProgram {        
        /// <summary>
        /// Creates and configures the MAUI application.
        /// </summary>
        /// <returns>Configured <see cref="MauiApp"/> instance.</returns>
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
							var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
                            int width = (int)(displayInfo.Width * 0.49);  // Fraction of screen width
                            int height = (int)(displayInfo.Height * 0.94);  // Fraction of screen height
                            
                            appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
                            appWindow.Move(new Windows.Graphics.PointInt32(50, 10));
						}
					});
				});
			});

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            try {
                return builder.Build();
            }
            catch (System.Exception ex) {
                Console.WriteLine($"Errore: {ex.Message}");
                Console.WriteLine($"Dettagli: {ex.InnerException?.Message}");
                throw;
            }
            
        }
    }
}
