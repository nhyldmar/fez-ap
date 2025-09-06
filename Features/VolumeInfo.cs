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
	internal class VolumeInfo : IFezapCommand
	{
		public string Name => "volumeinfo";

		public string HelpText => "volumeinfo: displays the ID of the volumes in the current level.";

		[ServiceDependency]
		public IGameLevelManager LevelManager { private get; set; }

		public VolumeInfo()
		{

		}

        public void Initialize() { }

		public List<string> Autocomplete(string[] _args) { return new List<string> { }; }

		public bool Execute(string[] args)
		{
			string result = "";
            if(args.Length > 0)
            {
                if(int.TryParse(args[0], out int volumeId))
                {
                    if (LevelManager.Volumes.TryGetValue(volumeId, out var volume))
                    {
                        FezapConsole.Print($"From: {volume.From}");
                        FezapConsole.Print($"To: {volume.To}");
                        FezapConsole.Print($"Bounding Box: {volume.BoundingBox}");
                        FezapConsole.Print($"Enabled: {volume.Enabled}");
                        FezapConsole.Print($"Orientations: {String.Join(", ", volume.Orientations.OrderBy(a=>a).ToArray())}");
                        if (volume.ActorSettings != null)
                        {
                            var actorSettings = volume.ActorSettings;
                            if (actorSettings.IsPointOfInterest)
                            {
                                FezapConsole.Print($"IsPointOfInterest: {actorSettings.IsPointOfInterest}");
                            }
                            if (actorSettings.IsSecretPassage)
                            {
                                FezapConsole.Print($"IsSecretPassage: {actorSettings.IsSecretPassage}");
                            }
                            if (actorSettings.IsBlackHole)
                            {
                                FezapConsole.Print($"IsBlackHole: {actorSettings.IsBlackHole}");
                            }
                            if (actorSettings.WaterLocked)
                            {
                                FezapConsole.Print($"WaterLocked: {actorSettings.WaterLocked}");
                            }
                            if (actorSettings.NeedsTrigger)
                            {
                                FezapConsole.Print($"NeedsTrigger: {actorSettings.NeedsTrigger}");
                            }
                            if(actorSettings.CodePattern != null && actorSettings.CodePattern.Length > 0)
                            {
                                FezapConsole.Print($"CodePattern: {String.Join(", ", actorSettings.CodePattern)}");
                            }
                        }
                        return true;
                    }
                }
                FezapConsole.Print($"Unknown volume ID: {args[0]}");
                return false;
            }
			var Volumes = LevelManager.Volumes.Values;
			foreach (var volume in Volumes)
			{
				if(!(volume.ActorSettings != null && volume.ActorSettings.IsBlackHole))
				{
                    result += (volume.Id.ToString() + ", ");
                }
			}
			FezapConsole.Print(result);
			return true;
		}
    }

	internal class VolumeWarp : IFezapCommand
	{
        public string Name => "volumewarp";

        public string HelpText => "volumewarp <id> - warps to the specified volume.";

        [ServiceDependency]
        public IGameLevelManager LevelManager { private get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        public VolumeWarp()
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

            int volumeId = int.Parse(args[0]);

            if(!LevelManager.VolumeExists(volumeId))
            {
                FezapConsole.Print("Volume ID given does not exist in the current level or is a blackhole.");
                return false;
            }

            var Volume = LevelManager.Volumes[int.Parse(args[0])];
            PlayerManager.IgnoreFreefall = true;
            PlayerManager.Position = (Volume.From + Volume.To) / 2.0f;
            return true;
        }
    }
}

