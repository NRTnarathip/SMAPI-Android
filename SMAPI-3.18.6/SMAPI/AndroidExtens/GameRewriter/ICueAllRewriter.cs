using Microsoft.Xna.Framework.Audio;
using StardewValley;
using System;

namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    public interface ICueRewriter : ICue
    {
        float Pitch { get; set; }
        float Volume { get; set; }
        bool IsPitchBeingControlledByRPC { get; }
    }

    public class CueWrapper : ICueRewriter, IDisposable
    {
        private Cue cue;

        public bool IsStopped => cue.IsStopped;

        public bool IsStopping => cue.IsStopping;

        public bool IsPlaying => cue.IsPlaying;

        public bool IsPaused => cue.IsPaused;

        public string Name => cue.Name;

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

        public CueWrapper(Cue cue)
        {
            this.cue = cue;
        }

        public void Play()
        {
            cue.Play();
        }

        public void Pause()
        {
            cue.Pause();
        }

        public void Resume()
        {
            cue.Resume();
        }

        public void Stop(AudioStopOptions options)
        {
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            cue.Stop(options);
        }

        public void SetVariable(string var, int val)
        {
            cue.SetVariable(var, (float)val);
        }

        public void SetVariable(string var, float val)
        {
            cue.SetVariable(var, val);
        }

        public float GetVariable(string var)
        {
            return cue.GetVariable(var);
        }

        public void Dispose()
        {
            cue.Dispose();
            cue = null;
        }
    }

    public class DummyCue : ICueRewriter, IDisposable
    {
        public bool IsStopped => true;

        public bool IsStopping => false;

        public bool IsPlaying => false;

        public bool IsPaused => false;

        public string Name => "";

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

        public void Play()
        {
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        public void SetVariable(string var, int val)
        {
        }

        public void SetVariable(string var, float val)
        {
        }

        public float GetVariable(string var)
        {
            return 0f;
        }

        public void Stop(AudioStopOptions options)
        {
        }

        public void Dispose()
        {
        }
    }
}