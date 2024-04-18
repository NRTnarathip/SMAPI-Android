//Rewrite code For PC
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Reflection;
public class Game1Patcher
{
    public static MethodInfo Game1_warpFarmOriginal = null;
    public static void Init()
    {


        //{
        //    var harmony = new Harmony(typeof(Game1Patcher).FullName);
        //    var method = typeof(Game1).GetMethod(nameof(Game1.warpFarmer),
        //        [typeof(LocationRequest), typeof(int), typeof(int), typeof(int), typeof(bool)]);
        //    var methodReplacement = typeof(Game1Patcher).GetMethod(nameof(warpFarmerRetarget));
        //    Game1_warpFarmOriginal = harmony.Patch(method, prefix: new(methodReplacement));
        //}

        //try
        //{
        //    var game = Mono.Cecil.AssemblyDefinition.ReadAssembly(Constants.GamePath + "/StardewValley.dll");
        //    foreach (var t in game.MainModule.Types)
        //    {
        //        Console.WriteLine("SV: found type: " + t.FullName);
        //    }
        //    var Game1Type = game.MainModule.Types.Single(t => t.FullName == "StardewValley.Game1").Resolve();
        //    Console.WriteLine("SV: found method in Game1: " + Game1Type.Methods.Count);
        //    Game1Type.Methods.Clear();
        //    Console.WriteLine("SV: found method in Game1 after clear all: " + Game1Type.Methods.Count);
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("SV: " + ex);
        //}

        //foreach (var methodInGame1 in typeof(Game1).GetMethods())
        //{
        //    Console.WriteLine("SV: found method in Game1: " + methodInGame1);
        //}
    }
    //public static bool warpFarmerRetarget(LocationRequest locationRequest,
    //    int tileX, int tileY, int facingDirectionAfterWarp, bool doFade = true)
    //{
    //    Game1Facade._warpFarmer_doFade = doFade;
    //    Game1Facade.warpFarmer(locationRequest, tileX, tileY, facingDirectionAfterWarp);
    //    return false;
    //}
}
public class Game1Facade : Game1
{
    public new static IList<GameLocation> locations => Game1.locations;
    public new static IList<IClickableMenu> onScreenMenus => Game1.onScreenMenus;
    //public static bool _warpFarmer_doFade;
    //public new static void warpFarmer(LocationRequest locationRequest, int tileX, int tileY, int facingDirectionAfterWarp)
    //{
    //    var param = new object[] { locationRequest, tileX, tileY, facingDirectionAfterWarp, _warpFarmer_doFade };
    //    Game1Patcher.Game1_warpFarmOriginal.Invoke(null, param);
    //}
}

