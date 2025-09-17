using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using FEZUG.Features.Console;

namespace FEZAP.Archipelago
{
    internal class Connect : IFezugCommand
    {
        public string Name => "connect";

        public string HelpText => "connect <server> <port> <slot_name> <password> - connect to server";

        public List<string> Autocomplete(string[] args)
        {
            if (args.Length == 1)
            {
                return new string[] { "archipelago.gg", "localhost" }
                .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))
                .ToList();
            }
            return null;
        }

        public bool Execute(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
            {
                FezugConsole.Print("Incorrect number of arguments", FezugConsole.OutputType.Warning);
                return false;
            }

            int port;
            try
            {
                port = int.Parse(args[1]);
            }
            catch
            {
                FezugConsole.Print($"{args[1]} is not a valid port number", FezugConsole.OutputType.Warning);
                return false;
            }

            string pass = args.Length == 4 ? args[3] : null;
            ArchipelagoManager.Connect(args[0], port, args[2], pass);

            return ArchipelagoManager.IsConnected();
        }
    }

    internal class Disconnect : IFezugCommand
    {
        public string Name => "disconnect";

        public string HelpText => "disconnect - disconnect from the server";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                ArchipelagoManager.session.Socket.DisconnectAsync();
            }
            else
            {
                FezugConsole.Print("Unable to disconnect. Not connected to a server.", FezugConsole.OutputType.Warning);
            }
            return true;
        }
    }

    internal class Received : IFezugCommand
    {
        public string Name => "received";

        public string HelpText => "received - list all received items";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                foreach (ItemInfo item in ArchipelagoManager.session.Items.AllItemsReceived)
                {
                    FezugConsole.Print(item.ItemName);
                }
            }
            else
            {
                FezugConsole.Print("Unable to check received items. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
            }
            return true;
        }
    }

    internal class Missing : IFezugCommand
    {
        public string Name => "missing";

        public string HelpText => "missing - list all missing locations";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                foreach (long locationId in ArchipelagoManager.session.Locations.AllMissingLocations)
                {
                    string locationName = ArchipelagoManager.session.Locations.GetLocationNameFromId(locationId);
                    FezugConsole.Print(locationName);
                }
            }
            else
            {
                FezugConsole.Print("Unable to check missing locations. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
            }
            return true;
        }
    }

    internal class Ready : IFezugCommand
    {
        public string Name => "ready";

        public string HelpText => "ready - send ready status to server";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                ArchipelagoManager.session.SetClientState(ArchipelagoClientState.ClientReady);
            }
            else
            {
                FezugConsole.Print("Unable set ready status. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
            }
            return true;
        }
    }

    internal class Say : IFezugCommand
    {
        public string Name => "say";

        public string HelpText => "say <message> - send message to server";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                ArchipelagoManager.session.Say(string.Join(" ", args));
            }
            else
            {
                FezugConsole.Print("Unable to send. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
            }

            return true;
        }
    }

    internal class Send : IFezugCommand
    {
        public string Name => "send";

        public string HelpText => "send <name> - send location";

        public List<string> Autocomplete(string[] args)
        {
            // TODO: Figure out if it needs to be >= 1 or if this is fine
            if (args.Length == 1)
            {
                // TODO: Figure out if this works
                return LocationData.allLocations
                    .Select(loc => loc.name.ToString())
                    .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return null;
        }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                string locationName = string.Join(" ", args);
                long locationId = ArchipelagoManager.session.Locations.GetLocationIdFromName(ArchipelagoManager.gameName, locationName);
                if (locationId == -1)
                {
                    FezugConsole.Print($"Unknown location {locationName}");
                }
                else
                {
                    ArchipelagoManager.session.Locations.CompleteLocationChecks([locationId]);
                }
            }
            else
            {
                FezugConsole.Print("Unable to send location. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
            }

            return true;
        }
    }

    internal class Release : IFezugCommand
    {
        public string Name => "release";

        public string HelpText => "release - release all remaining checks";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                var missingLocations = ArchipelagoManager.session.Locations.AllMissingLocations;
                ArchipelagoManager.session.Locations.CompleteLocationChecks([.. missingLocations]);
                // TODO: Figure out why it crashes here.
            }
            else
            {
                FezugConsole.Print("Unable to release. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
            }

            return true;
        }
    }

    internal class Deathlink : IFezugCommand
    {
        public string Name => "deathlink";

        public string HelpText => "deathlink <true/false> - enable or disable deathlink";

        public List<string> Autocomplete(string[] args)
        {
            if (args.Length == 1)
            {
                return new string[] { "true", "false" }
                .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))
                .ToList();
            }
            return null;
        }

        public bool Execute(string[] args)
        {
            if (ArchipelagoManager.IsConnected())
            {
                if (bool.Parse(args[0]))
                {
                    ArchipelagoManager.deathLinkService.EnableDeathLink();
                }
                else
                {
                    ArchipelagoManager.deathLinkService.DisableDeathLink();
                }
            }
            else
            {
                FezugConsole.Print("Unable to update deathlink flag. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
            }

            return true;
        }
    }
}