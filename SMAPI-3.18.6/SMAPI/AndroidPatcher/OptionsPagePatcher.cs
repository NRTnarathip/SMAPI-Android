using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class OptionsPagePatcher
{
    internal static void Init()
    {
        var optionsPageType = typeof(OptionsPage);

        // Get the constructor method with the specified parameter types
        ConstructorInfo constructor = optionsPageType.GetConstructor(new[] {
            typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float)
        });
        var harmony = new Harmony(nameof(OptionsPagePatcher));
        var postfixMethod = typeof(OptionsPagePatcher).GetMethod(nameof(PostfixCtor), BindingFlags.Static | BindingFlags.NonPublic);
        harmony.Patch(constructor, postfix: new(postfixMethod));
    }
    static void PostfixCtor(int x, int y, int width, int height,
            float widthMod, float heightMod, OptionsPage __instance)
    {
        var page = __instance;
        var optionsPageType = typeof(OptionsPage);

        FieldInfo optionsField = optionsPageType.GetField("options", BindingFlags.NonPublic | BindingFlags.Instance);
        var options = (List<OptionsElement>)optionsField.GetValue(page);

        var buttonRequestFarmMigrate = new OptionsButton("Request Farm Migrate", OnClickRequestFarmMigrate);

        var items = new List<OptionsElement>();
        items.Add(buttonRequestFarmMigrate);
        items.Add(new OptionsButton("Delete & Backup Mods", OnClickDeleteAndBackupMods));

        options.InsertRange(3, items);
    }

    static void OnClickDeleteAndBackupMods()
    {
        //move folder mods to documents
        var modsDir = Constants.ModsPath;
        var downloadDir = Android.OS.Environment.GetExternalStoragePublicDirectory("") + "/Download/Mods-Backup";
        FileTool.CopyFolder(modsDir, downloadDir);
        Directory.Delete(modsDir, true);
    }
    private static void OnClickRequestFarmMigrate()
    {
        AndroidLog.Log("On click request farm migrate");
        MainActivity.instance.CheckStorageMigration();
    }
}
