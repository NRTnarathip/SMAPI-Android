using AssemblyStoreTool;
using System.IO.Compression;

internal class Program
{
    static string ReverseBackFolders(string path, int levels)
    {
        // Ensure the path is not null or empty
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        // Get the root path (drive letter or root network path)
        string rootPath = Path.GetPathRoot(path);

        // Traverse up the directory tree by the specified number of levels
        for (int i = 0; i < levels; i++)
        {
            // Get the parent directory of the current path
            path = Path.GetDirectoryName(path);

            // If the path is null, break out of the loop
            if (path == null)
            {
                break;
            }
        }

        // Combine the root path with the reversed path to get the final result
        string reversedPath = Path.Combine(rootPath, path ?? string.Empty);

        return reversedPath;
    }
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        //argument

        var projectDir = ReverseBackFolders(Directory.GetCurrentDirectory(), 3);
        var solutionDir = ReverseBackFolders(projectDir, 1);
        var smapiProjectDir = solutionDir.combine("SMAPI-3.18.6", "SMAPI");

        var smapiLoaderProjectDir = solutionDir.combine("SMAPILoader");
        var smapiLoaderDllPath = smapiLoaderProjectDir.combine("bin", "Debug", "SMAPILoader.dll");

        var smapiDebugBuildDir = smapiProjectDir.combine("bin", "Debug");
        var smapiSystemReplacementZip = projectDir.combine("SMAPI-System-Replacement.zip");
        var apkPatcherProjectDir = solutionDir.combine("SMAPI APK Patcher");
        var baseApk = apkPatcherProjectDir.combine("base.apk");


        Console.WriteLine("Base Project: " + projectDir);
        Console.WriteLine("Base Solution Dir: " + solutionDir);
        Console.WriteLine("SMAPI Project Dir: " + smapiProjectDir);
        Console.WriteLine("SMAPI Debug Bin Dir: " + smapiDebugBuildDir);
        Console.WriteLine("SMAPILoader.dll Path: " + smapiLoaderDllPath);

        var extractDir = projectDir.combine("TempExtract");
        if (Directory.Exists(extractDir))
            Directory.Delete(extractDir, true);
        var tempExtractDir = Directory.CreateDirectory(extractDir);

        var blobPath = extractDir.combine("assemblies.blob");
        var manifestPath = extractDir.combine("assemblies.manifest");
        using (var zip = ZipFile.OpenRead(baseApk))
        {
            zip.GetEntry("assemblies/assemblies.blob").ExtractToFile(blobPath);
            zip.GetEntry("assemblies/assemblies.manifest").ExtractToFile(manifestPath);
        }

        var store = new AssemblyStore(blobPath);

        var gameModuleMap = new HashSet<string>(store.blob.Assemblies.Select(asm => asm.DllName));


        //Build Final
        var SMAPIModulesOutput = projectDir.combine("SMAPI-Modules");
        if (Directory.Exists(SMAPIModulesOutput))
            Directory.Delete(SMAPIModulesOutput, true);
        Directory.CreateDirectory(SMAPIModulesOutput);

        //add dependencies
        var needModuleMap = new HashSet<string>()
        {
            "System.Runtime.Serialization.dll",
            "mscorlib.dll",
        };
        foreach (var smapiModulePath in Directory.GetFiles(smapiDebugBuildDir))
        {
            var fileInfo = new FileInfo(smapiModulePath);
            var fileName = fileInfo.Name;

            if (fileInfo.Extension != ".dll") continue;

            if (!needModuleMap.Contains(fileName) && gameModuleMap.Contains(fileName))
                continue;

            var dest = SMAPIModulesOutput.combine(fileName);
            File.Copy(smapiModulePath, dest);
            Console.WriteLine("Added dll: " + dest);
        }

        //add smapi loader
        File.Copy(smapiLoaderDllPath, SMAPIModulesOutput.combine("SMAPILoader.dll"));
        Console.WriteLine("Added SMAPILoader.dll");

        //final replacement system dll
        using (var zipSysReplacement = ZipFile.OpenRead(smapiSystemReplacementZip))
        {
            foreach (var entry in zipSysReplacement.Entries)
            {
                if (entry.Name.Contains(".dll") == false) continue;
                var dest = SMAPIModulesOutput.combine(entry.Name);
                using (var fileStream = File.Open(dest, FileMode.Create))
                {
                    using (var entryStream = entry.Open())
                    {
                        entryStream.CopyTo(fileStream);
                        Console.WriteLine("Added module: " + dest);
                    }
                }
            }
        }

        using (var fileStream = File.Open(SMAPIModulesOutput + ".zip", FileMode.Create))
        {
            ZipFile.CreateFromDirectory(SMAPIModulesOutput, fileStream);
            Console.WriteLine("Done zip file");
        }


        //cleanup
        Directory.Delete(SMAPIModulesOutput, true);
        tempExtractDir.Delete(true);
    }
}