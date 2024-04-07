using Android.App;
using Android.Support.V4.Provider;
using Java.IO;
using StardewValley;
using System;
using System.IO;
using System.Security.Cryptography;
namespace StardewModdingAPI.AndroidExtensions;

public static class FileTool
{
    public static string DownloadDir = Android.OS.Environment.GetExternalStoragePublicDirectory("") + "/Download";
    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
    public static void FileCopy(DocumentFile source, string dest)
    {
        using Stream stream = MainActivity.instance.ContentResolver.OpenInputStream(source.Uri);
        using FileOutputStream fileOutputStream = new FileOutputStream(dest);
        byte[] buffer = new byte[4096];
        int len;
        while ((len = stream.Read(buffer)) > 0)
        {
            fileOutputStream.Write(buffer, 0, len);
        }
    }
    public static bool IsSame(DocumentFile file1, string file2)
    {
        return GetFileHash(file1) == GetFileHash(file2);
    }
    public static string GetFileHash(DocumentFile file)
    {
        using var md5 = MD5.Create();
        using Stream stream = MainActivity.instance.ContentResolver.OpenInputStream(file.Uri);
        byte[] hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
    public static string GetFileHash(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
    public static DocumentFile ToDocumentFile(this Android.Net.Uri uri)
    {
        return DocumentFile.FromSingleUri(Application.Context, uri);
    }
}

