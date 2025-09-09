using FezEngine.Tools;
using FezGame.Services;
using FEZAP.Features.Console;

namespace FEZAP.Features
{
    internal class ItemCountSet : IFezapCommand
    {
        public string Name => "itemcount";
        public string HelpText => "itemcount <goldens/antis/bits/hearts/keys/owls> <count> - sets number of collected items of given type.";

        [ServiceDependency]
        public IGameLevelManager LevelManager { private get; set; }

        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        public bool Execute(string[] args)
        {
            if (args.Length != 2)
            {
                FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                return false;
            }

            Int32.TryParse(args[1], out var amount);
            string itemName = "";

            switch (args[0])
            {
                case "goldens":
                    itemName = "golden cubes";
                    GameState.SaveData.CubeShards = amount;
                    break;
                case "antis":
                    itemName = "anti-cubes";
                    GameState.SaveData.SecretCubes = amount;
                    break;
                case "bits":
                    itemName = "cube bits";
                    GameState.SaveData.CollectedParts = amount;
                    break;
                case "hearts":
                    itemName = "heart cubes";
                    GameState.SaveData.PiecesOfHeart = amount;
                    break;
                case "keys":
                    itemName = "keys";
                    GameState.SaveData.Keys = amount;
                    break;
                case "owls":
                    itemName = "owls";
                    GameState.SaveData.CollectedOwls = amount;
                    break;
                default:
                    FezapConsole.Print($"Incorrect item name: '{args[0]}'", FezapConsole.OutputType.Warning);
                    return false;
            }

            FezapConsole.Print($"Number of {itemName} has been changed to {amount}.");

            return true;
        }

        public List<string> Autocomplete(string[] args)
        {
            if(args.Length == 1)
            {
                return new string[]{ "goldens", "antis", "bits", "hearts", "keys", "owls"}.Where(s => s.StartsWith(args[0])).ToList();
            }
            else if(args.Length == 2 && args[1].Length == 0)
            {
                int value = 0;
                switch (args[0])
                {
                    case "goldens":
                        value = GameState.SaveData.CubeShards; break;
                    case "antis":
                        value = GameState.SaveData.SecretCubes; break;
                    case "bits":
                        value = GameState.SaveData.CollectedParts; break;
                    case "hearts":
                        value = GameState.SaveData.PiecesOfHeart; break;
                    case "keys":
                        value = GameState.SaveData.Keys; break;
                    case "owls":
                        value = GameState.SaveData.CollectedOwls; break;
                }
                return new List<string>() { value.ToString() };
            }
            return null;
        }
    }
}
