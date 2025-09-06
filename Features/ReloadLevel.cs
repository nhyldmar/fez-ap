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
    internal class ReloadLevel : IFezapCommand
    {
        public string Name => "reload";
        public string HelpText => "reload - reloads current map. If \"fresh\" flag is set, resets all collectibles as well.";

        public List<string> Autocomplete(string[] args) => null;

        [ServiceDependency]
        public IGameLevelManager LevelManager { private get; set; }

        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        public bool Execute(string[] args)
        {
            if (args.Length > 0)
            {
                FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                return false;
            }

            WarpLevel.Warp(LevelManager.Name);

            FezapConsole.Print($"Current level ({LevelManager.Name}) has been reloaded.");

            return true;
        }
    }
}
