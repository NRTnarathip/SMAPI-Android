namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    public interface ICueFacde
    {
        float Pitch { get; set; }
        float Volume { get; set; }
        bool IsPitchBeingControlledByRPC { get; }
    }
}