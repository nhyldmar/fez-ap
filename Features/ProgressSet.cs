using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FEZAP.Features.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FEZAP.Features
{
    internal class ProgressSet : IFezapCommand
    {
        public string Name => "progress";
        public string HelpText => "progress <flag/level/all> <name> <unlock/reset> - changes progress state for given flag or level.";

        [ServiceDependency]
        public IGameStateManager GameState { private get; set; }

        [ServiceDependency]
        public IGameLevelManager LevelManager { private get; set; }

        public List<string> AllowedFlagNames = new List<string>
        {
            "CanNewGamePlus", "IsNewGamePlus", "Finished32", "Finished64", "HasFPView", "HasStereo3D", "HasDoneHeartReboot",
            "FezHidden", "HasHadMapHelp", "CanOpenMap", "AchievementCheatCodeDone", "MapCheatCodeDone", "AnyCodeDeciphered",
        };

        public bool Execute(string[] args)
        {
            if (args.Length < 1 || args.Length > 3)
            {
                FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                return false;
            }

            bool isLevel = args[0] == "level";
            bool isFlag = args[0] == "flag";
            bool isAll = args[0] == "all";

            if (!isLevel && !isFlag && !isAll)
            {
                FezapConsole.Print($"Invalid first parameter: '{args[0]}'. Should be either 'flag', 'level' or 'all'.", FezapConsole.OutputType.Warning);
                return false;
            }

            if (args.Length == 1)
            {
                if (isFlag)
                {
                    FezapConsole.Print($"List of available flags:");
                    FezapConsole.Print(String.Join(", ", AllowedFlagNames));
                    return true;
                }
                else
                {
                    FezapConsole.Print($"Incorrect number of parameters.", FezapConsole.OutputType.Warning);
                    return false;
                }
            }

            string propertyName = "";
            bool reset = false;

            if (args.Length == 2) {
                if (isAll || isLevel)
                {
                    if(args[1] == "reset" || args[1] == "unlock")
                    {
                        reset = args[1] == "reset";
                        propertyName = LevelManager.Name;
                    }
                    else
                    {
                        FezapConsole.Print($"Incorrect parameter: {args[1]}.", FezapConsole.OutputType.Warning);
                        return false;
                    }
                }
                else
                {
                    if (!TryGetFlagState(args[1], out bool state))
                    {
                        FezapConsole.Print($"Incorrect flag name: {args[1]}.", FezapConsole.OutputType.Warning);
                        return false;
                    }
                    else
                    {
                        FezapConsole.Print($"Flag \"{args[1]}\" is {(state ? "unlocked" : "locked")}.", FezapConsole.OutputType.Warning);
                        return true;
                    }
                }
            }
            if(args.Length == 3)
            {
                if (isAll)
                {
                    FezapConsole.Print($"Incorrect number of parameters.", FezapConsole.OutputType.Warning);
                    return false;
                }
                if (args[2] != "reset" && args[2] != "unlock")
                {
                    FezapConsole.Print($"Invalid last parameter: '{args[0]}'. Should be either 'reset' or 'unlock'.", FezapConsole.OutputType.Warning);
                    return false;
                }
                propertyName = args[1];
                reset = args[2] == "reset";
            }

            if (isLevel)
            {
                return SetLevelState(propertyName, !reset);
            }
            else if (isFlag)
            {
                if(!SetFlagState(propertyName, !reset))
                {
                    FezapConsole.Print($"Invalid flag: {propertyName}!", FezapConsole.OutputType.Warning);
                    return false;
                }
                else
                {
                    FezapConsole.Print($"Flag {propertyName} has been {(reset ? "reset" : "unlocked")}!");
                    return true;
                }

            }
            else if(isAll)
            {
                // unlock all
                SetEveryLevelState(!reset);
                SetEveryFlagState(!reset);

                FezapConsole.Print($"Everything has been {(reset ? "reset" : "unlocked")}!");
            }

            return false;
        }

        private void UnlockLevel(string levelName)
        {
            if (!MemoryContentManager.AssetExists("Levels\\" + levelName.Replace('/', '\\')))
            {
                FezapConsole.Print($"Level with name '{levelName}' does not exist.", FezapConsole.OutputType.Warning);
                return;
            }

        }

        private bool SetLevelState(string levelName, bool unlock)
        {
            if (!MemoryContentManager.AssetExists("Levels\\" + levelName.Replace('/', '\\')))
            {
                FezapConsole.Print($"Level with name '{levelName}' does not exist.", FezapConsole.OutputType.Warning);
                return false;
            }

            if (!unlock)
            {
                GameState.SaveData.World.Remove(LevelManager.Name);
                FezapConsole.Print($"Progress in level '{levelName}' has been reset.");
            }
            else
            {
                FezapConsole.Print($"Unlocking levels hasn't been fully implemented yet.",  FezapConsole.OutputType.Warning);
                UnlockLevel(levelName);
            }
            return true;
        }

        private bool SetFlagState(string flagName, bool unlocked)
        {
            if (AllowedFlagNames.Where(s=>s.Equals(flagName,StringComparison.OrdinalIgnoreCase)).Count() == 0)
            {
                return false;
            }

            var flagField = GameState.SaveData.GetType().GetField(flagName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            flagField.SetValue(GameState.SaveData, unlocked);

            return true;
        }

        private bool TryGetFlagState(string flagName, out bool state)
        {
            state = false;
            if (AllowedFlagNames.Where(s => s.Equals(flagName, StringComparison.OrdinalIgnoreCase)).Count() == 0)
            {
                return false;
            }
            var flagField = GameState.SaveData.GetType().GetField(flagName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            state = (bool)flagField.GetValue(GameState.SaveData);
            return true;
        }

        private void SetEveryLevelState(bool unlocked)
        {
            if (!unlocked)
            {
                GameState.SaveData.World.Clear();
            }
            else
            {
                foreach (var levelName in WarpLevel.LevelList)
                {
                    UnlockLevel(levelName);
                }
            }
        }

        private void SetEveryFlagState(bool unlocked)
        {
            foreach(var flag in AllowedFlagNames)
            {
                SetFlagState(flag, unlocked);
            }
        }


        public List<string> Autocomplete(string[] args)
        {
            if (args.Length == 1)
            {
                return new string[] { "flag", "level", "all" }.Where(s => s.StartsWith(args[0])).ToList();
            }
            else if (args.Length == 3 || args[0] == "all")
            {
                return new string[] { "unlock", "reset" }.Where(s => s.StartsWith(args[args.Length-1])).ToList();
            }
            else if (args.Length == 2)
            {
                if (args[0] == "level")
                {
                    var list = WarpLevel.Instance.Autocomplete(new string[]{ args[1] });
                    list.AddRange(
                        new string[] { "unlock", "reset" }.Where(s => s.StartsWith(args[1])).ToList()
                    );
                    return list;
                }
                else if (args[0] == "flag")
                {
                    return AllowedFlagNames.Where(s => s.StartsWith(args[1])).ToList();
                }
            }

            return null;
        }
    }
}
