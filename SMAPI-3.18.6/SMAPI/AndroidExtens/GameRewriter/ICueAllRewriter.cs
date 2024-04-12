using Microsoft.Xna.Framework.Audio;
using StardewValley;

namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    public interface ICueRewriter : ICue
    {
        float Pitch { get; set; }
        float Volume { get; set; }
        bool IsPitchBeingControlledByRPC { get; }
    }

    public class CueWrapperRewriter : StardewValley.CueWrapper
    {
        private Cue cue;
        public CueWrapperRewriter(Cue cue) : base(cue) { }
        public float Volume
        {
            get
            {
                return cue.Volume;
            }
            set
            {
                cue.Volume = value;
            }
        }
        public float Pitch
        {
            get
            {
                return cue.Pitch;
            }
            set
            {
                cue.Pitch = value;
            }
        }
        public bool IsPitchBeingControlledByRPC => cue.IsPitchBeingControlledByRPC;
    }

    public class DummyCueRewriter : StardewValley.DummyCue
    {
        public float Volume
        {
            get
            {
                return 1f;
            }
            set
            {
            }
        }
        public float Pitch
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }
        public bool IsPitchBeingControlledByRPC => true;
    }
}