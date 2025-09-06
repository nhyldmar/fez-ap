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
    internal class Allow3DRotation : IFezapCommand
    {
        public string Name => "allow3d";
        public string HelpText => "allow3d - allows the player to use 3d rotation";

        public List<string> Autocomplete(string[] args) => null;

        [ServiceDependency]
        public IGameStateManager GameState { private get; set; }

        [ServiceDependency]
        public IGameLevelManager LevelManager { private get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        public bool Execute(string[] args)
        {
            GameState.DisallowRotation = false;
            LevelManager.Flat = false;
            PlayerManager.CanRotate = true;

            FezapConsole.Print("Open your eyes...");

            return true;
        }
    }
}
