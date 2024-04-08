﻿using Android.Support.V4.Provider;
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
        public static void CheckAllUpdate()
        {
            //detect command with Folder Name
            //select event only 1
            if (FolderCmdTool.CheckFolderCmd("UpdateSMAPI"))
            {
                AlertUpdateSMAPI();
            }
            else if (FolderCmdTool.CheckFolderCmd("BackupSaves"))
            {
                AlertBackupSaves();
            }
            //else if (FolderCmdTool.CheckFolderCmd("SyncSave"))
            //{
            //    AlertSyncSave();
            //}
            else if (FolderCmdTool.CheckFolderCmd("UpdateMods"))
            {
                AlertUpdateMods();
            }
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

            NotifyTool.ConfirmOnly("Backup Saves", "Are you sure to backup saves in folder Download", () =>
            {
                SaveGamePatcher.BackupSavesToDownload();
                FolderCmdTool.DeleteFolderCmd("UpdateMods");
                MainActivity.instance.Finish();
            });
        }

        public static void AlertUpdateSMAPI()
        {
            NotifyTool.Confirm("Check Update SMAPI", "choose folder SMAPI-Game for check & update", async (confirm) =>
            {
                if (!confirm) return;

                AndroidLog.Log("Start folder picker");
                var pick = await FolderPicker.Pick();
                UpdateSMAPI(pick);
                FolderCmdTool.DeleteFolderCmd("UpdateSMAPI");
            });
        }
        static void UpdateSMAPI(Uri uri)
        {
            var docFile = DocumentFile.FromTreeUri(MainActivity.instance.ApplicationContext, uri);
            var files = docFile.ListFiles();
            //delete folder name cmd

            //replace all .dll
            var gamePath = Constants.GamePath;
            var smapiFile = files.SingleOrDefault(file => file.Name.Contains("StardewModdingAPI.dll"));
            if (smapiFile != null)
            {
                var dest = Path.Combine(gamePath, smapiFile.Name);
                //unuse
                //use Date time + AssemblyVersion instead 
                //if (!FileTool.IsSame(smapiFile, dest))
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
                    var currentBuild = Constants.ApiVersion;
                    AndroidLog.Log("Current Version: " + currentBuild);
                    AndroidLog.Log("Load File Version: " + targetFileVersion);
                    if (targetFileVersion.IsNewerThan(currentBuild))
                    {
                        //please check file date
                        AndroidLog.Log("Found New Version: " + targetFileVersion);
                        FileTool.FileCopy(smapiFile, dest);
                    }
                    else
                    {
                        AndroidLog.Log("No need to update");
                    }
                }
            }
            //bug touch screen game got crash
            //important please exit app
            MainActivity.instance.Finish();
        }
        public static void AlertUpdateMods()
        {
            NotifyTool.ConfirmOnly("Check & Update Mods", "choose folder SMAPI-Game for update mods", async () =>
            {
                var folder = await FolderPicker.Pick();
                UpdateMods(folder);
                //delete cmd
                FolderCmdTool.DeleteFolderCmd("UpdateMods");
                MainActivity.instance.Finish();
            });
        }
        static void UpdateMods(Uri uri)
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
            string externalModsPath = FolderPicker.ExternalMods;
            foreach (var modDocFile in externalMods)
            {
                //no need to check folder mod it's correct
                //detect mod new version
                var modFiles = modDocFile.ListFiles();
                var manifestDocFile = modFiles.SingleOrDefault(f => f.Name == "manifest.json");
                if (manifestDocFile == null)
                    continue;

                var manifestContent = manifestDocFile.ReadFile();
                var manifest = JsonConvert.DeserializeObject<ModManifest>(manifestContent);

                var modPathInGameFiles = Constants.ModsPath.combine(modDocFile.Name);
                //check if exists mod
                if (Directory.Exists(modPathInGameFiles))
                {
                    //check mod version
                    var manifestGameFilesPath = modPathInGameFiles.combine("manifest.json");
                    if (!File.Exists(manifestGameFilesPath)) continue;

                    var manifestGameFiles = JsonConvert.DeserializeObject<ModManifest>(File.ReadAllText(manifestGameFilesPath));
                    AndroidLog.Log("mod external version: " + manifest.Version);
                    AndroidLog.Log("mod in game version: " + manifestGameFiles.Version);
                    //sync with any version
                    if (manifest.Version == manifestGameFiles.Version)
                        continue;
                }
                var st = Stopwatch.StartNew();
                //sync new mod with any version
                if (Directory.Exists(modPathInGameFiles))
                    Directory.Delete(modPathInGameFiles, true);
                modDocFile.CopyTo(modPathInGameFiles);
                st.Stop();
                AndroidLog.Log("done copy mod to: " + modDocFile.Name + ", total time " + st.Elapsed.TotalMilliseconds + "ms");
            }
        }

        public static void Log(string msg) => SCore.Instance.GetMonitorForGame().Log(msg, LogLevel.Debug);
    }
}