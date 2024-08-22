using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StardewModdingAPI.AndroidExtens
{
    [HarmonyPatch(typeof(TitleMenu))]
    internal class TitleMenuPatcher
    {
        static SpriteFont font;
        static void RenderSMAPVersionIInfo(SpriteBatch b)
        {
            var viewport = Game1.viewport;
            var centerX = viewport.Width / 2f;
            if (font == null)
            {
                if (Game1.smallFont == null)
                    return;
                font = Game1.smallFont;
            }
            var text = $"SMAPI Version: {Constants.ApiVersion}";
            var textSizeRect = font.MeasureString(text);
            var pos = Vector2.Zero;
            pos.X = centerX - (textSizeRect.X / 2f);
            pos.Y = 16;

            b.DrawString(font, text, pos, Color.White);
            b.DrawString(font, "Port By NRTnarathip", pos + new Vector2(0, textSizeRect.Y), Color.White);
        }

        [HarmonyPostfix]
        [HarmonyPatch("draw", [typeof(SpriteBatch)])]
        static void PostfixDraw(SpriteBatch b, TitleMenu __instance)
        {
            //check if it's on customize character, so we dont need render
            if (TitleMenu.subMenu != null)
                return;

            RenderSMAPVersionIInfo(b);
        }
    }
}