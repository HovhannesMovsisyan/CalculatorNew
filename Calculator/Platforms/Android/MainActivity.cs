using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using AndroidX.Core.View;

namespace Calculator
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            UpdateSystemBars();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            UpdateSystemBars();
        }

        // Match the status and navigation bars to the page background (same colors as MainPage.xaml)
        void UpdateSystemBars()
        {
            if (Window is null)
                return;

            var isDark = (Resources?.Configuration?.UiMode & UiMode.NightMask) != UiMode.NightNo;
            var color = Android.Graphics.Color.ParseColor(isDark ? "#0B1220" : "#F0FDF4");

#pragma warning disable CA1422 // Still honored below Android 15, which is what the emulator targets
            Window.SetStatusBarColor(color);
            Window.SetNavigationBarColor(color);
#pragma warning restore CA1422

            if (WindowCompat.GetInsetsController(Window, Window.DecorView) is { } insets)
            {
                insets.AppearanceLightStatusBars = !isDark;
                insets.AppearanceLightNavigationBars = !isDark;
            }
        }
    }
}
