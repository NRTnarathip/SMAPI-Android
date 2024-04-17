using StardewValley.Menus;
using System.Collections.Generic;
using System.Reflection;

namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    public class OptionsPageFacade : OptionsPage
    {
        static FieldInfo _optionsField = null;
        public List<OptionsElement> options
        {
            get
            {
                if (_optionsField == null)
                {
                    _optionsField = typeof(OptionsPage).GetField(nameof(options), BindingFlags.Instance | BindingFlags.NonPublic);
                }
                return _optionsField.GetValue(this) as List<OptionsElement>;
            }
        }

        public OptionsPageFacade(int x, int y, int width, int height, float widthMod = 1, float heightMod = 1)
            : base(x, y, width, height, widthMod, heightMod)
        {

        }
    }
}