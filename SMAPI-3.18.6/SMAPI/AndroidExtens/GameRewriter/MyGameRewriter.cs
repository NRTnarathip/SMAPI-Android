//namespace StardewModdingAPI.AndroidExtens.GameRewriter
//{
//    public static class MyGameRewriter
//    {
//        //called by AssemblyLoader.cs
//        internal static bool PostAssemblyLoaderRewriter(AssemblyLoader assemblyLoader, IModMetadata mod, AssemblyDefinition assembly)
//        {
//            //return false;
//            var asmName = assembly.Name.Name;
//            if (asmName.Contains("SAAT.API"))
//            {

//                var AddCue_StaticMethod = typeof(SoundBankWrapperRewriter).GetMethod("AddCue");
//                var SoundBankWrapperType = assembly.MainModule.ImportReference(typeof(SoundBankWrapper)).Resolve();
//                var ISoundBankInterfaceType = assembly.MainModule.ImportReference(typeof(ISoundBank)).Resolve();
//                var CueDefinitionType = assembly.MainModule.ImportReference(typeof(CueDefinition));

//                //MethodReference methodReference = assembly.ImportReference(AddCue_StaticMethod);
//                var Addcue_MethodReference = assembly.MainModule.ImportReference(AddCue_StaticMethod);
//                Console.WriteLine("SV: AddCue static: " + AddCue_StaticMethod);
//                if (instruction.Operand is GenericInstanceMethod genericInstanceMethod)
//                {
//                    GenericInstanceMethod genericInstanceMethod2 = new GenericInstanceMethod(methodReference);
//                    genericInstanceMethod2.GenericArguments.AddRange(genericInstanceMethod.GenericArguments);
//                    methodReference = genericInstanceMethod2;
//                }
//                instruction.Operand = Addcue_MethodReference;


//                return true;
//                try
//                {
//                    {
//                        var AddCue_InterfaceMethod = new MethodDefinition(nameof(SoundBankWrapperRewriter.AddCue),
//                            MethodAttributes.NewSlot
//                            | MethodAttributes.Public
//                            | MethodAttributes.HideBySig
//                            | MethodAttributes.Abstract
//                            | MethodAttributes.Virtual,
//                            assembly.MainModule.ImportReference(typeof(void)));
//                        var cueDefinitionParam = new ParameterDefinition("cue_definition", ParameterAttributes.None, CueDefinitionType);
//                        AddCue_InterfaceMethod.Parameters.Add(cueDefinitionParam);
//                        ISoundBankInterfaceType.Methods.Add(AddCue_InterfaceMethod);
//                    }

//                    try
//                    {
//                        MethodDefinition addCueMethod = new MethodDefinition("AddCue",
//                            MethodAttributes.Public
//                            | MethodAttributes.Final
//                             | MethodAttributes.HideBySig
//                            | MethodAttributes.NewSlot
//                            | MethodAttributes.Virtual,
//                            assembly.MainModule.TypeSystem.Void);
//                        ParameterDefinition cueDefinitionParam = new ParameterDefinition("cue_definition", ParameterAttributes.None, CueDefinitionType);
//                        addCueMethod.Parameters.Add(cueDefinitionParam);
//                        addCueMethod.Body = new MethodBody(addCueMethod);
//                        var il = addCueMethod.Body.GetILProcessor();
//                        il.Emit(OpCodes.Ldarg_0); // Load 'this' reference
//                        il.Emit(OpCodes.Ldfld, SoundBankWrapperType.Fields.FirstOrDefault(f => f.Name == "soundBank")); // Load 'soundBank' field
//                        il.Emit(OpCodes.Ldarg_1); // Load 'cue_definition' parameter
//                        il.Emit(OpCodes.Callvirt, ISoundBankInterfaceType.Methods.FirstOrDefault(m => m.Name == "AddCue")); // Call 'AddCue' method
//                        il.Emit(OpCodes.Ret); // Return
//                        SoundBankWrapperType.Methods.Add(addCueMethod);

//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine("SV: Error try create method addCue: " + ex.Message);
//                    }


//                    // GetCueDefinition Method Class & Interface
//                    MethodDefinition GetCueDefinitionInterfaceMethod = new MethodDefinition("GetCueDefinition",
//                                           MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
//                                           CueDefinitionType);
//                    {
//                        ParameterDefinition nameParam = new ParameterDefinition("name", ParameterAttributes.None, assembly.MainModule.TypeSystem.String);
//                        GetCueDefinitionInterfaceMethod.Parameters.Add(nameParam);
//                    }
//                    ISoundBankInterfaceType.Methods.Add(GetCueDefinitionInterfaceMethod);
//                    try
//                    {
//                        MethodDefinition getCueDefinitionMethod = new MethodDefinition("GetCueDefinition",
//                            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
//                            CueDefinitionType);
//                        ParameterDefinition nameParam = new ParameterDefinition("name", ParameterAttributes.None, assembly.MainModule.TypeSystem.String);
//                        getCueDefinitionMethod.Parameters.Add(nameParam);
//                        getCueDefinitionMethod.Body = new MethodBody(getCueDefinitionMethod);
//                        var il = getCueDefinitionMethod.Body.GetILProcessor();
//                        il.Emit(OpCodes.Ldarg_0); // Load 'this' reference
//                        il.Emit(OpCodes.Ldfld, SoundBankWrapperType.Fields.FirstOrDefault(f => f.Name == "soundBank")); // Load 'soundBank' field
//                        il.Emit(OpCodes.Ldarg_1); // Load 'name' parameter
//                        // Call 'GetCueDefinition' method
//                        il.Emit(OpCodes.Callvirt, GetCueDefinitionInterfaceMethod);
//                        il.Emit(OpCodes.Ret); // Return
//                        SoundBankWrapperType.Methods.Add(getCueDefinitionMethod);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine("SV: try to create method GetCueDef: " + ex.Message);
//                    }

//                    foreach (var t in ISoundBankInterfaceType.Methods)
//                    {
//                        Console.WriteLine("SV: method interface: " + t.Name);
//                    }
//                    foreach (var t in SoundBankWrapperType.Methods)
//                    {
//                        Console.WriteLine("SV: found method: " + t.Name);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("SV: Error Try To Patch SAAT:" + ex.Message);
//                }
//                Console.WriteLine("SV: Success make IL all AddCue & GetCueDef: " + asmName);
//                return true;
//            }
//            return false;
//        }
//    }
//}