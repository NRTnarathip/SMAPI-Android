using Android.App;
using Android.OS;
using Android.Support.V4.Provider;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
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
    public static void CopyFile(DocumentFile source, string path)
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
    public static void CreateFileFromFilePath(this DocumentFile folderDocFile, string fileName, string mimeType, string srcPath, string destFolderPath)
    {
        if (folderDocFile.IsFile) return;

        var newOutputDocFile = folderDocFile.CreateFile(mimeType, fileName);

        var destFilePath = destFolderPath.combine(newOutputDocFile.Name);
        using var sourceStream = System.IO.File.OpenRead(srcPath);
        ParcelFileDescriptor fileDesc = MainActivity.instance.ContentResolver.OpenFileDescriptor(newOutputDocFile.Uri, "w");
        using var outputStream = new FileOutputStream(fileDesc.FileDescriptor);

        const int bufferOneKBSize = 1024;
        const int bufferSize = bufferOneKBSize * 128;
        byte[] buffer = new byte[bufferSize];
        int len;
        while ((len = sourceStream.Read(buffer)) > 0)
        {
            outputStream.Write(buffer, 0, len);
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

    public static void CopyDirectory(this DocumentFile docFile, string folderPath)
    {
        if (docFile.IsDirectory)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        var files = docFile.ListFiles();
        foreach (var file in files)
        {
            var destPath = folderPath.combine(file.Name);
            if (file.IsDirectory)
            {
                file.CopyDirectory(destPath);
                continue;
            }
            CopyFile(file, destPath);
        }
    }


    //zip tool
    public static void CreateZipFile(string zipPath, string folderPath)
    {
        using (FileStream fsOut = System.IO.File.Create(zipPath))
        {
            using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
            {
                // Set the compression level (0-9, 9 being the highest compression)
                zipStream.SetLevel(9);
                FileTool.AddDirectoryToZip(folderPath, zipStream, folderPath.GetFolderName());
            }
        }
    }
    public static void AddDirectoryToZip(string sourceDirectory, ZipOutputStream zipStream, string parentFolderName)
    {
        string[] files = Directory.GetFiles(sourceDirectory);

        foreach (string file in files)
        {
            // Create a ZipEntry object representing the file
            ZipEntry entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(file)));

            // Set the file time and size
            entry.DateTime = DateTime.Now;
            FileInfo fi = new FileInfo(file);
            entry.Size = fi.Length;

            // Add the file entry to the zip stream
            zipStream.PutNextEntry(entry);

            // Copy the file contents to the zip stream
            using (FileStream fs = System.IO.File.OpenRead(file))
            {
                byte[] buffer = new byte[4096];
                StreamUtils.Copy(fs, zipStream, buffer);
            }

            zipStream.CloseEntry();
        }

        // Recursively add subdirectories
        string[] directories = Directory.GetDirectories(sourceDirectory);
        foreach (string directory in directories)
        {
            AddDirectoryToZip(directory, zipStream, Path.Combine(parentFolderName, Path.GetFileName(directory)));
        }
    }
}

