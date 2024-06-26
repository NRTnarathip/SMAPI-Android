using StardewModdingAPI.Events;
using System;

namespace StardewModdingAPI.Framework.Content
{
    /// <summary>An edit to apply to an asset when it's requested from the content pipeline.</summary>
    /// <param name="Mod">The mod applying the edit.</param>
    /// <param name="Priority">If there are multiple edits that apply to the same asset, the priority with which this one should be applied.</param>
    /// <param name="OnBehalfOf">The content pack on whose behalf the edit is being applied, if any.</param>
    /// <param name="ApplyEdit">Apply the edit to an asset.</param>
    internal struct AssetEditOperation(IModMetadata Mod, AssetEditPriority Priority, IModMetadata? OnBehalfOf, Action<IAssetData> ApplyEdit)
    {
        public readonly IModMetadata Mod = Mod;
        public readonly AssetEditPriority Priority = Priority;
        public readonly IModMetadata OnBehalfOf = OnBehalfOf;
        public readonly Action<IAssetData> ApplyEdit = ApplyEdit;
    }
}
