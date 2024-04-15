using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewModdingAPI.AndroidExtensions
{
    [HarmonyPatch(typeof(Console))]
    public class ConsoleHook
    {
        static List<string> _lines = new List<string>();
        public const int MaxLogLine = 200;
        public static List<string> Lines
        {
            get
            {
                lock (_lines)
                {
                    return _lines.ToList();
                }
            }
        }
        public static void AddLogHistory(string text)
        {
            lock (_lines)
            {
                _lines.Add(text);
                if (_lines.Count > MaxLogLine)
                {
                    _lines.RemoveAt(0);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("WriteLine", new Type[] { typeof(string) })]
        static void WriteLine(string value)
        {
            Android.Util.Log.Debug("CS:", value);
        }

        [HarmonyPostfix]
        [HarmonyPatch("WriteLine", new Type[] { typeof(string), typeof(object) })]
        static void WriteLine(string format, object arg0)
        {
            Android.Util.Log.Debug("CS:", string.Format(format, arg0));
        }
    }
}