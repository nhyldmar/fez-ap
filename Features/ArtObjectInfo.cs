using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FezGame.Services;
using FezEngine.Tools;
using FEZAP.Features.Console;

namespace FEZAP.Features
{
	internal class ArtObjectInfo : IFezapCommand
	{
		public string Name => "artobjectinfo";

		public string HelpText => "artobjectinfo: displays the ID of the art objects in the current level.";

		[ServiceDependency]
		public IGameLevelManager LevelManager { private get; set; }

		public ArtObjectInfo()
		{

		}

        public void Initialize() { }

		public List<string> Autocomplete(string[] _args) { return new List<string> { }; }

		public bool Execute(string[] args)
		{
			string result = "";
            if(args.Length > 0)
            {
                if(int.TryParse(args[0], out int artobjectID))
                {
                    //Note: some art objects get removed upon loading the level, and some art objects are added to the level after it's loaded.

                    if (LevelManager.ArtObjects.TryGetValue(artobjectID, out var artObjectInstance))
                    {
                        var artObject = artObjectInstance.ArtObject;
                        FezapConsole.Print($"ArtObject Name: {artObjectInstance.ArtObjectName}");
                        FezapConsole.Print($"Position: {artObjectInstance.Position}");
                        FezapConsole.Print($"Size: {artObject.Size}");
                        if (artObjectInstance.Scale != Vector3.One)
                        {
                            FezapConsole.Print($"Scale: {artObjectInstance.Scale}");
                        }
                        FezapConsole.Print($"Rotation: {artObjectInstance.Rotation}");
                        FezapConsole.Print($"Bounds: {artObjectInstance.Bounds}");
                        if (!artObjectInstance.Enabled)
                        {
                            FezapConsole.Print($"Enabled: {artObjectInstance.Enabled}");
                        }
                        if (artObjectInstance.ActorSettings != null)
                        {
                            var actorSettings = artObjectInstance.ActorSettings;
                            if (actorSettings.Inactive)
                            {
                                FezapConsole.Print($"Inactive: {actorSettings.Inactive}");
                            }
                            if (actorSettings.ContainedTrile != FezEngine.Structure.ActorType.None)
                            {
                                FezapConsole.Print($"ContainedTrile: {actorSettings.ContainedTrile}");
                            }
                            if (actorSettings.AttachedGroup != null)
                            {
                                FezapConsole.Print($"AttachedGroup: {actorSettings.AttachedGroup}");
                            }
                            if (actorSettings.SpinOffset != 0)
                            {
                                FezapConsole.Print($"SpinOffset: {actorSettings.SpinOffset}");
                            }
                            if (actorSettings.SpinEvery != 0)
                            {
                                FezapConsole.Print($"SpinEvery: {actorSettings.SpinEvery}");
                            }
                            if (actorSettings.SpinView != FezEngine.Viewpoint.None)
                            {
                                FezapConsole.Print($"SpinView: {actorSettings.SpinView}");
                            }
                            if (actorSettings.OffCenter)
                            {
                                FezapConsole.Print($"OffCenter: {actorSettings.OffCenter}");
                            }
                            if (actorSettings.RotationCenter != Vector3.Zero)
                            {
                                FezapConsole.Print($"RotationCenter: {actorSettings.RotationCenter}");
                            }
                            if(actorSettings.VibrationPattern != null && actorSettings.VibrationPattern.Length > 0)
                            {
                                FezapConsole.Print($"VibrationPattern: {String.Join(", ", actorSettings.VibrationPattern)}");
                            }
                            if(actorSettings.CodePattern != null && actorSettings.CodePattern.Length > 0)
                            {
                                FezapConsole.Print($"CodePattern: {String.Join(", ", actorSettings.CodePattern)}");
                            }
                            //if (actorSettings.Segment != null)
                            //{
                            //    FezapConsole.Print($"Segment: {actorSettings.Segment}");
                            //}
                            if (actorSettings.NextNode != null)
                            {
                                FezapConsole.Print($"NextNode: {actorSettings.NextNode}");
                            }
                            if (actorSettings.DestinationLevel != null)
                            {
                                FezapConsole.Print($"DestinationLevel: {actorSettings.DestinationLevel}");
                            }
                            if (actorSettings.TreasureMapName != null)
                            {
                                FezapConsole.Print($"TreasureMapName: {actorSettings.TreasureMapName}");
                            }
                            if (actorSettings.TimeswitchWindBackSpeed != 0f)
                            {
                                FezapConsole.Print($"TimeswitchWindBackSpeed: {actorSettings.TimeswitchWindBackSpeed}");
                            }
                            if (actorSettings.InvisibleSides != null && actorSettings.InvisibleSides.Count > 0)
                            {
                                FezapConsole.Print($"InvisibleSides: {String.Join(", ", actorSettings.InvisibleSides)}");
                            }
                            //if (actorSettings.NextNodeAo != null)
                            //{
                            //    FezapConsole.Print($"NextNodeAo: {actorSettings.NextNodeAo}");
                            //}
                            //if (actorSettings.PrecedingNodeAo != null)
                            //{
                            //    FezapConsole.Print($"PrecedingNodeAo: {actorSettings.PrecedingNodeAo}");
                            //}
                            if (actorSettings.ShouldMoveToEnd)
                            {
                                FezapConsole.Print($"ShouldMoveToEnd: {actorSettings.ShouldMoveToEnd}");
                            }
                            if (actorSettings.ShouldMoveToHeight != null)
                            {
                                FezapConsole.Print($"ShouldMoveToHeight: {actorSettings.ShouldMoveToHeight}");
                            }
                        }
                        return true;
                    }
                }
                FezapConsole.Print($"Unknown art object ID: {args[0]}");
                return false;
            }
            result += String.Join(", ", LevelManager.ArtObjects.Keys);
			FezapConsole.Print(result);
			return true;
		}
    }

	internal class ArtObjectWarp : IFezapCommand
	{
        public string Name => "artobjectwarp";

        public string HelpText => "artobjectwarp <id> - warps to the specified art object.";

        [ServiceDependency]
        public IGameLevelManager LevelManager { private get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        public ArtObjectWarp()
        {

        }

        public void Initialize() { }

        public List<string> Autocomplete(string[] _args) { return new List<string> { }; }

        public bool Execute(string[] args)
        {
            if(args.Length != 1)
            {
                FezapConsole.Print("Wrong number of arguments.");
                return false;
            }

            if(int.TryParse(args[0], out int artobjectID) && LevelManager.ArtObjects.TryGetValue(artobjectID, out var artObjectInstance))
            {
                PlayerManager.IgnoreFreefall = true;
                PlayerManager.Position = artObjectInstance.Bounds.GetCenter();
                return true;
            }
            FezapConsole.Print("Art Object ID given does not exist in the current level.");
            return false;

        }
    }
}

