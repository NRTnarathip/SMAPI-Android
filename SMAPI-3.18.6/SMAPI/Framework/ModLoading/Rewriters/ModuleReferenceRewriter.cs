using Mono.Cecil;
using StardewModdingAPI.Framework.ModLoading.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StardewModdingAPI.Framework.ModLoading.Rewriters
{
    internal class ModuleReferenceRewriter : BaseInstructionHandler
    {
        private readonly Dictionary<string, Version> AssemblyRules;

        private readonly Dictionary<string, AssemblyNameReference> TargetMap = new Dictionary<string, AssemblyNameReference>();

        public ModuleReferenceRewriter(Dictionary<string, Version> assemblyRules,
            Assembly[] assemblies) : base("System.* assembly ref")
        {
            AssemblyRules = assemblyRules;
            foreach (var assembly in assemblies)
            {
                var asmNameReference = AssemblyNameReference.Parse(assembly.FullName);
                var dictionary = assembly.GetTypes().ToDictionary((Type type) => type.FullName, (Type p) => asmNameReference);
                foreach (KeyValuePair<string, AssemblyNameReference> item in dictionary)
                {
                    TargetMap.TryAdd(item.Key, item.Value);
                }
            }
        }

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

        public override bool Handle(ModuleDefinition module)
        {
            if (!module.AssemblyReferences.Any(IsMatch))
                return false;

            var enumerable = from p in module.GetTypeReferences().Where(IsMatch) orderby p.FullName select p;
            var hashSet = new HashSet<string>();
            foreach (var item in enumerable)
            {
                var assemblyNameReference = TargetMap[item.FullName.Split('/')[0]];
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
            return this.MarkRewritten();
        }
    }
}