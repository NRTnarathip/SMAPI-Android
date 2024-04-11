using StardewModdingAPI.Framework;
using System;
using System.IO;

namespace StardewModdingAPI.AndroidExtens
{
    public static class FolderCmdTool
    {
        public static string GetFolderCmdPath(string cmd) => Path.Combine(FolderPicker.ExternalSMAPIDir, cmd);
        public static bool CheckFolderCmd(string cmd)
        {
            return Directory.Exists(GetFolderCmdPath(cmd));
        }
        public static bool CheckFolderCmds(string[] cmds)
        {
            foreach (var cmd in cmds)
            {
                if (Directory.Exists(GetFolderCmdPath(cmd)))
                {
                    return true;
                }
            }
            return false;
        }
        //please call FolderPicker allow read & write permissions
        public static void DeleteFolderCmd(string cmd)
        {
            var path = GetFolderCmdPath(cmd);
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path);
                if (Directory.Exists(path))
                    Console.WriteLine("Erorr can't delete cmd path: " + cmd);
                else
                    Console.WriteLine("Done delete cmd path: " + cmd);
            }
            catch (Exception ex)
            {
                SCore.Instance.GetMonitorForGame().Log("Error can't delete cmd folder path: " + path + " error: " + ex.Message, LogLevel.Error);
            }
        }

        public static void Clean()
        {
            foreach (var dir in Directory.GetDirectories(FolderPicker.ExternalSMAPIDir))
            {
                var folderName = dir.GetFolderName();
                Console.WriteLine("found dir: " + folderName);
                if (folderName.StartsWith(SMAPIUpdateTool.CMDPrefix))
                {
                    DeleteFolderCmd(folderName);
                }
            }
        }
    }
}