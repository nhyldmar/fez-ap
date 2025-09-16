using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame;
using FezGame.Services;
using FEZUG.Features.Console;
using Color = Microsoft.Xna.Framework.Color;

namespace FEZAP.Archipelago
{
    public class ArchipelagoManager
    {
        public static readonly string gameName = "Fez";
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;

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
                FezugConsole.Print(errorMessage, FezugConsole.OutputType.Error);
            }
        }

        private static void OnConnectSuccess()
        {
            FezugConsole.Print("Connected successfully");

            // Restore internal information
            Fezap.itemManager.RestoreReceivedItems();
            Fezap.locationManager.RestoreCollectedLocations();

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
                deathLinkService.OnDeathLinkReceived += Fezap.deathManager.HandleDeathlink;
                FezugConsole.Print("Deathlink enabled");
            }
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

        private static void HandleSocketClosed(string reason)
        {
            if (reason != "")
            {
                FezugConsole.Print($"Socket closed: {reason}", FezugConsole.OutputType.Error);
                // TODO: Reattempt connection logic with retry count
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

        public static void Update()
        {
            if (IsConnected())
            {
                Fezap.locationManager.MonitorLocations();
                Fezap.locationManager.MonitorGoal();
                Fezap.deathManager.MonitorDeath();
            }
            else
            {
                // TODO: Remove this once MenuManager works as intended.
                Connect("localhost", 38281, "Fez_Test");
            }
        }
    }
}
