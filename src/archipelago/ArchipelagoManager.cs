using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame.Services;
using FEZUG.Features.Console;

namespace FEZAP.Archipelago
{
    public readonly struct ConnectionInfo(string server, int port, string user, string pass = null)
    {
        public readonly string server = server;
        public readonly int port = port;
        public readonly string user = user;
        public readonly string pass = pass;
    };

    public class ArchipelagoManager
    {
        public static readonly string gameName = "Fez";
        private static ConnectionInfo connectionInfo;
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;
        private static bool connectInitFinished;

        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        public IGameService GameService { private get; set; }

        [ServiceDependency]
        public ICameraService CameraService { private get; set; }

        [ServiceDependency]
        public IDotService DotService { private get; set; }

        [ServiceDependency]
        public IOwlService OwlService { private get; set; }

        [ServiceDependency]
        public ILevelManager levelManager;

        public void Connect(string server, int port, string user, string pass = null)
        {
            connectInitFinished = false;
            connectionInfo = new(server, port, user, pass);
            session = ArchipelagoSessionFactory.CreateSession(server, port);
            LoginResult result = session.TryConnectAndLogin(gameName, user, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);

            if (result.Successful)
            {
                OnConnectSuccess();
                connectInitFinished = true;
            }
            else
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server}:{port} as {user}";
                if (pass != null)
                {
                    errorMessage += $" with password: {pass}";
                }
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }
                FezugConsole.Print(errorMessage, FezugConsole.OutputType.Error);
            }
        }

        private void OnConnectSuccess()
        {
            FezugConsole.Print("Connected successfully");
            var slotData = session.DataStorage.GetSlotData(session.ConnectionInfo.Slot);

            // Restore internal information
            Fezap.itemManager.RestoreReceivedItems();
            Fezap.locationManager.RestoreCollectedLocations();

            // Bind events
            session.MessageLog.OnMessageReceived += HandleLogMsg;
            session.Socket.ErrorReceived += HandleErrorRecv;
            session.Socket.SocketClosed += HandleSocketClosed;
            session.Items.ItemReceived += HandleRecvItem;

            // Setup goal checking
            LocationManager.goal = Convert.ToInt16(slotData["goal"]);
            // TODO: Figure out why levelManager is a null reference
            // levelManager.LevelChanged += Fezap.locationManager.MonitorGoal;

            // Disable visual pain if in slot data
            if (Convert.ToBoolean(slotData["disable_visual_pain"]))
            {
                levelManager.LevelChanged += HandleVisualPainRemoval;
            }

            // Setup deathlink if enabled
            DeathManager.deathlinkOn = Convert.ToBoolean(slotData["death_link"]);
            deathLinkService = session.CreateDeathLinkService();
            if (DeathManager.deathlinkOn)
            {
                deathLinkService.EnableDeathLink();
                deathLinkService.OnDeathLinkReceived += Fezap.deathManager.HandleDeathlink;
                FezugConsole.Print("Deathlink enabled");
            }
        }

        public static bool IsConnected()
        {
            return (session != null) && session.Socket.Connected && connectInitFinished;
        }

        private static void HandleLogMsg(LogMessage message)
        {
            switch (message)
            {
                case CountdownLogMessage:
                case ServerChatLogMessage:
                    FezugConsole.Print(message.ToString());
                    break;
                default:
                    break;
            }
        }

        private static void HandleErrorRecv(Exception e, string message)
        {
            FezugConsole.Print($"Error: {message}\n{e}", FezugConsole.OutputType.Error);
        }

        private void HandleSocketClosed(string reason)
        {
            if (reason != "")
            {
                FezugConsole.Print($"Socket closed: {reason}", FezugConsole.OutputType.Error);
                FezugConsole.Print("Attempting reconnection");
                Connect(connectionInfo.server, connectionInfo.port, connectionInfo.user, connectionInfo.pass);
            }
        }

        public static async Task SendLocation(string name)
        {
            if (IsConnected())
            {
                var id = session.Locations.GetLocationIdFromName(gameName, name);
                var result = await session.Locations.ScoutLocationsAsync(id);
                ScoutedItemInfo item = result[0];
                await session.Locations.CompleteLocationChecksAsync(id);
                FezugConsole.Print($"Sent {item.ItemDisplayName} to {item.ItemGame}");
            }
        }

        private static void HandleRecvItem(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                ItemInfo item = helper.DequeueItem();
                FezugConsole.Print($"Received {item.ItemDisplayName} from {item.ItemGame}");
                Fezap.itemManager.HandleReceivedItem(item);
            }
        }

        private void HandleVisualPainRemoval()
        {
            // TODO: Figure out how to keep rain, but remove lightning
            // TODO: Remove other sources of visual pain
            levelManager.Quantum = false;
            levelManager.Rainy = false;
            // TODO: Check if this is necessary
            // levelManager.Rebuild();
        }

        public void Update()
        {
            if (IsConnected())
            {
                // TODO: Move these over to a hook event handler
                Fezap.locationManager.MonitorLocations();
                Fezap.deathManager.MonitorDeath();
            }
        }
    }
}
