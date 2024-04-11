using Microsoft.Xna.Framework.Audio;
using StardewValley;
using System;

namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    public interface ISoundBankRewriter : ISoundBank
    {
        void AddCue(CueDefinition cue_definition);
        CueDefinition GetCueDefinition(string name);
    }
    public class SoundBankWrapper : ISoundBankRewriter, IDisposable
    {
        private SoundBank soundBank;

        public bool IsInUse => soundBank.IsInUse;

        public bool IsDisposed => soundBank.IsDisposed;

        public SoundBankWrapper(SoundBank soundBank)
        {
            this.soundBank = soundBank;
        }

        public ICue GetCue(string name)
        {
            return new CueWrapper(soundBank.GetCue(name));
        }

        public void PlayCue(string name)
        {
            soundBank.PlayCue(name);
        }

        public void PlayCue(string name, AudioListener listener, AudioEmitter emitter)
        {
            soundBank.PlayCue(name, listener, emitter);
        }

        public void Dispose()
        {
            soundBank.Dispose();
        }

        public void AddCue(CueDefinition cue_definition)
        {
            soundBank.AddCue(cue_definition);
        }

        public CueDefinition GetCueDefinition(string name)
        {
            return soundBank.GetCueDefinition(name);
        }
    }
    public class DummySoundBank : ISoundBankRewriter, IDisposable
    {
        private ICue dummyCue = new DummyCue();

        public bool IsInUse => false;

        public bool IsDisposed => true;

        public void Dispose()
        {
        }

        public ICue GetCue(string name)
        {
            return dummyCue;
        }

        public void PlayCue(string name)
        {
        }

        public void PlayCue(string name, AudioListener listener, AudioEmitter emitter)
        {
        }

        public void AddCue(CueDefinition cue_definition)
        {
        }

        public CueDefinition GetCueDefinition(string cue_name)
        {
            return null;
        }
    }
}