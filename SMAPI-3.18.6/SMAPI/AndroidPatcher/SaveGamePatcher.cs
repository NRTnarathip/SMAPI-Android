using HarmonyLib;
using StardewValley;
using System.IO;
using System.Reflection;

namespace StardewModdingAPI.AndroidExtensions
{
    internal class SaveGamePatcher
    {
        public static void Init()
        {
            var harmony = new Harmony(nameof(SaveGamePatcher));
            var method = typeof(Game1).GetMethod("saveWholeBackup", BindingFlags.Static | BindingFlags.Public);
            var postfix = typeof(SaveGamePatcher).GetMethod(nameof(PostfixSaveWholeBackup),
                BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        }

        private static void PostfixSaveWholeBackup()
        {
            var filesDir = MainActivity.instance.ApplicationContext.GetExternalFilesDir("");
            var savesDir = filesDir + "/Saves";
            var externalDir = Android.OS.Environment.GetExternalStoragePublicDirectory("").AbsolutePath;
            var downloadDir = externalDir + "/Download";
            var backupDir = downloadDir;
            AndroidLog.Log("On Backup Saves..");
            foreach (var saveDir in Directory.GetDirectories(savesDir))
            {
                if (!saveDir.Contains("SMAPI-Game"))
                {
                    var saveFolderName = new DirectoryInfo(saveDir).Name;
                    FileTool.CopyFolder(saveDir, Path.Combine(backupDir, saveFolderName));
                }
            }
            AndroidLog.Log("Done Backup Saves..");
        }
    }
}