using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FEZAP.Features.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZAP.Features
{
    internal class ArtifactItemSet : IFezapCommand
    {
        public string Name => "artifacts";

        public string HelpText => "artifacts <give/remove/list> [name] - gives or removes an artifact to/from your inventory.";

        [ServiceDependency]
        public IGameStateManager GameState { private get; set; }

        public readonly ActorType[] AllowedActorTypes = new ActorType[]
        {
            ActorType.Tome,
            ActorType.TriSkull,
            ActorType.LetterCube,
            ActorType.NumberCube
        };

        public List<string> Autocomplete(string[] args)
        {
            if (args.Length == 1)
            {
                return new string[] { "give", "remove", "list" }
                .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))
                .ToList();
            }
            if (args.Length == 2)
            {
                return AllowedActorTypes
                    .Select(s => s.ToString().ToLower())
                    .Where(s => s.StartsWith(args[1], StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return null;
        }

        public bool Execute(string[] args)
        {
            if (args.Length >= 1 && args[0] != "give" && args[0] != "remove" && args[0] != "list")
            {
                FezapConsole.Print($"Invalid argument: '{args[0]}'", FezapConsole.OutputType.Warning);
                return false;
            }

            var artifactList = AllowedActorTypes.Select(a=>a.ToString().ToLower());

            if (args.Length != 2 || args[0] == "list")
            {
                if (args.Length >= 1 && (args[0] == "remove" || args[0] == "list"))
                {
                    FezapConsole.Print($"List of artifacts in your inventory:");
                    FezapConsole.Print($"{String.Join(", ", GameState.SaveData.Artifacts.Select(s => s.ToString().ToLower()))}");
                }
                else
                {
                    FezapConsole.Print($"List of available artifacts:");
                    FezapConsole.Print($"{String.Join(", ", artifactList.Select(s => s.ToLower()))}");
                }
                return true;
            }

            if (!Enum.TryParse<ActorType>(args[1], true, out var artifact))
            {
                if (args[0] == "remove" && args[1] == "all")
                {
                    GameState.SaveData.Artifacts.Clear();
                    FezapConsole.Print($"Removed all artifacts from player");
                    return true;
                }
                FezapConsole.Print($"Invalid actor type: {args[1]}.", FezapConsole.OutputType.Warning);
                return false;
            }

            if (!AllowedActorTypes.Contains(artifact))
            {
                FezapConsole.Print($"Given actor type is not a valid artifact.", FezapConsole.OutputType.Warning);
                return false;
            }

            if (args[0] == "remove")
            {
                if (!GameState.SaveData.Artifacts.Contains(artifact))
                {
                    FezapConsole.Print("You don't have this artifact in your inventory.", FezapConsole.OutputType.Warning);
                    return false;
                }
                GameState.SaveData.Artifacts.Remove(artifact);
                FezapConsole.Print($"Artifact \"{artifact}\" has been removed from your inventory!");
            }
            else
            {
                if (GameState.SaveData.Artifacts.Contains(artifact))
                {
                    FezapConsole.Print("You already have this artifact in your inventory.", FezapConsole.OutputType.Warning);
                    return false;
                }
                GameState.SaveData.Artifacts.Add(artifact);
                FezapConsole.Print($"Artifact \"{artifact}\" has been added to your inventory!");
            }

            return true;
        }
    }
}
