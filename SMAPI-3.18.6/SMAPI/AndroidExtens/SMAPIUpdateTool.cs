using Android.Support.V4.Provider;
using StardewModdingAPI.AndroidExtensions;
using StardewValley;
using System.IO;
using System.Linq;
using System.Reflection;
using Uri = Android.Net.Uri;

namespace StardewModdingAPI.AndroidExtens
{
    public static class SMAPIUpdateTool
    {
        //call this when on first enter TitleMenu
        public static void CheckAllUpdate()
        {
            //detect command with Folder Name
            //select event only 1
            if (FolderCmdTool.CheckFolderCmd("UpdateSMAPI"))
            {
                AlertUpdateSMAPI();
            }
            else if (FolderCmdTool.CheckFolderCmd("UpdateMods"))
            {
                AlertUpdateMods();
            }
            else if (FolderCmdTool.CheckFolderCmd("BackupSaves"))
            {
                AlertBackupSaves();
            }
        }

        private static void AlertBackupSaves()
        {

        }

        public static void AlertUpdateSMAPI()
        {
            NotifyTool.Confirm("Check Update SMAPI", "choose folder SMAPI-Game for check & update", async (confirm) =>
            {
                if (!confirm) return;

                AndroidLog.Log("Start folder picker");
                var pick = await FolderPicker.Pick();
                UpdateSMAPI(pick);
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
            NotifyTool.ConfirmOnly("Found New Mods", "choose folder SMAPI-Game for update mods", async () =>
            {
                var folder = await FolderPicker.Pick();
                SyncMods(folder);
            });
        }
        static void SyncMods(Uri uri)
        {
            var doc = uri.ToDocumentFile();
        }
    }
}