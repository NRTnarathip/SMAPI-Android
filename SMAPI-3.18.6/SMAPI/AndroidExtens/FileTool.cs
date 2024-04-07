using Android.App;
using Android.Support.V4.Provider;
using Java.IO;
using StardewModdingAPI.AndroidExtens;
using StardewValley;
using System;
using System.IO;
using System.Security.Cryptography;
namespace StardewModdingAPI.AndroidExtensions;

public static class FileTool
{
    public static string DownloadDir = Android.OS.Environment.GetExternalStoragePublicDirectory("") + "/Download";
    public static void SharpCopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
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
                SharpCopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
    public static void FileCopy(DocumentFile source, string path)
    {
        if (source.IsFile == false) return;

        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        //delete first
        using Stream stream = MainActivity.instance.ContentResolver.OpenInputStream(source.Uri);
        using FileOutputStream fileOutputStream = new FileOutputStream(path);
        const int bufferOneKBSize = 1024;
        const int bufferSize = bufferOneKBSize * 128;
        byte[] buffer = new byte[bufferSize];
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
    public static DocumentFile ToDocument(this Android.Net.Uri uri)
    {
        return DocumentFile.FromTreeUri(Application.Context, uri);
    }
    public static byte[] ToBytes(this DocumentFile file)
    {
        using Stream stream = MainActivity.instance.ContentResolver.OpenInputStream(file.Uri);
        using MemoryStream mem = new MemoryStream();
        stream.CopyTo(mem);
        return mem.ToArray();
    }
    public static string ReadFile(this DocumentFile file)
    {
        using var stream = MainActivity.instance.ContentResolver.OpenInputStream(file.Uri);
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public static void CopyTo(this DocumentFile docFile, string path)
    {
        if (docFile.IsDirectory)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        var files = docFile.ListFiles();
        foreach (var file in files)
        {
            var destPath = path.combine(file.Name);
            if (file.IsDirectory)
            {
                file.CopyTo(destPath);
                continue;
            }
            FileCopy(file, destPath);
        }
    }
}

