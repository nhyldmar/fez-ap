using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame.Services;
using Color = Microsoft.Xna.Framework.Color;

namespace FEZAP.Archipelago
{
    public class ArchipelagoManager
    {
        public static readonly string gameName = "Fez";
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;
        private static readonly DeathManager deathManager = new();
        private static readonly ItemManager itemManager = new();
        private static readonly LocationManager locationManager = new();

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

        public static void Connect(string server, int port, string user, string pass = null)
        {
            session = ArchipelagoSessionFactory.CreateSession(server, port);

            LoginResult result = session.TryConnectAndLogin(gameName, user, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);

            if (result.Successful)
            {
                OnConnectSuccess();
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
                HudManager.Print(errorMessage, Color.Red);
            }
        }

        private static void OnConnectSuccess()
        {
            // Restore internal information
            // TODO: Uncomment once no issues created
            // itemManager.RestoreReceivedItems();
            // locationManager.RestoreCollectedLocations();

            // Bind events
            session.MessageLog.OnMessageReceived += HandleLogMsg;
            session.Socket.ErrorReceived += HandleErrorRecv;
            session.Socket.SocketClosed += HandleSocketClosed;
            session.Items.ItemReceived += HandleRecvItem;

            // Get slot data and restore item info
            var slotData = session.DataStorage.GetSlotData(session.ConnectionInfo.Slot);
            LocationManager.goal = Convert.ToInt16(slotData["goal"]);
            DeathManager.deathlinkOn = Convert.ToBoolean(slotData["death_link"]);

            // Setup deathlink
            deathLinkService = session.CreateDeathLinkService();
            if (DeathManager.deathlinkOn)
            {
                deathLinkService.EnableDeathLink();
                deathLinkService.OnDeathLinkReceived += deathManager.HandleDeathlink;
            }

            // Display in UI that session is connected
            HudManager.isConnected = true;
        }

        public static bool IsConnected()
        {
            if (session == null)
            {
                return false;
            }
            else
            {
                return session.Socket.Connected;
            }
        }

        private static void HandleLogMsg(LogMessage message)
        {
            switch (message)
            {
                case CountdownLogMessage:
                case ServerChatLogMessage:
                    HudManager.Print(message.ToString());
                    break;
                default:
                    break;
            }
        }

        private static void HandleErrorRecv(Exception e, string message)
        {
            HudManager.Print($"Error: {message}\n{e}", Color.Red);
        }

        private static void HandleSocketClosed(string reason)
        {
            if (reason != "")
            {
                HudManager.Print($"Socket closed: {reason}", Color.Red);
                // TODO: Reattempt connection logic with retry count
                HudManager.isConnected = false;
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
                HudManager.Print($"Sent {item.ItemDisplayName} to {item.ItemGame}");
            }
        }

        private static void HandleRecvItem(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                ItemInfo item = helper.DequeueItem();
                HudManager.Print($"Received {item.ItemDisplayName} from {item.ItemGame}");
                itemManager.HandleReceivedItem(item);
            }
        }

        public static void Update()
        {
            if (IsConnected())
            {
                // TODO: Uncomment once no issues created
                // locationManager.MonitorLocations();
                // locationManager.MonitorGoal();
                // deathManager.MonitorDeath();
            }
            else
            {
                // TODO: Remove this once MenuManager works as intended.
                Connect("localhost", 38281, "Fez_Test");
            }
        }
    }
}
