using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Framework.ModLoading.Framework;

#pragma warning disable CS0109 // Member does not hide an inherited member, new keyword is not required: This is deliberate to support legacy XNA Framework platforms.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member: This is internal code to support rewriters that shouldn't be called directly.

namespace StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_5
{
    /// <summary>Provides <see cref="SpriteBatch"/> method signatures that can be injected into mod code for compatibility with mods written for XNA Framework before Stardew Valley 1.5.5.</summary>
    /// <remarks>This is public to support SMAPI rewriting and should never be referenced directly by mods. See remarks on <see cref="ReplaceReferencesRewriter"/> for more info.</remarks>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = SuppressReasons.UsedViaRewriting)]
    public class SpriteBatchFacade : SpriteBatch
    {
        /****
        ** XNA signatures
        ****/
        public new void Begin()
        {
            base.Begin();
        }

        public new void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
            base.Begin(sortMode, blendState);
        }

        public new void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            base.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState);
        }

        public new void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
            base.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect);
        }

        public new void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            base.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }


        /*********
        ** Private methods
        *********/
        private SpriteBatchFacade()
            : base(null)
        {
            RewriteHelper.ThrowFakeConstructorCalled();
        }
    }
}
