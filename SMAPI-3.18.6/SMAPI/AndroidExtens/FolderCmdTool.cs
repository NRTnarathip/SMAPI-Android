using System.IO;

namespace StardewModdingAPI.AndroidExtens
{
    public static class FolderCmdTool
    {
        public static bool CheckFolderCmd(string cmd)
        {
            return Directory.Exists(Path.Combine(FolderPicker.SMAPI_GamePath, "CMD_" + cmd));
        }
        //please call FolderPicker allow read & write permissions
        public static void DeleteFolderCmd(string cmd)
        {
            var path = Path.Combine(FolderPicker.SMAPI_GamePath, "CMD_" + cmd);

            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
        }
    }
}