using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace StardewModdingAPI.AndroidExtensions
{

    [HarmonyPatch(typeof(String))]
    public class StringSplitPatcher
    {
        static void Log(object msg) => Android.Util.Log.Debug("NRT DEbug", msg.ToString());
        static void Log(string msg) => Android.Util.Log.Debug("NRT DEbug", msg);

        //call this for init
        public static void Init()
        {
            Log("On init String Split Patcher..");
            var harmony = new Harmony(nameof(StringSplitPatcher));
            var ThisType = typeof(StringSplitPatcher);
            var StringType = typeof(String);

            {
                MethodInfo[] splitMethods = StringType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(m => m.Name == "Split").ToArray();
                foreach (var methodInfo in splitMethods)
                {
                    var paramTypes = methodInfo.GetParameters().Select(param => param.ParameterType).ToArray();
                    if (!paramTypes.Contains(typeof(StringSplitOptions)))
                        continue;
                    for (int i = 0; i < paramTypes.Length; i++)
                    {
                        var param = paramTypes[i];
                        if (param == typeof(StringSplitOptions))
                        {
                            paramTypes[i] = param.MakeByRefType();
                            break;
                        }
                    }
                    var prefixStringSplit = ThisType.GetMethod(nameof(Split),
                        BindingFlags.Static | BindingFlags.NonPublic, null, paramTypes, null);
                    harmony.Patch(methodInfo, new HarmonyMethod(prefixStringSplit));
                }
            }

            //Patch Split Internal
            {
                MethodInfo[] splitInternalMethods = StringType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(m => m.Name == nameof(SplitInternal)).ToArray();
                foreach (var methodInfo in splitInternalMethods)
                {
                    //add type string[] __result;
                    var paramTypes = methodInfo.GetParameters().Select(param => param.ParameterType).ToList();
                    paramTypes.Insert(0, typeof(string[]).MakeByRefType());
                    var postfix = ThisType.GetMethod(nameof(SplitInternal),
                        BindingFlags.Static | BindingFlags.NonPublic, null,
                        paramTypes.ToArray(), null);
                    harmony.Patch(methodInfo, postfix: new HarmonyMethod(postfix));
                }
            }

            //Test case
            //var items = "Android, Window,   ios  ";
            //var fixs = items.Split(',', int.MaxValue, System.StringSplitOptions.RemoveEmptyEntries + 1);
            //foreach (var fix in fixs)
            //    Log($"res fix: [{fix}]");

            Log("done init string split patcher");
        }

        static int LastFlag = 0;
        static bool LastFlagIsNeedTrimEntries = false;


        static void SplitInternal(ref string[] __result, ReadOnlySpan<char> separators, int count, StringSplitOptions options)
        {
            if (__result.Length > 0)
                SplitInternalTrimEntries(ref __result);
        }
        static void SplitInternal(ref string[] __result, string separator, string[] separators, int count, StringSplitOptions options)
        {
            if (__result.Length > 0)
                SplitInternalTrimEntries(ref __result);
        }
        static void SplitInternal(ref string[] __result, string separator, int count, StringSplitOptions options)
        {
            if (__result.Length > 0)
                SplitInternalTrimEntries(ref __result);
        }
        //static int lastTick = 0;
        //static int runSplitInternalTrimEntriesCount = 0;
        //static Stopwatch SplitInternalTrimEntriesTimer = new();
        static void SplitInternalTrimEntries(ref string[] result)
        {
            //SplitInternalTrimEntriesTimer.Start();

            if (LastFlagIsNeedTrimEntries)
                result = result.Select(result => result.Trim()).ToArray();
            LastFlagIsNeedTrimEntries = false;
            LastFlag = 0;
            //runSplitInternalTrimEntriesCount++;

            //SplitInternalTrimEntriesTimer.Stop();
            //if (lastTick != Game1.ticks)
            //{
            //    AndroidLog.Log("runSplitInternalTrimEntriesCount: " + runSplitInternalTrimEntriesCount);
            //    AndroidLog.Log("total time: " + SplitInternalTrimEntriesTimer.Elapsed.TotalMilliseconds + "ms");

            //    lastTick = Game1.ticks;
            //    runSplitInternalTrimEntriesCount = 0;
            //    SplitInternalTrimEntriesTimer.Restart();
            //}

        }


        static void Split(char separator, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void Split(char separator, int count, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void Split(char[] separator, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void Split(char[] separator, int count, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void Split(string separator, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void Split(string separator, Int32 count, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void Split(string[] separator, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void Split(string[] separator, Int32 count, ref StringSplitOptions options)
        {
            PrefixStringSplitOptions(ref options);
        }
        static void PrefixStringSplitOptions(ref StringSplitOptions options)
        {
            LastFlag = (int)options;
            //check flag have TrimEntries
            if (LastFlag >= 2)
            {
                LastFlagIsNeedTrimEntries = true;
                //fix flag for mscorelib 
                if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                    options = StringSplitOptions.RemoveEmptyEntries;
                else
                    options = StringSplitOptions.None;
            }
        }

    }
}