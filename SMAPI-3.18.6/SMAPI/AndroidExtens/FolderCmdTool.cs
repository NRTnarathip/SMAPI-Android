using System;
using System.IO;

namespace StardewModdingAPI.AndroidExtens
{
    public static class FolderCmdTool
    {
        public static string GetFolderCmdPath(string cmd) => Path.Combine(FolderPicker.ExternalSMAPIDir, "CMD_" + cmd);
        public static bool CheckFolderCmd(string cmd)
        {
            return Directory.Exists(GetFolderCmdPath(cmd));
        }
        //please call FolderPicker allow read & write permissions
        public static void DeleteFolderCmd(string cmd)
        {
            var path = GetFolderCmdPath(cmd);
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
            if (Directory.Exists(path))
            {
                Console.WriteLine("Erorr can't delete cmd path: " + cmd);
            }
            else
            {
                Console.WriteLine("Done delete cmd path: " + cmd);
            }
        }
    }
}