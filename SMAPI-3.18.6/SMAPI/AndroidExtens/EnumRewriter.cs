using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;
using StardewModdingAPI.Framework.ModLoading.Framework;
using System;

namespace StardewModdingAPI.AndroidExtens
{
    public static class EnumMethods
    {
        //support Template Type
        //public static bool IsDefined<TEnum>(TEnum value) where TEnum : struct, Enum
        //{
        //    return Enum.IsDefined(typeof(TEnum), value);
        //}
        public static bool IsDefined<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            Type typeFromHandle = typeof(TEnum);
            return Enum.IsDefined(typeFromHandle, value);
        }
    }
    internal class EnumRewriter : BaseInstructionHandler
    {
        public EnumRewriter() : base("done enum method rewriter")
        {

        }
        public override bool Handle(ModuleDefinition module, ILProcessor cil, Instruction instruction)
        {
            var thisMethod = RewriteHelper.AsMethodReference(instruction);
            if (thisMethod == null)
                return false;

            if (thisMethod.Name == "IsDefined" && thisMethod.Parameters.Count == 1)
            {
                RewriteIsDefined(module, cil, instruction);
                return MarkRewritten();
            }

            return false;
        }
        void RewriteIsDefined(ModuleDefinition module, ILProcessor cil, Instruction instruction)
        {
            var newMethodInfo = typeof(EnumMethods).GetMethod("IsDefined");
            var methodRef = module.ImportReference(newMethodInfo);
            var genericInstanceMethod = instruction.Operand as GenericInstanceMethod;

            var newMethodOperand = new GenericInstanceMethod(methodRef);
            newMethodOperand.GenericArguments.AddRange(genericInstanceMethod.GenericArguments);
            instruction.Operand = newMethodOperand;
        }
    }
}