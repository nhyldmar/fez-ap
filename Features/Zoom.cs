using Common;
using FezEngine.Components;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame;
using FezGame.Services;
using FezGame.Structure;
using FEZAP.Features.Console;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FEZAP.Features
{
    internal class CameraZoomSet : IFezapCommand
    {
        public string Name => "zoom";
        public string HelpText => "zoom <number> - sets the camera zoom to the specified value";

		[ServiceDependency]
		public IDefaultCameraManager CameraManager { private get; set; }

		public CameraZoomSet()
        {
		}

		public bool Execute(string[] args)
        {
			if (args.Length > 1)
            {
				FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
				return false;
			}

			if(args.Length == 0)
            {
				FezapConsole.Print($"Current zoom level: {CameraManager.PixelsPerTrixel}");
				return true;
			}

			string zoomLevel = args[0];
            //Note: a zoom level of less than 0.16 will cause the sky renderer to start freaking out
			if (float.TryParse(zoomLevel, out float value) && value > 0.16f && !float.IsInfinity(value) && !float.IsNaN(value))
			{
				CameraManager.PixelsPerTrixel = value;
                FezapConsole.Print($"zoom set to {zoomLevel}");
            }
            else
            {
                FezapConsole.Print($"Invalid zoom value: {zoomLevel}");
                return false;
            }

            return true;
        }

        public List<string> Autocomplete(string[] args)
        {
            if (args.Length == 0 || args[args.Length - 1].Length > 0) return null;

            return [CameraManager.PixelsPerTrixel.ToString("0.000", CultureInfo.InvariantCulture)];
		}
    }
}
