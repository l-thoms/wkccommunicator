using AndroidX.Core.Graphics;
using AndroidX.Core.View;
using Microsoft.Maui.Platform;
using AColor = Android.Graphics.Color;
using MauiContext = Microsoft.Maui.IMauiContext;

namespace WkcCommunicator;

public partial class MainWindow
{
	/// <summary>
	/// Applies the status bar appearance of the current theme.
	///
	/// Light theme: the status bar keeps its own accent background (the theme's primary dark tone,
	/// as before this restyle) and only its content color is forced to the light set, so the clock
	/// and the status icons read white instead of black.
	/// Dark theme: the status bar is tinted with the surface color, so it blends with the app bar
	/// and the page background, again with the light icon set.
	///
	/// The night mode is read from the Activity configuration and the colors from the Android theme
	/// resources, so both follow the resource qualifiers Android has already resolved. The color is
	/// painted from the decor view background as well as through the window API, because Android
	/// re-resolves the window decorations whenever a popup or a dialog is shown (the icon set is
	/// also pinned in the theme, see Platforms/Android/Resources/values*/styles.xml, and re-applied
	/// on every resume through MainActivity.RefreshStatusBar).
	/// </summary>
	internal void ApplyStatusBarColor()
	{
#if ANDROID
		if (Handler?.MauiContext is not MauiContext mauiContext)
			return;

		var activity = mauiContext.Context.GetActivity();
		if (activity?.Window is not { } window)
			return;

		var night = IsNightMode(activity);

		var color = night
			? ResolveThemeColor(activity, Resource.Color.colorSurface, "#17120F")
			: ResolveThemeColor(activity, Resource.Color.colorSurface, "#FFF8F5");

		window.SetStatusBarColor(color);
		window.DecorView?.SetBackgroundColor(color);

		// Light icons and clock on the status bar in both themes.
		// WindowCompat.GetInsetsController(window, window.DecorView).AppearanceLightStatusBars = false;
#endif
	}

#if ANDROID
	static bool IsNightMode(Android.App.Activity activity) =>
		(activity.Resources?.Configuration?.UiMode & Android.Content.Res.UiMode.NightMask)
			== Android.Content.Res.UiMode.NightYes;

	static AColor ResolveThemeColor(Android.App.Activity activity, int colorResource, string fallback)
	{
		try
		{
			return new AColor(AndroidX.Core.Content.ContextCompat.GetColor(activity, colorResource));
		}
		catch
		{
			return AColor.ParseColor(fallback);
		}
	}
#endif
}
