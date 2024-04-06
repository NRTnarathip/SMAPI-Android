using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

namespace StardewModdingAPI.Framework.Content
{
    /// <summary>An operation which provides the initial instance of an asset when it's requested from the content pipeline.</summary>
    /// <param name="Mod">The mod applying the edit.</param>
    /// <param name="Priority">If there are multiple loads that apply to the same asset, the priority with which this one should be applied.</param>
    /// <param name="OnBehalfOf">The content pack on whose behalf the asset is being loaded, if any.</param>
    /// <param name="GetData">Load the initial value for an asset.</param>
    //internal record AssetLoadOperation(IModMetadata Mod, IModMetadata? OnBehalfOf, AssetLoadPriority Priority, Func<IAssetInfo, object> GetData);
    internal readonly struct AssetLoadOperation(IModMetadata Mod, IModMetadata? OnBehalfOf,
        AssetLoadPriority Priority, Func<IAssetInfo, object> GetData)
    {
        public IModMetadata Mod { get; } = Mod;
        public IModMetadata? OnBehalfOf { get; } = OnBehalfOf;
        public AssetLoadPriority Priority { get; } = Priority;
        public Func<IAssetInfo, object> GetData { get; } = GetData;

        public override bool Equals(object obj)
        {
            return obj is AssetLoadOperation operation &&
                   EqualityComparer<IModMetadata>.Default.Equals(Mod, operation.Mod) &&
                   EqualityComparer<IModMetadata>.Default.Equals(OnBehalfOf, operation.OnBehalfOf) &&
                   Priority == operation.Priority &&
                   EqualityComparer<Func<IAssetInfo, object>>.Default.Equals(GetData, operation.GetData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Mod, OnBehalfOf, Priority, GetData);
        }
    }
}
