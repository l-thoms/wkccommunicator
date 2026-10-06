using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
#if ANDROID
using Microsoft.Maui.LifecycleEvents;
#endif

namespace WkcCommunicator
{
    public static class MauiProgram
    {
		public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit();

#if DEBUG
			builder.Logging.AddDebug();
#endif

#if ANDROID
			// Showing a popup or a dialog makes Android re-resolve the window decorations, which
			// resets the explicitly set status bar color. Re-apply it whenever we come back.
			builder.ConfigureLifecycleEvents(lifecycle =>
			{
				lifecycle.AddAndroid(android => android
					.OnResume(activity => (activity as MainActivity)?.RefreshStatusBar())
					.OnPostResume(activity => (activity as MainActivity)?.RefreshStatusBar()));
			});
#endif

            return builder.Build();
        }
    }
}
