using FezEngine.Components;
using FezEngine.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZAP.Features.Console
{
    internal class ConsoleToggle : IFezapCommand
    {
        public string Name => "toggleconsole";
        public string HelpText => "toggleconsole - toggles displaying the console";

        public List<string> Autocomplete(string[] args) => null;

        public bool Execute(string[] args)
        {
            FezapConsole.CommandHandler handler = FezapConsole.Instance.Handler;
            handler.Enabled = !handler.Enabled;

            InputManager im = (InputManager)handler.InputManager;
            im.Enabled = !handler.Enabled;

            return true;
        }
    }
}
