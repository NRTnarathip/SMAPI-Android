using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI.Framework.ModLoading.Framework;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member: This is internal code to support rewriters and shouldn't be called directly.

namespace StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_5
{
    /// <summary>Maps Harmony 1.x <see cref="HarmonyMethod"/> methods to Harmony 2.x to avoid breaking older mods.</summary>
    /// <remarks>This is public to support SMAPI rewriting and should never be referenced directly by mods. See <see cref="HarmonyRewriter"/> for more info.</remarks>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = SuppressReasons.UsedViaRewriting)]
    public class HarmonyMethodFacade : HarmonyMethod
    {
        /*********
        ** Public methods
        *********/
        public HarmonyMethodFacade(MethodInfo method)
        {
            this.ImportMethodImpl(method);
        }

        public HarmonyMethodFacade(Type type, string name, Type[]? parameters = null)
        {
            this.ImportMethodImpl(AccessTools.Method(type, name, parameters));
        }


        /*********
        ** Private methods
        *********/
        // note: we deliberately don't use RewriteHelper.ThrowFakeConstructorCalled() here, since the constructors are
        // used via HarmonyRewriter.

        /// <summary>Import a method directly using the internal HarmonyMethod code.</summary>
        /// <param name="methodInfo">The method to import.</param>
        private void ImportMethodImpl(MethodInfo methodInfo)
        {
            // A null method is no longer allowed in the constructor with Harmony 2.0, but the
            // internal code still handles null fine. For backwards compatibility, this bypasses
            // the new restriction when the mod hasn't been updated for Harmony 2.0 yet.

            MethodInfo? importMethod = typeof(HarmonyMethod).GetMethod("ImportMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            if (importMethod == null)
                throw new InvalidOperationException("Can't find 'HarmonyMethod.ImportMethod' method");
            importMethod.Invoke(this, new object[] { methodInfo });
        }
    }
}
