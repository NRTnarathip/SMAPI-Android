using Newtonsoft.Json;
using StardewModdingAPI.AndroidExtensions;
using StardewModdingAPI.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Uri = Android.Net.Uri;

namespace StardewModdingAPI.AndroidExtens
{
    public static class SMAPIUpdateTool
    {
        public const string CMDPrefix = "CMD_";
        public const string CMD_UpdateSMAPI = CMDPrefix + "UpdateSMAPI";
        public const string CMD_SyncMods = CMDPrefix + "SyncMods";
        public const string CMD_BackupSaves = CMDPrefix + "BackupSaves";

        public struct ModManifest
        {
            public string Name;
            public string Version;
            public string MinimumApiVersion;
            public string UniqueID;

            internal bool IsNewer(ModManifest targetManifest)
            {
                return new SemanticVersion(this.Version).IsNewerThan(new SemanticVersion(targetManifest.Version));
            }
        }
        //call this when on first enter TitleMenu
        public static void CheckAllCommand()
        {
            //detect command with Folder Name
            //select event only 1
            if (FolderCmdTool.CheckFolderCmd(CMD_UpdateSMAPI))
            {
                AlertUpdateSMAPI();
            }
            else if (FolderCmdTool.CheckFolderCmd(CMD_BackupSaves))
            {
                AlertBackupSaves();
            }
            //check CMD_UpdateMods when game it's 3.20.7
            else if (FolderCmdTool.CheckFolderCmd(CMD_SyncMods))
            {
                AlertSyncMods();
            }

            //clean all cmd folder
            FolderCmdTool.Clean();
        }
        private static void AlertSyncSave()
        {
            NotifyTool.ConfirmOnly("Sync Saves", "please choose folder for load saves", () =>
            {
                SyncSave();
            });
        }
        private static async void SyncSave()
        {
            var uri = await FolderPicker.Pick();

            var docFile = uri.ToDocument();
            var files = docFile.ListFiles();
            foreach (var file in files)
            {
                if (!file.IsFile)
                    continue;
                if (!file.Name.Contains(".zip")) continue;
                var zipFileName = file.Name;
                var splitFileName = zipFileName.Split("_");
                if (splitFileName.Length == 2)
                {
                    //look like save mod
                    if (int.TryParse(splitFileName[1], out int savehash))
                    {
                        AndroidLog.Log("found save: " + zipFileName + ", save hash: " + savehash);
                    }
                }
            }
        }
        private static void AlertBackupSaves()
        {
            if (!SaveGamePatcher.CanBackupSaves())
                return;

            NotifyTool.Confirm("Backup Saves", "Are you sure to backup saves in folder Download", (isConfirm) =>
            {
                if (isConfirm)
                {
                    SaveGamePatcher.BackupSavesToDownload();
                }
                MainActivity.instance.Finish();
            });
        }
        public static void AlertUpdateSMAPI()
        {
            NotifyTool.Confirm("Check Update SMAPI", $"choose folder SMAPI-{Constants.ApiVersion}++ for check & update", async (isConfirm) =>
            {
                if (isConfirm)
                {
                    var pick = await FolderPicker.Pick();
                    UpdateSMAPI(pick);
                }
                MainActivity.instance.Finish();
            });
        }
        static void UpdateSMAPI(Uri uri)
        {
            var folderPick = uri.ToDocument();
            if (!folderPick.Name.StartsWith("SMAPI-"))
                return;

            var filesInFolderPicker = folderPick.ListFiles();
            var gamePath = Constants.GamePath;
            //check you are picker folder smapi modules correct
            var smapiFile = filesInFolderPicker.SingleOrDefault(file => file.Name.Contains("StardewModdingAPI.dll"));
            if (smapiFile != null)
            {
                ISemanticVersion targetFileVersion;
                using (var stream = MainActivity.instance.ContentResolver.OpenInputStream(smapiFile.Uri))
                {
                    using var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    var asm = Assembly.Load(memoryStream.ToArray());
                    var fileVersion = asm.GetName().Version.ToString();
                    targetFileVersion = new Toolkit.SemanticVersion(fileVersion.Substring(0, fileVersion.Length - 2));
                }

                //check file build version
                var currentVersion = Constants.ApiVersion;
                AndroidLog.Log("Current Version: " + currentVersion);
                AndroidLog.Log("Load File Version: " + targetFileVersion);
                //update when newer or equal
                if (targetFileVersion.IsNewerThan(currentVersion) || targetFileVersion.Equals(currentVersion))
                {
                    //please check file date
                    AndroidLog.Log("Found Replace Version: " + targetFileVersion);
                    //FileTool.FileCopy(smapiFile, dest);
                    //copy all modules and rename file with postfix _New.dll
                    foreach (var file in filesInFolderPicker)
                    {
                        if (file.IsDirectory)
                            continue;
                        if (file.Name.Contains(".dll") == false)
                            continue;
                        var dest = Path.Combine(gamePath, file.Name.Replace(".dll", "_New.dll"));
                        FileTool.CopyFile(file, dest);
                        AndroidLog.Log("done copy file to: " + dest);
                    }

                }
                else
                {
                    AndroidLog.Log("No need to update");
                }
            }
            //bug touch screen game got crash
            //important please exit app
        }
        public static void AlertSyncMods()
        {
            NotifyTool.Confirm("Check & Sync Mods", "select folder SMAPI-Game for syncs mods", async (isConfirm) =>
            {
                if (isConfirm)
                {
                    var folder = await FolderPicker.Pick();
                    SyncMods(folder);
                }
                MainActivity.instance.Finish();
            });
        }
        static void SyncMods(Uri uri)
        {
            var smapiDocFile = uri.ToDocument();
            if (smapiDocFile.Name != "SMAPI-Game")
            {
                Log("invalid folder " + smapiDocFile.Name);
                return;
            }

            var smapiFiles = smapiDocFile.ListFiles();
            var modsDocFile = smapiFiles.SingleOrDefault(file => file.Name == "Mods");
            if (modsDocFile == null)
            {
                Log("not found folder mods");
                return;
            }
            var externalMods = modsDocFile.ListFiles().Where(dir => dir.IsDirectory);
            var externalModsMap = new HashSet<string>(externalMods.Select(dir => dir.Name));
            var currentMods = Directory.GetDirectories(Constants.ModsPath);
            var currentModsMap = new HashSet<string>(currentMods.Select(dir => dir.GetFolderName()));

            foreach (var modFolderName in currentModsMap)
            {
                if (!externalModsMap.Contains(modFolderName))
                {
                    Directory.Delete(Constants.ModsPath.combine(modFolderName), true);
                }
            }


            //add and update
            foreach (var modDocFile in externalMods)
            {
                //no need to check folder mod it's correct
                //detect mod new version
                var modFiles = modDocFile.ListFiles();
                var manifestDocFile = modFiles.SingleOrDefault(f => f.Name == "manifest.json");
                if (manifestDocFile == null)
                    continue;


                var modPathInGameFiles = Constants.ModsPath.combine(modDocFile.Name);
                bool needToSyncMods = true;
                //check if exists mod
                if (Directory.Exists(modPathInGameFiles))
                {
                    //check mod version in game data & external public mods dir
                    var manifestGameFilesPath = modPathInGameFiles.combine("manifest.json");
                    if (File.Exists(manifestGameFilesPath))
                    {
                        var manifestContent = manifestDocFile.ReadFile();
                        var manifest = JsonConvert.DeserializeObject<ModManifest>(manifestContent);
                        var manifestGameFiles = JsonConvert.DeserializeObject<ModManifest>(File.ReadAllText(manifestGameFilesPath));
                        AndroidLog.Log("mod external version: " + manifest.Version);
                        AndroidLog.Log("mod in game version: " + manifestGameFiles.Version);
                        //dont sync with current version
                        if (manifest.Version == manifestGameFiles.Version)
                        {
                            needToSyncMods = false;
                        }
                    }
                }

                if (needToSyncMods)
                {
                    var st = Stopwatch.StartNew();
                    //sync new mod with any version
                    //delete mod clean up first
                    if (Directory.Exists(modPathInGameFiles))
                        Directory.Delete(modPathInGameFiles, true);
                    modDocFile.CopyDirectory(modPathInGameFiles);
                    st.Stop();
                    AndroidLog.Log("done copy mod to: " + modDocFile.Name + ", total time " + st.Elapsed.TotalMilliseconds + "ms");
                }
            }
        }
        public static void Log(string msg) => SCore.Instance.GetMonitorForGame().Log(msg, LogLevel.Debug);
    }
}