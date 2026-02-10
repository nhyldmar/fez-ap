using Archipelago.MultiClient.Net.Enums;
using FezEngine.Services;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
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
                return [.. new string[] { "archipelago.gg", "localhost" }
                        .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))];
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
            Fezap.archipelagoManager.Connect(args[0], port, args[2], pass);

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
                CodeInputScrambler.ResetScramble();
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

        public string HelpText => "received <page> - displays given page of all received items";

        private readonly int ListPageSize = 10;

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (args.Length > 1)
            {
                FezugConsole.Print("Incorrect number of arguments", FezugConsole.OutputType.Warning);
                return false;
            }

            if (!ArchipelagoManager.IsConnected())
            {
                FezugConsole.Print("Unable to check received items. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
                return false;
            }

            int pageNumber = 1;  // If no arg, show first page
            if (args.Length != 0 && !int.TryParse(args[0], out pageNumber))
            {
                FezugConsole.Print($"Unknown argument: '{args[0]}' is not a number", FezugConsole.OutputType.Warning);
                return false;
            }

            // Get item names, then sort and group them
            var itemStrings = ArchipelagoManager.session.Items.AllItemsReceived
                                .Select(item => item.ItemName)  // Get name
                                .OrderBy(str => str)            // Alphabetise
                                .GroupBy(str => str);           // Collect multiple occurences
            int pageCount = (int)Math.Ceiling(itemStrings.Count() / (float)ListPageSize);

            if (pageNumber > pageCount)
            {
                FezugConsole.Print($"Page number {pageNumber} is not within range 1-{pageCount}", FezugConsole.OutputType.Warning);
                return false;
            }

            // Print page
            var pageStart = (pageNumber - 1) * ListPageSize;
            var pageEnd = Math.Min(itemStrings.Count(), pageNumber * ListPageSize);
            FezugConsole.Print($"=== Received - page {pageNumber}/{pageCount} ===");
            for (var i = pageStart; i < pageEnd; i++)
            {
                string toPrint = itemStrings.ElementAt(i).Key;
                int count = itemStrings.ElementAt(i).Count();
                toPrint += (count == 1) ? "" : $" (x{count})";
                FezugConsole.Print(toPrint);
            }

            return true;
        }
    }

    internal class Missing : IFezugCommand
    {
        public string Name => "missing";

        public string HelpText => "missing <page> - displays given page of all missing locations";
        private readonly int ListPageSize = 10;

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            if (args.Length > 1)
            {
                FezugConsole.Print("Incorrect number of arguments", FezugConsole.OutputType.Warning);
                return false;
            }

            if (!ArchipelagoManager.IsConnected())
            {
                FezugConsole.Print("Unable to check missing locations. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
                return false;
            }

            int pageNumber = 1;  // If no arg, show first page
            if (args.Length != 0 && !int.TryParse(args[0], out pageNumber))
            {
                FezugConsole.Print($"Unknown argument: '{args[0]}' is not a number", FezugConsole.OutputType.Warning);
                return false;
            }

            // Get item names, then sort them
            var locationStrings = ArchipelagoManager.session.Locations.AllMissingLocations
                                    .Select(locationId => ArchipelagoManager.session.Locations.GetLocationNameFromId(locationId))
                                    .OrderBy(str => str);
            int pageCount = (int)Math.Ceiling(locationStrings.Count() / (float)ListPageSize);

            if (pageCount == 0)
            {
                FezugConsole.Print("No missing locations");
                return true;
            }

            if (pageNumber > pageCount)
                {
                    FezugConsole.Print($"Page number {pageNumber} is not within range 1-{pageCount}", FezugConsole.OutputType.Warning);
                    return false;
                }

            // Print page
            var pageStart = (pageNumber - 1) * ListPageSize;
            var pageEnd = Math.Min(locationStrings.Count(), pageNumber * ListPageSize);
            FezugConsole.Print($"=== Missing - page {pageNumber}/{pageCount} ===");
            for (var i = pageStart; i < pageEnd; i++)
            {
                FezugConsole.Print(locationStrings.ElementAt(i));
            }

            return true;
        }
    }

    internal class Ready : IFezugCommand
    {
        public string Name => "ready";

        public string HelpText => "ready <true/false> - send ready status to server or remove it";

        public List<string> Autocomplete(string[] args)
        {
            if (args.Length == 1)
            {
                return [.. new string[] { "true", "false" }
                        .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))];
            }
            return null;
        }

        public bool Execute(string[] args)
        {
            if (args.Length != 1)
            {
                FezugConsole.Print("Incorrect number of arguments.", FezugConsole.OutputType.Error);
                return false;
            }

            if (!ArchipelagoManager.IsConnected())
            {
                FezugConsole.Print("Unable set ready status. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
                return false;
            }

            switch (args[0])
            {
                case "true":
                    ArchipelagoManager.session.SetClientState(ArchipelagoClientState.ClientReady);
                    return true;
                case "false":
                    ArchipelagoManager.session.SetClientState(ArchipelagoClientState.ClientUnknown);
                    return true;
                default:
                    FezugConsole.Print($"Unknown argument '{args[0]}'.");
                    return false;
            }
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
            if (args.Length == 1)
            {
                return [.. LocationData.allLocations
                        .Select(loc => loc.name.ToString())
                        .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))];
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
                    ArchipelagoManager.SendLocation(locationName);
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
                return [.. new string[] { "true", "false" }
                        .Where(s => s.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))];
            }
            return null;
        }

        public bool Execute(string[] args)
        {
            if (args.Length != 1)
            {
                FezugConsole.Print("Incorrect number of arguments.", FezugConsole.OutputType.Error);
                return false;
            }

            if (!ArchipelagoManager.IsConnected())
            {
                FezugConsole.Print("Unable to update deathlink flag. Not connected to a server. Use 'connect' command first.", FezugConsole.OutputType.Warning);
                return false;
            }

            switch (args[0])
            {
                case "true":
                    DeathManager.deathlinkOn = true;
                    ArchipelagoManager.deathLinkService.EnableDeathLink();
                    return true;
                case "false":
                    DeathManager.deathlinkOn = false;
                    ArchipelagoManager.deathLinkService.DisableDeathLink();
                    return true;
                default:
                    FezugConsole.Print($"Unknown argument '{args[0]}'.");
                    return false;
            }
        }
    }

    #if DEBUG
    internal class LevelInfo : IFezugCommand
    {
        public string Name => "levelinfo";

        public string HelpText => "levelinfo";

        public List<string> Autocomplete(string[] args) { return null; }

        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public ILevelManager LevelManager { get; set; }

        public bool Execute(string[] args)
        {
            string level = LevelManager.Name;

            List<TrileEmplacement> DestroyedTriles = GameState.SaveData.World[level].DestroyedTriles;
            List<TrileEmplacement> InactiveTriles = GameState.SaveData.World[level].InactiveTriles;
            List<int> InactiveArtObjects = GameState.SaveData.World[level].InactiveArtObjects;
            List<int> InactiveEvents = GameState.SaveData.World[level].InactiveEvents;
            List<int> InactiveGroups = GameState.SaveData.World[level].InactiveGroups;
            List<int> InactiveVolumes = GameState.SaveData.World[level].InactiveVolumes;
            List<int> InactiveNPCs = GameState.SaveData.World[level].InactiveNPCs;

            DestroyedTriles.ForEach(x => FezugConsole.Print($"Destroyed Triles: {x.X}, {x.Y}, {x.Z}"));
            InactiveTriles.ForEach(x => FezugConsole.Print($"Inactive Triles: {x.X}, {x.Y}, {x.Z}"));
            InactiveArtObjects.ForEach(x => FezugConsole.Print($"Art Objects: {x}"));
            InactiveEvents.ForEach(x => FezugConsole.Print($"Events: {x}"));
            InactiveGroups.ForEach(x => FezugConsole.Print($"Groups: {x}"));
            InactiveVolumes.ForEach(x => FezugConsole.Print($"Volumes: {x}"));
            InactiveNPCs.ForEach(x => FezugConsole.Print($"NPCs: {x}"));

            return true;
        }
    }

    internal class Debug : IFezugCommand
    {
        public string Name => "debug";

        public string HelpText => "debug";

        public List<string> Autocomplete(string[] args) { return null; }

        public bool Execute(string[] args)
        {
            // Insert function to call here
            return true;
        }
    }
    #endif  // DEBUG
}
