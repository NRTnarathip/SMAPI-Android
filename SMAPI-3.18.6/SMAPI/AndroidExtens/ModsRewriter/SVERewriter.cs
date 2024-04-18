using Mono.Cecil;
using Mono.Cecil.Cil;
using StardewModdingAPI.Framework.ModLoading.Framework;
using System.Linq;

namespace StardewModdingAPI.AndroidExtens.ModsRewriter
{
    internal class SVERewriter : BaseInstructionHandler
    {
        const string dllName = "StardewValleyExpanded.dll";
        public SVERewriter() : base("SVE Rewriter")
        {

        }
        public override bool Handle(ModuleDefinition module)
        {
            if (module.Name != dllName)
                return false;

            var TMXLLoadMapFacingDirectionType = module.GetType("StardewValleyExpanded.HarmonyPatch_TMXLLoadMapFacingDirection");

            var ApplyPatchMethod = TMXLLoadMapFacingDirectionType.Methods.Single(m => m.Name == "ApplyPatch");
            var il = ApplyPatchMethod.Body.GetILProcessor();
            il.Clear();
            il.Emit(OpCodes.Ret);

            return false;
        }
    }
}