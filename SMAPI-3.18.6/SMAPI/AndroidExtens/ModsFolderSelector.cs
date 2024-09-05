using System.IO;

namespace StardewModdingAPI.AndroidExtens;

internal static class ModsFolderSelector
{
    public static string GetModsFolderFullPath()
    {
        //default at files/Saves/SMAPI-Game/Mods
        string path = Constants.DefaultModsPath;

        //options at files/Mods
        if (!Directory.Exists(path))
        {
            path = Path.Combine(Constants.ExternalFilesDir, "Mods");
        }

        return path;
    }
}