using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI.Framework.ModLoading.Framework;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member: This is internal code to support rewriters and shouldn't be called directly.

namespace StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_5
{
    /// <summary>Maps Harmony 1.x <c>HarmonyInstance</c> methods to Harmony 2.x's <see cref="Harmony"/> to avoid breaking older mods.</summary>
    /// <remarks>This is public to support SMAPI rewriting and should never be referenced directly by mods. See <see cref="HarmonyRewriter"/> for more info.</remarks>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = SuppressReasons.UsedViaRewriting)]
    public class HarmonyInstanceFacade : Harmony
    {
        /*********
        ** Public methods
        *********/
        public static Harmony Create(string id)
        {
            return new Harmony(id);
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract", Justification = "If the user passes a null original method, we let it fail in the underlying Harmony instance instead of handling it here.")]
        public DynamicMethod Patch(MethodBase original, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, HarmonyMethod? transpiler = null)
        {
            // In Harmony 1.x you could target a virtual method that's not implemented by the
            // target type, but in Harmony 2.0 you need to target the concrete implementation.
            // This just resolves the method to the concrete implementation if needed.
            if (original != null)
                original = original.GetDeclaredMember();

            // call Harmony 2.0 and show a detailed exception if it fails
            try
            {
                MethodInfo method = base.Patch(original: original, prefix: prefix, postfix: postfix, transpiler: transpiler);
                return (DynamicMethod)method;
            }
            catch (Exception ex)
            {
                string patchTypes = this.GetPatchTypesLabel(prefix, postfix, transpiler);
                string methodLabel = this.GetMethodLabel(original);
                throw new Exception($"Harmony instance {this.Id} failed applying {patchTypes} to {methodLabel}.", ex);
            }
        }


        /*********
        ** Private methods
        *********/
        private HarmonyInstanceFacade()
            : base(null)
        {
            RewriteHelper.ThrowFakeConstructorCalled();
        }

        /// <summary>Get a human-readable label for the patch types being applies.</summary>
        /// <param name="prefix">The prefix method, if any.</param>
        /// <param name="postfix">The postfix method, if any.</param>
        /// <param name="transpiler">The transpiler method, if any.</param>
        private string GetPatchTypesLabel(HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, HarmonyMethod? transpiler = null)
        {
            var patchTypes = new List<string>();

            if (prefix != null)
                patchTypes.Add("prefix");
            if (postfix != null)
                patchTypes.Add("postfix");
            if (transpiler != null)
                patchTypes.Add("transpiler");

            return string.Join("/", patchTypes);
        }

        /// <summary>Get a human-readable label for the method being patched.</summary>
        /// <param name="method">The method being patched.</param>
        private string GetMethodLabel(MethodBase? method)
        {
            return method != null
                ? $"method {method.DeclaringType?.FullName}.{method.Name}"
                : "null method";
        }
    }
}
