using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI.Framework.ModLoading.Framework;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member: This is internal code to support rewriters and shouldn't be called directly.

namespace StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_5
{
    /// <summary>Maps Harmony 1.x <see cref="AccessTools"/> methods to Harmony 2.x to avoid breaking older mods.</summary>
    /// <remarks>This is public to support SMAPI rewriting and should never be referenced directly by mods. See <see cref="HarmonyRewriter"/> for more info.</remarks>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = SuppressReasons.UsedViaRewriting)]
    public class AccessToolsFacade
    {
        /*********
        ** Public methods
        *********/
        public static ConstructorInfo DeclaredConstructor(Type type, Type[]? parameters = null)
        {
            // Harmony 1.x matched both static and instance constructors
            return
                AccessTools.DeclaredConstructor(type, parameters, searchForStatic: false)
                ?? AccessTools.DeclaredConstructor(type, parameters, searchForStatic: true);
        }

        public static ConstructorInfo Constructor(Type type, Type[]? parameters = null)
        {
            // Harmony 1.x matched both static and instance constructors
            return
                AccessTools.Constructor(type, parameters, searchForStatic: false)
                ?? AccessTools.Constructor(type, parameters, searchForStatic: true);
        }

        public static List<ConstructorInfo> GetDeclaredConstructors(Type type)
        {
            // Harmony 1.x matched both static and instance constructors
            return
                AccessTools.GetDeclaredConstructors(type, searchForStatic: false)
                ?? AccessTools.GetDeclaredConstructors(type, searchForStatic: true);
        }


        /*********
        ** Private methods
        *********/
        private AccessToolsFacade()
        {
            RewriteHelper.ThrowFakeConstructorCalled();
        }
    }
}
