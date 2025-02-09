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
							var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
                            int width = (int)(displayInfo.Width * 0.49);  // % della larghezza dello schermo
                            int height = (int)(displayInfo.Height * 0.94);  // % dell'altezza dello schermo
                            
                            appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
                            appWindow.Move(new Windows.Graphics.PointInt32(5, 10));

                            // Per monitorare il focus
                            w.Activated += (sender, args) => {
                                Console.WriteLine("Focus attivo!");
                                if (args.WindowActivationState == Microsoft.UI.Xaml.WindowActivationState.Deactivated) {
                                    Console.WriteLine("Ho perso il focus!");
                                    w.Activate();
                                }
                            };
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
