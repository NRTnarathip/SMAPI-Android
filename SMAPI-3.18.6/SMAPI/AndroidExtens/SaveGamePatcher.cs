using StardewModdingAPI.AndroidExtens;
using System;
using System.IO;
using System.Linq;

namespace StardewModdingAPI.AndroidExtensions;

public class SaveGamePatcher
{
    public static void Init()
    {
    }
    public static event Action OnBackupSaveDone;
    public static event Action OnBackupSaveFail;
    static void BackupSavesToDownloadInternal()
    {
        AndroidLog.Log("On Backup Saves..");
        var outputPath = Path.Combine(FileTool.DownloadDir, "Saves-Backup");
        var savesPath = Constants.SavesPath;
        foreach (var srcFolderPath in Directory.EnumerateDirectories(savesPath, "*.*", SearchOption.TopDirectoryOnly))
        {
            var folderName = srcFolderPath.Split("/").Last();
            if (folderName == "SMAPI-Game") continue;

            AndroidLog.Log("found save: " + srcFolderPath);
            var destFolderPath = Path.Combine(outputPath, folderName);
            AndroidLog.Log($"try clone {srcFolderPath} to {destFolderPath}");
            FileTool.CopyDirectory(srcFolderPath, destFolderPath);
        }
        AndroidLog.Log("Done Backup Saves..");
    }
    public static int FolderPickerBackuSaves = "FolderPickerBackupSaves".GetHashCode();
    public static void RequestFolder(string path)
    {
    }
    public static void BackupSavesToDownload()
    {
        NotifyTool.Confirm("Saves Backup", "Are you sure to backup saves in folder download", (confirm) =>
        {
            if (!confirm)
            {
                OnBackupSaveFail?.Invoke();
                return;
            }
            //select folder


            BackupSavesToDownloadInternal();
            OnBackupSaveDone?.Invoke();

            //notify save
            NotifyTool.Notify("Successfully Backup Saves", "done saves backup in folder download");
        });
    }
}