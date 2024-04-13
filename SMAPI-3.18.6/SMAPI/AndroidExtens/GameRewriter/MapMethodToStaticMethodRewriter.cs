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
        public class MapMethodToStatic(MethodInfo srcMethod, MethodInfo newMethod)
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
        public static void Log(string msg) => Console.WriteLine("Rewriter: " + msg);

        //key == Src Method FullName
        public readonly Dictionary<string, MapMethodToStatic> MapMethods = new();
        //lookup with type
        public readonly HashSet<string> MapMethosTypeLookup = new();
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
        public delegate void PostEditAddMethod(MapMethodToStatic mapMethod);

        public MethodInfo SelectMethod(Type type, SelectMethodCallbackDelegate selectorCallback)
        {
            var methods = type.GetMethods();
            foreach (var m in methods)
            {
                if (selectorCallback.Invoke(m))
                    return m;
            }
            return null;
        }

        public MapMethodToStaticMethodRewriter Add(Type srcType, SelectMethodCallbackDelegate srcMethodSelector,
            Type newType, SelectMethodCallbackDelegate newMethodSelector,
            PostEditAddMethod option = null)
        {
            var srcMethod = SelectMethod(srcType, srcMethodSelector);
            if (srcMethod == null)
            {
                Log("Errror not found src method in type: " + srcType);
                return this;

            }

            var newMethod = SelectMethod(newType, newMethodSelector);
            if (newMethod == null)
            {
                Log("Errror not found new method in type: " + newType);
                return this;
            }

            var mapMethod = new MapMethodToStatic(srcMethod, newMethod);

            if (MapMethosTypeLookup.Contains(srcMethod.DeclaringType.FullName) == false)
                MapMethosTypeLookup.Add(srcMethod.DeclaringType.FullName);

            if (option != null)
                option.Invoke(mapMethod);

            //added make sure you finsih edit src & new method full name
            MapMethods.TryAdd(mapMethod.srcMethodFullName, mapMethod);
            Log("done add map method: " + mapMethod.srcMethodFullName);
            return this;
        }
        public static readonly Dictionary<string, MethodReference> MapMethodsHasEdit = new();
        public override bool Handle(ModuleDefinition module, ILProcessor cil, Instruction instruction)
        {
            MethodReference methodByIL = RewriteHelper.AsMethodReference(instruction);
            if (methodByIL == null)
                return false;

            if (!MapMethosTypeLookup.Contains(methodByIL.DeclaringType.FullName))
                return false;

            if (!MapMethods.ContainsKey(methodByIL.FullName))
                return false;

            var mapMethod = MapMethods[methodByIL.FullName];
            instruction.Operand = module.ImportReference(mapMethod.newMethod);
            MapMethodsHasEdit.TryAdd(methodByIL.FullName, methodByIL);
            return this.MarkRewritten();

        }
    }
}