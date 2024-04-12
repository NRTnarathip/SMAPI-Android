using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using Mono.Cecil;
using Mono.Cecil.Cil;
using StardewModdingAPI.Framework.ModLoading.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StardewModdingAPI.AndroidExtens.GameRewriter
{
    public static class SoundBankWrapperStaticMethod
    {
        public static void AddCue(this ISoundBank iSoundBank, CueDefinition cue_definition)
        {
            if (iSoundBank is SoundBankWrapper soundBankkWrapper)
            {
                var soundBank = typeof(SoundBankWrapper).GetField("soundBank",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic).GetValue(soundBankkWrapper) as SoundBank;
                soundBank.AddCue(cue_definition);
                return;
            }
            var addCueMethod = AccessTools.Method(iSoundBank.GetType(), "AddCue", [typeof(CueDefinition)]);
            if (addCueMethod != null)
            {
                var parameters = new CueDefinition[1] { cue_definition };
                addCueMethod.Invoke(iSoundBank, parameters);
            }
            return;
        }
        public static CueDefinition GetCueDefinition(this ISoundBank iSoundBank, string name)
        {
            if (iSoundBank is SoundBankWrapper obj)
            {
                return ((SoundBank)(typeof(SoundBankWrapper).GetField("soundBank",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic)?.GetValue(obj)))?.GetCueDefinition(name);
            }
            MethodInfo getCueDefMethod = AccessTools.Method(iSoundBank.GetType(), "GetCueDefinition", new Type[1] { typeof(string) });
            if (getCueDefMethod == null)
                return null;

            var parameters = new string[1] { name };
            var result = getCueDefMethod.Invoke(iSoundBank, parameters);
            return result as CueDefinition;
        }

    }
    internal class SoundBankRewriter : BaseInstructionHandler
    {
        public readonly HashSet<string> WhitelistAssemblyRewriter = new()
        {
            "SAAT.API.dll",
            "SAAT.Mod.dll"
        };

        public SoundBankRewriter(string defaultPhrase) : base(defaultPhrase)
        {
        }
        public override bool Handle(ModuleDefinition module, ILProcessor cil, Instruction instruction)
        {
            if (!WhitelistAssemblyRewriter.Contains(module.Name))
                return false;

            MethodReference currentMethod = RewriteHelper.AsMethodReference(instruction);
            if (currentMethod == null)
                return false;

            //Console.WriteLine($"SV: on handle: " + module.Name + $", ins to method {currentMethod.FullName}");
            if (currentMethod.FullName.Contains("AddCue"))
            {
                var AddCueStaticMethod = module.ImportReference(typeof(SoundBankWrapperStaticMethod).GetMethod("AddCue"));
                instruction.Operand = AddCueStaticMethod;
            }
            else if (currentMethod.FullName.Contains("GetCueDefinition"))
            {
                var GetCueDef = module.ImportReference(typeof(SoundBankWrapperStaticMethod).GetMethod("GetCueDefinition"));
                instruction.Operand = GetCueDef;
            }
            else
            {
                return false;
            }
            return MarkRewritten();
        }
    }
}