using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using FezGame.Tools;
using FEZAP.Features.Console;
using Common;

namespace FEZAP.Features
{
    internal static class Quicksaving
    {
        public static readonly string SaveDirectory = "QuickSaves";

        public static string ValidateSaveFilePathOutOfArgs(string[] args)
        {
            if (args.Length != 1)
            {
                FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                return null;
            }

            string saveFileName = args[0];

            if(saveFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                FezapConsole.Print($"Given save file name contains invalid characters.", FezapConsole.OutputType.Error);
                return null;
            }

            return Path.Combine(SaveDirectory, saveFileName);
        }

        internal class Quicksave : IFezapCommand
        {
            public string Name => "save";
            public string HelpText => "save <name> - creates a quick save file with given name";

            [ServiceDependency]
            public IGameStateManager GameState { private get; set; }

            [ServiceDependency]
            public IPlayerManager PlayerManager { private get; set; }

            public bool Execute(string[] args)
            {
                if (GameState.ActiveSaveDevice == null)
                {
                    FezapConsole.Print($"Can't save while the game is loading.", FezapConsole.OutputType.Error);
                    return false;
                }

                string saveFilePath = ValidateSaveFilePathOutOfArgs(args);
                if (saveFilePath == null) return false;

                var quicksaveDir = Path.Combine(Util.LocalConfigFolder, SaveDirectory);
                if (!Directory.Exists(quicksaveDir))
                {
                    Directory.CreateDirectory(quicksaveDir);
                }

                bool success = GameState.ActiveSaveDevice.Save(saveFilePath, delegate (BinaryWriter writer)
                {
                    var dummySave = new SaveData();
                    GameState.SaveData.CloneInto(dummySave);

                    // make sure to include current position in the save file
                    PlayerManager.RecordRespawnInformation(true);

                    if (GameState.SaveData.SinceLastSaved.HasValue)
                    {
                        GameState.SaveData.PlayTime += (DateTime.Now.Ticks - GameState.SaveData.SinceLastSaved.Value);
                    }
                    GameState.SaveData.SinceLastSaved = DateTime.Now.Ticks;
                    SaveFileOperations.Write(new CrcWriter(writer), GameState.SaveData);
                    GameState.SaveData = dummySave;
                });

                if (!success)
                {
                    FezapConsole.Print($"An error occurred when trying to create a quicksave.", FezapConsole.OutputType.Error);
                    return false;
                }

                FezapConsole.Print($"Saved quicksave \"{args[0]}\".");

                return true;
            }

            public List<string> Autocomplete(string[] args) => null;
        }



        internal class Quickload : IFezapCommand
        {
            public string Name => "load";
            public string HelpText => "load <name> - loads a quick save file with given name";

            [ServiceDependency]
            public IGameStateManager GameState { private get; set; }

            public bool Execute(string[] args)
            {
                if (args.Length == 0)
                {
                    List<string> paths = Autocomplete([""]);
                    if (paths == null || paths.Count == 0)
                    {
                        FezapConsole.Print("No available quicksaves");
                    }
                    else
                    {
                        FezapConsole.Print("List of available quicksaves:");
                        FezapConsole.Print(string.Join(" ", paths));
                    }
                    return true;
                }

                if (GameState.ActiveSaveDevice == null)
                {
                    FezapConsole.Print($"Can't load while the game is loading.", FezapConsole.OutputType.Error);
                    return false;
                }

                string saveFilePath = ValidateSaveFilePathOutOfArgs(args);
                if (saveFilePath == null) return false;

                if (!GameState.ActiveSaveDevice.FileExists(saveFilePath))
                {
                    FezapConsole.Print($"Quicksave file \"{args[0]}\" not found.", FezapConsole.OutputType.Warning);
                    return false;
                }

                bool success = GameState.ActiveSaveDevice.Load(saveFilePath, delegate (BinaryReader reader)
                {
                    GameState.SaveData = SaveFileOperations.Read(new CrcReader(reader));
                });

                if (!success)
                {
                    FezapConsole.Print($"An error occurred when trying to load a quicksave.", FezapConsole.OutputType.Error);
                    return false;
                }

                WarpLevel.Warp(GameState.SaveData.Level, WarpLevel.WarpType.SaveChange);

                FezapConsole.Print($"Loaded quicksave \"{args[0]}\".");
                return true;
            }

            public List<string> Autocomplete(string[] args)
            {
                if (GameState.ActiveSaveDevice == null) return null;

                var quicksaveDir = Path.Combine(Util.LocalConfigFolder, SaveDirectory);

                if (!Directory.Exists(quicksaveDir))
                    return null;

                return Directory.GetFiles(quicksaveDir).Select(path => Path.GetFileName(path))
                    .Where(name => name.StartsWith(args[0])).ToList();
            }
        }
    }
}
