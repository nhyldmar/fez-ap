using FezEngine.Services;
using FezEngine.Tools;
using FezGame.Services;
using FEZAP.Features.Console;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZAP.Features
{
    internal class Teleport : IFezapCommand
    {
        public string Name => "tp";

        public string HelpText => "tp <x> <y> <z> - teleports Gomez to given coordinates";

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        public IDefaultCameraManager CameraManager { private get; set; }

        public List<string> Autocomplete(string[] args)
        {
            if (args.Length == 0 || args[args.Length - 1].Length > 0) return null;

            var pos = PlayerManager.Position;
            float value = 0.0f;
            switch (args.Length)
            {
                case 1: value = pos.X; break;
                case 2: value = pos.Y; break;
                case 3: value = pos.Z; break;
                default: return null;
            }

            return new List<string> { value.ToString("0.000", CultureInfo.InvariantCulture) };
        }

        public bool Execute(string[] args)
        {
            if (args.Length != 3)
            {
                FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                return false;
            }

            if(!float.TryParse(args[0], NumberStyles.Number, CultureInfo.InvariantCulture, out float x))
            {
                FezapConsole.Print($"Incorrect coordinate: '{args[0]}'", FezapConsole.OutputType.Warning);
                return false;
            }
            if (!float.TryParse(args[1], NumberStyles.Number, CultureInfo.InvariantCulture, out float y))
            {
                FezapConsole.Print($"Incorrect coordinate: '{args[1]}'", FezapConsole.OutputType.Warning);
                return false;
            }
            if (!float.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out float z))
            {
                FezapConsole.Print($"Incorrect coordinate: '{args[2]}'", FezapConsole.OutputType.Warning);
                return false;
            }

            PlayerManager.Position = new Vector3(x,y,z);
            FezapConsole.Print($"Player teleported to coordinates: (X:{x}, Y:{y}, Z:{z})");
            return true;
        }
    }
}
