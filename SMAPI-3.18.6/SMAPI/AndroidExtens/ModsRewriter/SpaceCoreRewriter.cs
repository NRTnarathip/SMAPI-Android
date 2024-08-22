using Mono.Cecil;
using StardewModdingAPI.Framework.ModLoading.Framework;
using System;

namespace StardewModdingAPI.AndroidExtens.ModsRewriter
{
    internal class SpaceCoreRewriter : BaseInstructionHandler
    {
        public SpaceCoreRewriter() : base("Rewriter SpaceCore")
        {
        }
        public override bool Handle(ModuleDefinition module, TypeReference type, Action<TypeReference> replaceWith)
        {
            if (module.Assembly.Name.Name != "SpaceCore")
                return false;

            //if (type.FullName == "System.Single")
            //{
            //    return this.MarkRewritten();
            //}
            return false;
        }
    }
}