using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.AssemblyRewriters;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.ModLoading;
using StardewModdingAPI.Framework.ModLoading.Finders;
using StardewModdingAPI.Framework.ModLoading.Rewriters;
using StardewValley;

namespace StardewModdingAPI.Metadata
{
    /// <summary>Provides CIL instruction handlers which rewrite mods for compatibility and throw exceptions for incompatible code.</summary>
    internal class InstructionMetadata
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get rewriters which detect or fix incompatible CIL instructions in mod assemblies.</summary>
        public IEnumerable<IInstructionHandler> GetHandlers()
        {
            return new IInstructionHandler[]
            {
                /****
                ** throw exception for incompatible code
                ****/
                // changes in Stardew Valley 1.2 (with no rewriters)
                new FieldFinder("StardewValley.Item", "set_Name", InstructionHandleResult.NotCompatible),

                // APIs removed in SMAPI 1.9
                new TypeFinder("StardewModdingAPI.Advanced.ConfigFile", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Advanced.IConfigFile", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Entities.SPlayer", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Extensions", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Inheritance.SGame", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Inheritance.SObject", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.LogWriter", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Manifest", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Version", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GraphicsEvents", "DrawDebug", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GraphicsEvents", "DrawTick", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GraphicsEvents", "OnPostRenderHudEventNoCheck", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GraphicsEvents", "OnPostRenderGuiEventNoCheck", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GraphicsEvents", "OnPreRenderHudEventNoCheck", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GraphicsEvents", "OnPreRenderGuiEventNoCheck", InstructionHandleResult.NotCompatible),

                // APIs removed in SMAPI 2.0
                new TypeFinder("StardewModdingAPI.Command", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Config", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Log", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GameEvents", "Initialize", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GameEvents", "LoadContent", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GameEvents", "GameLoaded", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.GameEvents", "FirstUpdateTick", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.PlayerEvents", "LoadedGame", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.PlayerEvents", "FarmerChanged", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.TimeEvents", "DayOfMonthChanged", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.TimeEvents", "YearOfGameChanged", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.TimeEvents", "SeasonOfYearChanged", InstructionHandleResult.NotCompatible),
                new EventFinder("StardewModdingAPI.Events.TimeEvents", "OnNewDay", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Events.EventArgsCommand", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Events.EventArgsFarmerChanged", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Events.EventArgsLoadedGameChanged", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Events.EventArgsNewDay", InstructionHandleResult.NotCompatible),
                new TypeFinder("StardewModdingAPI.Events.EventArgsStringChanged", InstructionHandleResult.NotCompatible),
                new PropertyFinder("StardewModdingAPI.Mod", "PathOnDisk", InstructionHandleResult.NotCompatible),
                new PropertyFinder("StardewModdingAPI.Mod", "BaseConfigPath", InstructionHandleResult.NotCompatible),
                new PropertyFinder("StardewModdingAPI.Mod", "PerSaveConfigFolder", InstructionHandleResult.NotCompatible),
                new PropertyFinder("StardewModdingAPI.Mod", "PerSaveConfigPath", InstructionHandleResult.NotCompatible),

                /****
                ** detect code which may impact game stability
                ****/
                new TypeFinder("Harmony.HarmonyInstance", InstructionHandleResult.DetectedGamePatch),
                new TypeFinder("System.Runtime.CompilerServices.CallSite", InstructionHandleResult.DetectedDynamic),
                new FieldFinder(typeof(SaveGame).FullName, nameof(SaveGame.serializer), InstructionHandleResult.DetectedSaveSerialiser),
                new FieldFinder(typeof(SaveGame).FullName, nameof(SaveGame.farmerSerializer), InstructionHandleResult.DetectedSaveSerialiser),
                new FieldFinder(typeof(SaveGame).FullName, nameof(SaveGame.locationSerializer), InstructionHandleResult.DetectedSaveSerialiser),

                /****
                ** rewrite CIL to fix incompatible code
                ****/
                // crossplatform
                new MethodParentRewriter(typeof(SpriteBatch), typeof(SpriteBatchMethods), onlyIfPlatformChanged: true),

                // Stardew Valley 1.2
                new FieldToPropertyRewriter(typeof(Game1), nameof(Game1.activeClickableMenu)),
                new FieldToPropertyRewriter(typeof(Game1), nameof(Game1.currentMinigame)),
                new FieldToPropertyRewriter(typeof(Game1), nameof(Game1.gameMode)),
                new FieldToPropertyRewriter(typeof(Game1), nameof(Game1.player)),
                new FieldReplaceRewriter(typeof(Game1), "borderFont", nameof(Game1.smallFont)),
                new FieldReplaceRewriter(typeof(Game1), "smoothFont", nameof(Game1.smallFont)),

                // SMAPI 1.9
                new TypeReferenceRewriter("StardewModdingAPI.Inheritance.ItemStackChange", typeof(ItemStackChange)),

                // SMAPI 2.0
                new VirtualEntryCallRemover() // Mod.Entry changed from virtual to abstract in SMAPI 2.0, which breaks the few mods which called base.Entry()
            };
        }
    }
}
