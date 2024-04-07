using Android.OS;
using StardewModdingAPI.AndroidExtens;
using System.IO;

namespace StardewModdingAPI.AndroidExtensions;

public class SaveGamePatcher
{
    public static void Init()
    {
    }
    public static bool CanBackupSaves()
    {
        //android 13,14
        return Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu;
    }
    public static void BackupSaves()
    {
        if (!CanBackupSaves())
            return;
        var saves = Directory.GetDirectories(Constants.SavesPath);
        AndroidLog.Log("try backup saves");
        foreach (var currentSavePath in saves)
        {
            var saveName = currentSavePath.GetFolderName();
            if (saveName == "SMAPI-Game") continue;

            AndroidLog.Log("Found save path: " + currentSavePath);
            var fileName = saveName + ".zip";
            var filePath = Constants.SavesPath.combine(fileName);
            FileTool.CreateZipFile(filePath, currentSavePath);
            var destPath = FolderPicker.DownloadDir.combine(fileName);
            File.Copy(filePath, destPath, true);
            AndroidLog.Log("Done save path: " + destPath);
        }
    }
}