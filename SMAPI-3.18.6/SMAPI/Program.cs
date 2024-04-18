using HarmonyLib;
using StardewModdingAPI.AndroidExtens;
using StardewModdingAPI.AndroidExtensions;
using StardewModdingAPI.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StardewModdingAPI
{
    /// <summary>The main entry point for SMAPI, responsible for hooking into and launching the game.</summary>
    public class Program
    {
        /*********
        ** Fields
        *********/
        /// <summary>The absolute path to search for SMAPI's internal DLLs.</summary>
        private static readonly string DllSearchPath = EarlyConstants.InternalFilesPath;

        /// <summary>The assembly paths in the search folders indexed by assembly name.</summary>
        private static Dictionary<string, string>? AssemblyPathsByName;

        /*********
        ** Public methods
        *********/
        /// <summary>The main entry point which hooks into and launches the game.</summary>
        /// <param name="args">The command-line arguments.</param>
        public static void RunMain()
        {
            AndroidLog.Log("Start RunMain()");
            AppDomain.CurrentDomain.AssemblyResolve += Program.CurrentDomain_AssemblyResolve;

            //Init my android Extens
            OnCreatePartTwoPatcher.Init();
            StringSplitPatcher.Init();
            SaveGamePatcher.Init();
            OptionsPagePatcher.Init();
            FolderPicker.Init();
            Game1Patcher.Init();

            new Harmony(typeof(Program).FullName).PatchAll();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Method called when assembly resolution fails, which may return a manually resolved assembly.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs e)
        {

            AndroidLog.Log("Try to resolve assembly: " + e.Name + ", Request From: " + e.RequestingAssembly.FullName);

            // cache assembly paths by name
            if (Program.AssemblyPathsByName == null)
            {
                Program.AssemblyPathsByName = new(StringComparer.OrdinalIgnoreCase);

                foreach (string searchPath in new[] { EarlyConstants.GamePath, Program.DllSearchPath })
                {
                    foreach (string dllPath in Directory.EnumerateFiles(searchPath, "*.dll"))
                    {
                        try
                        {
                            string? curName = AssemblyName.GetAssemblyName(dllPath).Name;
                            if (curName != null)
                            {
                                //AndroidLog.Log("added cache dll search: " + curName + ", with patch: " + dllPath);
                                Program.AssemblyPathsByName[curName] = dllPath;
                            }
                        }
                        catch
                        {
                            // ignore invalid DLL
                        }
                    }
                }
            }

            try
            {
                //fix fix MonoCecil
                var dllName = e.Name.Split(",")[0];
                if (dllName == "Mono.Cecil")
                {
                    var cecil = Assembly.Load("Mono.Cecil");
                    AndroidLog.Log("it's already mono cecil: " + cecil);
                    AndroidLog.Log("mono cecil path: " + cecil.Location);
                    return cecil;
                }

                string? searchName = new AssemblyName(e.Name).Name;
                var resultLoad = searchName != null && Program.AssemblyPathsByName.TryGetValue(searchName, out string? assemblyPath)
                    ? Assembly.LoadFrom(assemblyPath)
                    : null;
                if (resultLoad != null)
                {
                    AndroidLog.Log("Done resulve: " + resultLoad);
                }
                return resultLoad;
            }
            catch (Exception ex)
            {

                Android.Util.Log.Debug("NRT DEbug", "Error resolving assembly:: " + ex);
                return null;
            }
        }

        //For Android SMAPI Launcher : Protect Call Dispose();
        static SCore core;
        internal static void Start()
        {
            string modsPath = Path.Combine(Constants.GamePath, "Mods");
            AndroidLog.Log("Mods path: " + modsPath);
            core = new(modsPath, false, false);
            core.RunInteractively();
        }
    }
}
