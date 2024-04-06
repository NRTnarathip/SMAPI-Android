#if SMAPI_DEPRECATED
using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI.Framework.ModLoading.Framework;

namespace StardewModdingAPI.Framework.ModLoading.RewriteFacades
{
    /// <summary>Stub version of <see cref="StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_5.AccessToolsFacade"/> to avoid breaking SpriteMaster.</summary>
    [Obsolete("This only exists for compatibility with older versions of SpriteMaster.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = SuppressReasons.UsedViaRewriting)]
    public class AccessToolsFacade { }
}
#endif
