using Android.App;
using Android.Content;
using HarmonyLib;
using StardewValley;
using System;

namespace StardewModdingAPI.AndroidExtens;

[HarmonyPatch(typeof(MainActivity))]
public class MainActivityPatcher
{
    public static event Action<int, Result, Intent> OnActivityResult;
    [HarmonyPostfix]
    [HarmonyPatch("OnActivityResult")]
    static void PostfixOnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        AndroidLog.Log($"On Postfix activity result {requestCode} {resultCode} {data.Data}");
        OnActivityResult(requestCode, resultCode, data);
    }
}