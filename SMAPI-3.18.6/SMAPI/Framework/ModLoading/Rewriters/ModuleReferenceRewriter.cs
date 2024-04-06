using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StardewModdingAPI.Framework.ModLoading.Rewriters
{
    internal class ModuleReferenceRewriter : IInstructionHandler
    {
        private readonly Dictionary<string, Version> AssemblyRules;

        private readonly Dictionary<string, AssemblyNameReference> TargetMap = new Dictionary<string, AssemblyNameReference>();

        public ModuleReferenceRewriter(string phrase, Dictionary<string, Version>
            assemblyRules, Assembly[] assemblies)
        {
            DefaultPhrase = phrase + " assembly ref";
            AssemblyRules = assemblyRules;
            foreach (Assembly assembly in assemblies)
            {
                AssemblyNameReference target = AssemblyNameReference.Parse(assembly.FullName);
                Dictionary<string, AssemblyNameReference> dictionary = assembly.GetTypes().ToDictionary((Type p) => p.FullName, (Type p) => target);
                foreach (KeyValuePair<string, AssemblyNameReference> item in dictionary)
                {
                    TargetMap.TryAdd(item.Key, item.Value);
                    if (item.Key.Contains("ToArray"))
                        Android.Util.Log.Debug("NRT Debug", $"Map item: {item.Key} :: {item.Value.FullName}");
                }
            }
        }

        public string DefaultPhrase { get; }

        public ISet<InstructionHandleResult> Flags { get; } = new HashSet<InstructionHandleResult>();


        public ISet<string> Phrases { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private bool IsMatch(AssemblyNameReference reference)
        {
            foreach (var (text2, value) in AssemblyRules)
            {
                if (text2.EndsWith('.'))
                {
                    if (reference.Name.Equals(text2) || reference.Name.StartsWith(text2))
                    {
                        return reference.Version.CompareTo(value) >= 0;
                    }
                }
                else if (reference.Name.Equals(text2))
                {
                    return reference.Version.CompareTo(value) >= 0;
                }
            }
            return false;
        }

        private bool IsMatch(TypeReference reference)
        {
            foreach (var (text2, _) in AssemblyRules)
            {
                if (text2.EndsWith('.'))
                {
                    if (reference.Scope.Name.Equals(text2) || reference.Scope.Name.StartsWith(text2))
                    {
                        return TargetMap.ContainsKey(reference.FullName.Split('/')[0]);
                    }
                }
                else if (reference.Scope.Name.Equals(text2))
                {
                    return TargetMap.ContainsKey(reference.FullName.Split('/')[0]);
                }
            }
            return false;
        }

        public bool Handle(ModuleDefinition module)
        {
            if (!module.AssemblyReferences.Any(IsMatch))
            {
                return false;
            }
            IEnumerable<TypeReference> enumerable = from p in module.GetTypeReferences().Where(IsMatch)
                                                    orderby p.FullName
                                                    select p;
            HashSet<string> hashSet = new HashSet<string>();
            foreach (TypeReference item in enumerable)
            {
                AssemblyNameReference assemblyNameReference = TargetMap[item.FullName.Split('/')[0]];
                if (!module.AssemblyReferences.Contains(assemblyNameReference) && !hashSet.Contains(assemblyNameReference.FullName))
                {
                    module.AssemblyReferences.Add(assemblyNameReference);
                    hashSet.Add(assemblyNameReference.FullName);
                }
                item.Scope = assemblyNameReference;
            }
            foreach (TypeDefinition type in module.GetTypes())
            {
                foreach (CustomAttribute customAttribute in type.CustomAttributes)
                {
                    foreach (CustomAttributeArgument constructorArgument in customAttribute.ConstructorArguments)
                    {
                        if (constructorArgument.Value is TypeReference typeReference && TargetMap.ContainsKey(typeReference.FullName))
                        {
                            AssemblyNameReference assemblyNameReference2 = TargetMap[type.FullName.Split('/')[0]];
                            if (!module.AssemblyReferences.Contains(assemblyNameReference2) && !hashSet.Contains(assemblyNameReference2.FullName))
                            {
                                module.AssemblyReferences.Add(assemblyNameReference2);
                                hashSet.Add(assemblyNameReference2.FullName);
                            }
                            type.Scope = assemblyNameReference2;
                        }
                    }
                }
            }
            for (int num = module.AssemblyReferences.Count - 1; num >= 0; num--)
            {
                if (IsMatch(module.AssemblyReferences[num]))
                {
                    module.AssemblyReferences.RemoveAt(num);
                }
            }
            Flags.Add(InstructionHandleResult.Rewritten);
            return true;
        }

        public bool Handle(ModuleDefinition module, TypeReference type, Action<TypeReference> replaceWith)
        {
            return false;
        }

        public bool Handle(ModuleDefinition module, ILProcessor cil, Instruction instruction)
        {
            return false;
        }

        public void Reset()
        {
            Flags.Clear();
            Phrases.Clear();
        }
    }
}