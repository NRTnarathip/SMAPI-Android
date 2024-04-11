using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Framework;
using StardewValley;
using System.Diagnostics;

namespace StardewModdingAPI.AndroidExtens
{
    [HarmonyPatch]
    public class GamePerformance
    {
        static Stopwatch tickTimer = new Stopwatch();
        static Stopwatch doUpdateTimer = new Stopwatch();
        static Stopwatch doDrawTimer = new Stopwatch();
        static IMonitor Monitor => SCore.Instance.GetMonitorForGame();

        static int fpsCounter = 0;
        static int currentFPS = 0;
        static double fpsCounterTimerMillisecond = 0f;

        const bool IsDebug = false;

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Game), "DoUpdate", [typeof(GameTime)])]
        //static void PrefixDoUpdate()
        //{
        //    doUpdateTimer.Restart();
        //}
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Game), "DoUpdate", [typeof(GameTime)])]
        //static void EndDoUpdate()
        //{
        //    doUpdateTimer.Stop();
        //    if (Monitor != null && Game1.ticks % 60 == 0)
        //    {
        //        //Monitor.Log("Update() time: " + doUpdateTimer.Elapsed.TotalMilliseconds + "ms", LogLevel.Info);
        //    }
        //}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Game), "DoDraw", [typeof(GameTime)])]
        static void PrefixDoDraw()
        {
            doDrawTimer.Restart();
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game), "DoDraw", [typeof(GameTime)])]
        static void EndDoDraw()
        {
            doDrawTimer.Stop();
            if (Monitor != null && Game1.ticks % 60 == 0)
            {
                if (IsDebug)
                    Monitor.Log("Draw() time: " + doDrawTimer.Elapsed.TotalMilliseconds + "ms", LogLevel.Info);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Game), "Tick")]
        static void PrefixTick()
        {
            tickTimer.Restart();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game), "Tick")]
        static void PostfixTick()
        {
            tickTimer.Stop();

            //draw FPS
            fpsCounter++;
            fpsCounterTimerMillisecond += tickTimer.Elapsed.TotalMilliseconds;
            if (fpsCounterTimerMillisecond > 1000)
            {
                fpsCounterTimerMillisecond = 0;
                currentFPS = fpsCounter;
                fpsCounter = 0;
            }
            if (IsDebug && Monitor != null && Game1.ticks % 60 == 0)
            {
                Monitor.Log("Tick() time: " + tickTimer.Elapsed.TotalMilliseconds + "ms", LogLevel.Info);
                var drawAndUpdateTime = doUpdateTimer.Elapsed.TotalMilliseconds;
                drawAndUpdateTime += doDrawTimer.Elapsed.TotalMilliseconds;
                Monitor.Log("DoUpdate() + DoDraw() time: " + drawAndUpdateTime + "ms", LogLevel.Info);
                Monitor.Log("FPS: " + currentFPS, LogLevel.Debug);
            }
        }
    }
}