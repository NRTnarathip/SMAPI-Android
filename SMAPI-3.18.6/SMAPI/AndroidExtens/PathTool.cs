using System.IO;
using System.Linq;

namespace StardewModdingAPI.AndroidExtens
{
    public static class PathTool
    {
        public static string GetFolderName(this string path)
        {
            return path.Split("/").Last();
        }
        public static string combine(this string path1, string path2) => Path.Combine(path1, path2);
        public static string combine(this string path1, string path2, string path3) => Path.Combine(path1, path2, path3);
        public static string combine(this string path1, string path2, string path3, string path4) => Path.Combine(path1, path2, path3, path4);
    }
}