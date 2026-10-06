using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;
using Microsoft.Maui;
using AndroidX.Fragment.App;
using AColor = Android.Graphics.Color;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace WkcCommunicator
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            // The splash theme only covers the launch screen; the app itself runs on the Material 3
            // theme, which is what makes the Shell tab bar the native Material 3 bottom navigation
            // and gives the Material widgets the Material 3 color roles they ask for.
            SetTheme(Resource.Style.AppTheme_NoActionBar);
            base.OnCreate(savedInstanceState);

            // Every dialog and popup gets its own window, and the status bar appearance is taken
            // from whichever window is on top. Their themes default to dark status bar icons, and a
            // floating popup does not dim the status bar the way a full screen dialog does, so both
            // the icon set and the darkened tone are applied here as soon as such a window exists.
            SupportFragmentManager.RegisterFragmentLifecycleCallbacks(new DialogStatusBarCallbacks(this), false);
        }

        /// <summary>
        /// Re-applies the status bar color of the running window. Called on every resume because
        /// popups and dialogs make Android re-resolve the window decorations.
        /// </summary>
        internal void RefreshStatusBar()
        {
            if (Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault() is MainWindow window)
                window.ApplyStatusBarColor();
        }

        /// <summary>
        /// The status bar color the app uses on its own window: the surface tone in the dark theme,
        /// the (darker) primary tone in the light theme.
        /// </summary>
        internal AColor StatusBarBaseColor()
        {
            // var night = (Resources?.Configuration?.UiMode & Android.Content.Res.UiMode.NightMask)
            //     == Android.Content.Res.UiMode.NightYes;
            var colorResource = Resource.Color.colorSurface;
            return new AColor(AndroidX.Core.Content.ContextCompat.GetColor(this, colorResource));
        }

        sealed class DialogStatusBarCallbacks : FragmentManager.FragmentLifecycleCallbacks
        {
            readonly MainActivity _activity;

            public DialogStatusBarCallbacks(MainActivity activity) => _activity = activity;

            public override void OnFragmentStarted(FragmentManager fm, Fragment f) => Apply(f);

            public override void OnFragmentResumed(FragmentManager fm, Fragment f) => Apply(f);

            public override void OnFragmentViewCreated(FragmentManager fm, Fragment f, Android.Views.View v, Bundle? savedInstanceState) => Apply(f);

            void Apply(Fragment fragment)
            {
                if (fragment is not DialogFragment dialogFragment || dialogFragment.Dialog?.Window is not { } window)
                    return;

                // The app's own status bar follows the theme (dark icons on the light surface),
                // but every dialog darkens the status bar behind it, so they all switch to the
                // light icon set. Without this a floating popup keeps the dark icons of the
                // activity theme and they disappear into the darkened bar.
                WindowCompat.GetInsetsController(window, window.DecorView).AppearanceLightStatusBars = false;

                // A full screen dialog already dims the status bar through its own dim layer, but a
                // floating popup's dim stops below it. Painting the dimmed tone here gives both the
                // same darkened status bar.
                var baseColor = _activity.StatusBarBaseColor();
                var factor = 1f - (window.Attributes?.DimAmount ?? 0f);
                if (factor > 0.95f || factor <= 0f)
                    factor = 0.7f;

                window.SetStatusBarColor(AColor.Argb(
                    255,
                    (int)(baseColor.R * factor),
                    (int)(baseColor.G * factor),
                    (int)(baseColor.B * factor)));
            }
        }
    }
}
