using FEZAP.Features.Console;

namespace FEZAP.Features
{
    internal class Connect : IFezapCommand
    {
        public string Name => "connect";

        public string HelpText => "connect <server> <port> <slot_name> <password> - connect to server";

        public List<string> Autocomplete(string[] args)
        {
            return null;
        }

        public bool Execute(string[] args)
        {
            // TODO: Make robust to int parsing and also avoid need for switch

            switch (args.Length)
            {
                case 3:
                    Helpers.Archipelago.Connect(args[0], int.Parse(args[1]), args[2], null);
                    break;
                case 4:
                    Helpers.Archipelago.Connect(args[0], int.Parse(args[1]), args[2], args[3]);
                    break;
                default:
                    FezapConsole.Print("Incorrect number of arguments", FezapConsole.OutputType.Warning);
                    break;
            }

            return true;
        }
    }

    internal class Say : IFezapCommand
    {
        public string Name => "say";

        public string HelpText => "say <message> - send message to server";

        public List<string> Autocomplete(string[] args)
        {
            return null;
        }

        public bool Execute(string[] args)
        {
            // TODO: Fix Socket.Connected being true even before session has been connected.
            if (Helpers.Archipelago.session.Socket.Connected)
            {
                Helpers.Archipelago.session.Say(string.Join(" ", args));
            }
            else
            {
                FezapConsole.Print("Unable to send. Not connected to a server. Use 'connect' command first.");
            }

            return true;
        }
    }

    internal class Release : IFezapCommand
    {
        public string Name => "release";

        public string HelpText => "release - release all remaining checks";

        public List<string> Autocomplete(string[] args)
        {
            return null;
        }

        public bool Execute(string[] args)
        {
            // TODO: Fix Socket.Connected being true even before session has been connected.
            if (Helpers.Archipelago.session.Socket.Connected)
            {
                var missingLocations = Helpers.Archipelago.session.Locations.AllMissingLocations;
                Helpers.Archipelago.session.Locations.CompleteLocationChecks([.. missingLocations]);
                // TODO: Figure out why it crashes here.
            }
            else
            {
                FezapConsole.Print("Unable to release. Not connected to a server. Use 'connect' command first.");
            }

            return true;
        }
    }

    internal class Deathlink : IFezapCommand
    {
        public string Name => "deathlink";

        public string HelpText => "deathlink <true/false> - enable or disable deathlink";

        public List<string> Autocomplete(string[] args)
        {
            return null;
        }

        public bool Execute(string[] args)
        {
            // TODO: Fix Socket.Connected being true even before session has been connected.
            if (Helpers.Archipelago.session.Socket.Connected)
            {
                if (bool.Parse(args[0]))
                {
                    Helpers.Archipelago.deathLinkService.EnableDeathLink();
                }
                else
                {
                    Helpers.Archipelago.deathLinkService.DisableDeathLink();
                }
            }
            else
            {
                FezapConsole.Print("Unable to release. Not connected to a server. Use 'connect' command first.");
            }

            return true;
        }
    }
}
