﻿using System;
using System.IO;
using System.Reflection;

namespace SMAPILoader
{
    public class Program
    {
        public static void logToFile(String filePath, String message)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(message);
            }
        }
        public static void Log(string msg)
        {
            //Android.Util.Log.Debug("NRT Debug", "[SMAPILoader] " + msg);
            //var fullMessage = $"{msg}";
            //string logFilePath = "/storage/emulated/0/Download/log.txt";
            //logToFile(logFilePath, fullMessage);
        }
        public static string CurrentDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static void RunMain()
        {
            Log("Starting..");

            //init
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;


            Log("Try to load SMAPI Framework");
            try
            {
                //fix fix load first dependencies of Mono.Cecil
                var runtimeSerialize = Path.Combine(CurrentDir, "System.Runtime.Serialization.dll");
                Assembly.LoadFile(runtimeSerialize);
                //fix fix use LoadFrom Not LoadFile
                var smapi = Assembly.LoadFrom(Path.Combine(CurrentDir, "StardewModdingAPI.dll"));
                Log("try call RunMain: " + smapi);
                smapi.GetType("StardewModdingAPI.Program")
                    .GetMethod("RunMain", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }

            Log("Done Init SMAPILoader");

        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            //bug when we use assembly.Location;
            Log("On Loaded:  " + args.LoadedAssembly);
        }
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var reqestMsg = "";
            if (args.RequestingAssembly != null)
                reqestMsg = ", Request From=" + args.RequestingAssembly.FullName;
            Log("Try resolve asm: " + args.Name + reqestMsg);
            return null;
        }
    }
}
