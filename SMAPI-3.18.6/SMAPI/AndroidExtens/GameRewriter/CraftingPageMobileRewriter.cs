using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;

namespace StardewModdingAPI.AndroidExtens.GameRewriter;

public class CraftingPageMobileRewriter : CraftingPageMobile
{
    public CraftingPageMobileRewriter(int x, int y, int width, int height,
        bool cooking = false, bool standalone_menu = false,
        List<Chest> material_containers = null)
            : base(x, y, width, height, cooking, 300, material_containers)
    {
    }

    public new List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes => this.pagesOfCraftingRecipes;
}