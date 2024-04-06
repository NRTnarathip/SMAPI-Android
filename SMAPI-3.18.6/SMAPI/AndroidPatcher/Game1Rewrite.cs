//Rewrite code For PC
using StardewValley;
using System.Collections.Generic;

public class Game1Rewrite : Game1
{
    public new static IList<GameLocation> locations => Game1.locations;

}

