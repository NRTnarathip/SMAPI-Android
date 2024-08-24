using Mono.Cecil;
using Mono.Cecil.Cil;
using StardewModdingAPI.Framework.ModLoading.Framework;
using StardewModdingAPI.Framework.ModLoading.Rewriters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    internal class MapMethodToStaticMethodRewriter : BaseInstructionHandler
    {
        public class MapMethodToStaticKeyValue(MethodInfo srcMethod, MethodInfo newMethod)
        {
            public readonly MethodInfo srcMethod = srcMethod;
            public readonly MethodInfo newMethod = newMethod;
            public string srcMethodFullName = GetMethodFullName(srcMethod);
            public string newMethodFullName = GetMethodFullName(newMethod);
            public void AddPramToSrc(Type newType)
            {
                var isEmptyParam = srcMethodFullName[srcMethodFullName.Length - 2] == '(';
                if (isEmptyParam)
                {
                    srcMethodFullName = srcMethodFullName.Replace(")", $"{newType.FullName})");
                }
                else
                {
                    srcMethodFullName = srcMethodFullName.Replace(")", $",{newType.FullName})");
                }
            }
        }
        public static void Log(string msg) => AndroidLog.Log("Fixbug Rewriter: " + msg);

        //key == Src Method FullName
        public readonly Dictionary<string, MapMethodToStaticKeyValue> MapMethods = new();
        //lookup with type
        public MapMethodToStaticMethodRewriter() : base("map this method to static method(this object self, ...)")
        {

        }

        public static string GetMethodFullName(MethodInfo method)
        {
            var type = method.DeclaringType;
            var returnFullName = ReplaceReferencesRewriter.FormatCecilType(method.ReturnType);
            var paramsFullName = ReplaceReferencesRewriter.FormatCecilParameterList(method.GetParameters());
            return $"{returnFullName} {type}::{method.Name}({paramsFullName})";
        }
        public delegate bool SelectMethodCallbackDelegate(MethodInfo method);
        public delegate void PostEditAddMethod(MapMethodToStaticKeyValue mapMethod);

        public MethodInfo HandleSelectorMethod(Type type, SelectMethodCallbackDelegate selectorCallback)
        {
            var methods = type.GetMethods();
            foreach (var m in methods)
            {
                if (selectorCallback.Invoke(m))
                    return m;
            }
            return null;
        }

        public MapMethodToStaticMethodRewriter Add(Type srcType,
            SelectMethodCallbackDelegate srcMethodSelector,
            Type newType,
            SelectMethodCallbackDelegate newMethodSelector,
            PostEditAddMethod option = null)
        {
            var srcMethod = HandleSelectorMethod(srcType, srcMethodSelector);
            if (srcMethod == null)
            {
                Log("Errror not found src method in type: " + srcType);
                return this;

            }

            var newMethod = HandleSelectorMethod(newType, newMethodSelector);
            if (newMethod == null)
            {
                Log("Errror not found new method in type: " + newType);
                return this;
            }

            var mapMethod = new MapMethodToStaticKeyValue(srcMethod, newMethod);

            if (option != null)
                option.Invoke(mapMethod);

            //added make sure you finsih edit src & new method full name
            MapMethods.TryAdd(mapMethod.srcMethodFullName, mapMethod);
            return this;
        }
        public override bool Handle(ModuleDefinition module, ILProcessor cil, Instruction instruction)
        {
            var thisMethod = RewriteHelper.AsMethodReference(instruction);
            if (thisMethod == null)
                return false;

            if (MapMethods.TryGetValue(thisMethod.FullName, out var newMethodRef))
            {
                instruction.Operand = module.ImportReference(newMethodRef.newMethod);
                return this.MarkRewritten();
            }
            return false;
        }
    }
}