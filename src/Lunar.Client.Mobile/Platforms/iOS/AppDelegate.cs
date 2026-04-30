using Foundation;
using UIKit;

namespace Lunar.Client.Mobile.Platforms.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private static MobileClient _game;

        public override void FinishedLaunching(UIApplication app)
        {
            _game = new MobileClient();
            _game.Run();
        }
    }

    public static class Application
    {
        [global::ObjCRuntime.BindingImpl(global::ObjCRuntime.BindingImplOptions.Optimizable)]
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(AppDelegate).Name);
        }
    }
}
