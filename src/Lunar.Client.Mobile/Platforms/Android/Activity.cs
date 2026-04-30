using Android.App;
using Android.Content.PM;
using Android.Views;
using Microsoft.Xna.Framework;

namespace Lunar.Client.Mobile.Platforms.Android
{
    [Activity(
        Label = "Lunar",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges =
            ConfigChanges.Orientation |
            ConfigChanges.Keyboard |
            ConfigChanges.KeyboardHidden |
            ConfigChanges.ScreenSize
    )]
    public class Activity : AndroidGameActivity
    {
        protected override void OnCreate(global::Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);
            var game = new MobileClient();
            SetContentView((View)game.Services.GetService(typeof(View)));
            game.Run();
        }
    }
}
