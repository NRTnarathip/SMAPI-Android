using StardewValley.Menus;
namespace StardewModdingAPI.AndroidExtens;

public class OptionsPagePatcher
{
    internal static void Init()
    {
        //var optionsPageType = typeof(OptionsPage);

        //// Get the constructor method with the specified parameter types
        //ConstructorInfo constructor = optionsPageType.GetConstructor(new[] {
        //    typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float)
        //});
        //var harmony = new Harmony(nameof(OptionsPagePatcher));
        //var postfixMethod = typeof(OptionsPagePatcher).GetMethod(nameof(PostfixCtor), BindingFlags.Static | BindingFlags.NonPublic);
        //harmony.Patch(constructor, postfix: new(postfixMethod));
    }
    static void PostfixCtor(int x, int y, int width, int height,
            float widthMod, float heightMod, OptionsPage __instance)
    {
        //var page = __instance;
        //var optionsPageType = typeof(OptionsPage);

        //FieldInfo optionsField = optionsPageType.GetField("options", BindingFlags.NonPublic | BindingFlags.Instance);
        //var options = (List<OptionsElement>)optionsField.GetValue(page);

        //var items = new List<OptionsElement>();
        ////items.Add(new OptionsButton("Saves Backup To Download", OnClickBackupSaves));
        //options.InsertRange(3, items);//insert memnu after button "Saves Backup" original game
    }
}
