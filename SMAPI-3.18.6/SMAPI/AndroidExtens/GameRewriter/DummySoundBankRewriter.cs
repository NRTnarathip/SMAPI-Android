using Microsoft.Xna.Framework.Audio;
using StardewValley;

namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    public class DummySoundBankRewriter : DummySoundBank
    {
        private ICue dummyCue = new DummyCueRewriter();
        public void AddCue(CueDefinition cue_definition) { }
        public CueDefinition GetCueDefinition(string cue_name)
        {
            return null;
        }
    }
}