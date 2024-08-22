using Mono.Cecil;
using StardewModdingAPI.Framework.ModLoading.Framework;
using System;

namespace StardewModdingAPI.AndroidExtens
{
    internal class TypeRewriter : BaseInstructionHandler
    {
        string originalType;
        Type newType;

        public TypeRewriter(string originalType, Type newType) : base("rewrite type: " + originalType)
        {
            this.originalType = originalType;
            this.newType = newType;
        }

        public override bool Handle(ModuleDefinition module, TypeReference type, Action<TypeReference> replaceWith)
        {
            if (type.FullName != originalType)
                return false;
            replaceWith(module.ImportReference(newType));
            return MarkRewritten();
        }
    }
}