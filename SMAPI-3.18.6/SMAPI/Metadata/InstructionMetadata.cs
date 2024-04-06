using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.ModLoading;
using StardewModdingAPI.Framework.ModLoading.Finders;
using StardewModdingAPI.Framework.ModLoading.Rewriters;
using StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_5;
using StardewValley;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace StardewModdingAPI.Metadata
{
    /// <summary>Provides CIL instruction handlers which rewrite mods for compatibility, and detect low-level mod issues like incompatible code.</summary>
    internal class InstructionMetadata
    {
        /*********
        ** Fields
        *********/
        /// <summary>The assembly names to which to heuristically detect broken references.</summary>
        /// <remarks>The current implementation only works correctly with assemblies that should always be present.</remarks>
        private readonly ISet<string> ValidateReferencesToAssemblies = new HashSet<string> { "StardewModdingAPI", "Stardew Valley", "StardewValley", "Netcode" };


        /*********
        ** Public methods
        *********/
        /// <summary>Get rewriters which detect or fix incompatible CIL instructions in mod assemblies.</summary>
        /// <param name="paranoidMode">Whether to detect paranoid mode issues.</param>
        /// <param name="rewriteMods">Whether to get handlers which rewrite mods for compatibility.</param>
        public IEnumerable<IInstructionHandler> GetHandlers(bool paranoidMode, bool rewriteMods)
        {
            /****
            ** rewrite CIL to fix incompatible code
            ****/
            // rewrite for crossplatform compatibility
            if (rewriteMods)
            {
                // heuristic rewrites
                yield return new HeuristicFieldRewriter(this.ValidateReferencesToAssemblies);
                yield return new HeuristicMethodRewriter(this.ValidateReferencesToAssemblies);

                //reReference module
                //StringSplitOptions.
                yield return new ModuleReferenceRewriter("System.*", new Dictionary<string, Version>
                {
                {
                    "System.Collections",
                    new Version(4, 0)
                },
                {
                    "System.Runtime.InteropServices",
                    new Version(4, 0)
                },
                {
                    "System.IO",
                    new Version(4, 0)
                },
                {
                    "System.Reflection",
                    new Version(4, 0)
                },
                {
                    "System.Text.Encoding",
                    new Version(4, 0)
                },
                {
                    "System.IO.FileSystem.Primitives",
                    new Version(4, 0)
                },
                {
                    "System.Linq.Expressions",
                    new Version(4, 0)
                },
                {
                    "System.Text.RegularExpressions",
                    new Version(4, 0)
                },
                {
                    "System.Runtime.Extensions",
                    new Version(4, 0)
                },
                {
                    "System.Linq",
                    new Version(4, 0)
                },
                {
                    "System.Reflection.Extensions",
                    new Version(4, 0)
                },
                {
                    "System.Globalization",
                    new Version(4, 0)
                },
                {
                    "System.IO.FileSystem",
                    new Version(4, 0)
                },
                {
                    "System.Console",
                    new Version(4, 0)
                },
                {
                    "System.Threading",
                    new Version(4, 0)
                },
                {
                    "System.Threading.Tasks",
                    new Version(4, 0)
                },
                {
                    "System.Text.Encoding.Extensions",
                    new Version(4, 0)
                },
                {
                    "System.",
                    new Version(5, 0)
                }
                }, new Assembly[]
                {
                        typeof(CollectionBase).Assembly,
                        typeof(ISet<>).Assembly,
                        typeof(HashSet<>).Assembly,
                        typeof(XmlDocument).Assembly,
                        typeof(XComment).Assembly,
                        typeof(IReadOnlySet<>).Assembly,
                        typeof(ReadOnlySpan<>).Assembly,
                        typeof(System.Data.DataTable).Assembly,
                });

                // specific versions
                yield return new ReplaceReferencesRewriter()
                    // Stardew Valley 1.5 (fields moved)
                    .MapField("Netcode.NetCollection`1<StardewValley.Objects.Furniture> StardewValley.Locations.DecoratableLocation::furniture", typeof(GameLocation), nameof(GameLocation.furniture))
                    .MapField("Netcode.NetCollection`1<StardewValley.TerrainFeatures.ResourceClump> StardewValley.Farm::resourceClumps", typeof(GameLocation), nameof(GameLocation.resourceClumps))
                    .MapField("Netcode.NetCollection`1<StardewValley.TerrainFeatures.ResourceClump> StardewValley.Locations.MineShaft::resourceClumps", typeof(GameLocation), nameof(GameLocation.resourceClumps))

                    // Stardew Valley 1.5.5 (XNA => MonoGame method changes)
                    .MapFacade<SpriteBatch, SpriteBatchFacade>()

                    // Stardew Valley PC To Android
                    //.MapFacade<OptionsPage, OptionsPageRewrite>()
                    .MapFacade<Game1, Game1Rewrite>();

                // 32-bit to 64-bit in Stardew Valley 1.5.5
                yield return new ArchitectureAssemblyRewriter();

                // detect Harmony & rewrite for SMAPI 3.12 (Harmony 1.x => 2.0 update)
                yield return new HarmonyRewriter();

#if SMAPI_DEPRECATED
                // detect issues for SMAPI 4.0.0
                yield return new LegacyAssemblyFinder();
#endif
            }
            else
                yield return new HarmonyRewriter(shouldRewrite: false);

            /****
            ** detect mod issues
            ****/
            // broken code
            yield return new ReferenceToMissingMemberFinder(this.ValidateReferencesToAssemblies);
            yield return new ReferenceToMemberWithUnexpectedTypeFinder(this.ValidateReferencesToAssemblies);

            // code which may impact game stability
            yield return new FieldFinder(typeof(SaveGame).FullName!, new[] { nameof(SaveGame.serializer), nameof(SaveGame.farmerSerializer), nameof(SaveGame.locationSerializer) }, InstructionHandleResult.DetectedSaveSerializer);
            yield return new EventFinder(typeof(ISpecializedEvents).FullName!, new[] { nameof(ISpecializedEvents.UnvalidatedUpdateTicked), nameof(ISpecializedEvents.UnvalidatedUpdateTicking) }, InstructionHandleResult.DetectedUnvalidatedUpdateTick);

            // paranoid issues
            if (paranoidMode)
            {
                // direct console access
                yield return new TypeFinder(typeof(System.Console).FullName!, InstructionHandleResult.DetectedConsoleAccess);

                // filesystem access
                yield return new TypeFinder(
                    new[]
                    {
                        typeof(System.IO.File).FullName!,
                        typeof(System.IO.FileStream).FullName!,
                        typeof(System.IO.FileInfo).FullName!,
                        typeof(System.IO.Directory).FullName!,
                        typeof(System.IO.DirectoryInfo).FullName!,
                        typeof(System.IO.DriveInfo).FullName!,
                        typeof(System.IO.FileSystemWatcher).FullName!
                    },
                    InstructionHandleResult.DetectedFilesystemAccess
                );

                // shell access
                yield return new TypeFinder(typeof(System.Diagnostics.Process).FullName!, InstructionHandleResult.DetectedShellAccess);
            }
        }
    }
}
