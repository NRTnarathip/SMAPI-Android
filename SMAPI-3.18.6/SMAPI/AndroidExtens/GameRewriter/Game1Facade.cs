//Rewrite code For PC
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
public class Game1Facade : Game1
{
    public new static IList<GameLocation> locations => Game1.locations;
    public new static IList<IClickableMenu> onScreenMenus => Game1.onScreenMenus;
    public new static RainDrop[] rainDrops => [.. Game1.rainDrops];//fix for mod TXMLoader
}

