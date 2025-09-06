using Common;
using FezEngine.Components;
using FezEngine.Tools;
using FEZAP.Features.Console;
using FEZAP.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZAP.Features
{
    using BindList = Dictionary<Keys, string>;

    internal class BindingSystem : IFezapFeature
    {
        public const string BindConfigFileName = "FezapBinds";

        public BindList Binds { get; private set; }

        public static BindingSystem Instance;

        [ServiceDependency]
        public IInputManager InputManager { get; set; }

        public BindingSystem()
        {
            Instance = this;
            Binds = new BindList();
        }

        public void Initialize() {
            LoadBinds();
        }

        public void Update(GameTime gameTime)
        {

            foreach (var bindPair in Binds)
            {
                if (!bindPair.Value.Trim().ToLower().Equals("toggleconsole") && !((InputManager)InputManager).Enabled) return;
                if (InputHelper.IsKeyPressed(bindPair.Key))
                {
                    FezapConsole.ExecuteCommand(bindPair.Value);
                }
            }
        }

        public void DrawHUD(GameTime gameTime) { }

        public void DrawLevel(GameTime gameTime) { }

        public static bool HasBind(Keys key)
        {
            return Instance.Binds.ContainsKey(key);
        }

        public static string GetBind(Keys key)
        {
            if (HasBind(key)) return Instance.Binds[key];
            else return "";
        }

        public static void SetBind(Keys key, string command)
        {
            if (command.Length == 0 || Instance.Binds.ContainsKey(key))
                Instance.Binds.Remove(key);
            if (command.Length > 0)
                Instance.Binds.Add(key, command);


            string configPath = Path.Combine(Util.LocalConfigFolder, BindConfigFileName);
            SaveBinds();
        }

        private static string GetBindsFilePath()
        {
            return Path.Combine(Util.LocalConfigFolder, BindConfigFileName);
        }

        private static void SaveBinds()
        {
            using (StreamWriter bindFile = new StreamWriter(GetBindsFilePath()))
            {
                foreach(var bind in Instance.Binds)
                {
                    bindFile.WriteLine($"{bind.Key} {bind.Value}");
                }
            }
        }

        private static void LoadBinds()
        {
            var bindFilePath = GetBindsFilePath();
            if (!File.Exists(bindFilePath)) return;
            var bindFileLines = File.ReadAllLines(bindFilePath);
            foreach(var line in bindFileLines)
            {
                string[] tokens = line.Split(new char[] { ' ' }, 2);
                if (tokens.Length < 2) continue;

                if(Enum.TryParse(tokens[0], out Keys key))
                {
                    Instance.Binds.Add(key, tokens[1]);
                }
            }
        }

        internal class BindCommand : IFezapCommand
        {
            public string Name => "bind";
            public string HelpText => "bind <key> <command> - binds a command to specified keyboard key";

            public List<string> Autocomplete(string[] args)
            {
                if (args.Length == 1)
                {
                    return Enum.GetNames(typeof(Keys)).Select(s=>s.ToLower()).Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (args.Length == 2 && Enum.TryParse<Keys>(args[0], true, out var key)
                && HasBind(key) && GetBind(key).StartsWith(args[1], StringComparison.OrdinalIgnoreCase))
                {
                    return new List<string> { GetBind(key) };
                }
                return null;
            }

            public bool Execute(string[] args)
            {
                if (args.Length < 1 || args.Length > 2)
                {
                    FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                    return false;
                }

                if (!Enum.TryParse<Keys>(args[0], true, out var key))
                {
                    FezapConsole.Print($"Invalid key: {args[0]}.", FezapConsole.OutputType.Warning);
                    return false;
                }

                if (args.Length == 1)
                {
                    if (HasBind(key))
                    {
                        FezapConsole.Print($"Key {key} is bound to command \"{ GetBind(key)}\".");
                    }
                    else
                    {
                        FezapConsole.Print($"No command has been bound to key {key}.");
                    }
                }

                if (args.Length == 2)
                {
                    SetBind(key, args[1]);
                    if (args[1].Length == 0)
                        FezapConsole.Print($"Key {key} has been unbound.");
                    else
                        FezapConsole.Print($"Command has been bound to key {key}.");
                }

                return true;
            }
        }

        internal class UnbindCommand : IFezapCommand
        {
            public string Name => "unbind";
            public string HelpText => "unbind <key> - unbinds specified keyboard key";

            public List<string> Autocomplete(string[] args)
            {
                if (args.Length == 1)
                {
                    return Enum.GetNames(typeof(Keys)).Select(s => s.ToLower()).Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase)).ToList();
                }

                return null;
            }

            public bool Execute(string[] args)
            {
                if (args.Length != 1)
                {
                    FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                    return false;
                }

                if (!Enum.TryParse<Keys>(args[0], true, out var key))
                {
                    FezapConsole.Print($"Invalid key: {args[0]}.", FezapConsole.OutputType.Warning);
                    return false;
                }

                if (HasBind(key))
                {
                    SetBind(key, "");
                    FezapConsole.Print($"Key {key} has been unbound.");
                }
                else
                {
                    FezapConsole.Print($"No command has been bound to key {key}.");
                }

                return true;
            }
        }
    }
}
