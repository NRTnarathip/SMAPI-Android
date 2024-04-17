using Android.Views;
using HarmonyLib;
using StardewValley;
using System.Reflection;

namespace StardewModdingAPI.AndroidExtensions
{
    internal class OnCreatePartTwoPatcher
    {
        //call this at first line of MainActivity.OnCreate()
        public static void Init()
        {
            var harmony = new Harmony("OnCreatePartTwo");
            var method = typeof(MainActivity).GetMethod("OnCreatePartTwo", BindingFlags.Instance | BindingFlags.NonPublic);
            var prefix = typeof(OnCreatePartTwoPatcher).GetMethod(nameof(PrefixOnCreatePartTwo),
                BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(method, prefix: new HarmonyMethod(prefix));
        }

        static bool PrefixOnCreatePartTwo()
        {

            Program.Start();

            var assembly = typeof(MainActivity).Assembly;
            var MainActivityType = typeof(MainActivity);
            var MobileDisplayType = assembly.GetType("StardewValley.Mobile.MobileDisplay");
            var SetupDisplaySettingsMethod = MobileDisplayType.GetMethod("SetupDisplaySettings");
            var DoLicenseCheckMethod = MainActivityType.GetMethod("DoLicenseCheck", BindingFlags.Instance | BindingFlags.NonPublic);


            SetupDisplaySettingsMethod.Invoke(null, null);
            MainActivity.instance.SetPaddingForMenus();
            MainActivity.instance.SetContentView((View)GameRunner.instance.Services.GetService(typeof(View)));
            GameRunner.instance.Run();
            DoLicenseCheckMethod.Invoke(MainActivity.instance, null);

            return false;
        }
    }
}