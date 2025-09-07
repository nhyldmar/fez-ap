using FEZAP.Features.Console;

namespace FEZAP.Features
{
    internal class Connect : IFezapCommand
    {
        public string Name => "connect";

        public string HelpText => "connect <server> <port> <slot_name> <password> - connect to server";

        public List<string> Autocomplete(string[] args) { return null; }

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

    internal class Disconnect : IFezapCommand
    {
        public string Name => "disconnect";

        public string HelpText => "disconnect - disconnect from the server";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            // TODO: Figure out a nicer way
            Helpers.Archipelago.session = null;
            return true;
        }
    }

    internal class Received : IFezapCommand
    {
        public string Name => "received";

        public string HelpText => "received - list all received items";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (Helpers.Archipelago.session.Socket.Connected)
            {
                FezapConsole.Print(Helpers.Archipelago.session.Items.AllItemsReceived.ToString());
            }
            else
            {
                FezapConsole.Print("Unable to check received items. Not connected to a server. Use 'connect' command first.");
            }
            return true;
        }
    }

    internal class Missing : IFezapCommand
    {
        public string Name => "missing";

        public string HelpText => "missing - list all missing locations";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (Helpers.Archipelago.IsConnected())
            {
                FezapConsole.Print(Helpers.Archipelago.session.Locations.AllMissingLocations.ToString());
            }
            else
            {
                FezapConsole.Print("Unable to check missing locations. Not connected to a server. Use 'connect' command first.");
            }
            return true;
        }
    }

    internal class Ready : IFezapCommand
    {
        public string Name => "ready";

        public string HelpText => "ready - send ready status to server";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (Helpers.Archipelago.IsConnected())
            {
                Helpers.Archipelago.session.SetClientState(Archipelago.MultiClient.Net.Enums.ArchipelagoClientState.ClientReady);
            }
            else
            {
                FezapConsole.Print("Unable set ready status. Not connected to a server. Use 'connect' command first.");
            }
            return true;
        }
    }

    internal class Say : IFezapCommand
    {
        public string Name => "say";

        public string HelpText => "say <message> - send message to server";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (Helpers.Archipelago.IsConnected())
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

    internal class Send : IFezapCommand
    {
        public string Name => "send";

        public string HelpText => "send <name> - send location";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (Helpers.Archipelago.IsConnected())
            {
                string locationName = string.Join(" ", args);
                long locationId = Helpers.Archipelago.session.Locations.GetLocationIdFromName(Helpers.Archipelago.gameName, locationName);
                if (locationId == -1)
                {
                    FezapConsole.Print($"Unknown location {locationName}");
                }
                else
                {
                    Helpers.Archipelago.session.Locations.CompleteLocationChecks([locationId]);
                }
            }
            else
            {
                FezapConsole.Print("Unable to release. Not connected to a server. Use 'connect' command first.");
            }

            return true;
        }
    }

    internal class Release : IFezapCommand
    {
        public string Name => "release";

        public string HelpText => "release - release all remaining checks";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (Helpers.Archipelago.IsConnected())
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

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (Helpers.Archipelago.IsConnected())
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
