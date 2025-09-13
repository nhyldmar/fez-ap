using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using FEZAP.Features;
using FEZAP.Features.Console;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame.Services;
using Microsoft.Xna.Framework;

namespace FEZAP.Helpers
{
    public class ArchipelagoManager : IFezapFeature
    {
        public static readonly string gameName = "Fez";
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;
        private static Dictionary<string, object> slotData;
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
                FezapConsole.Print(errorMessage, FezapConsole.OutputType.Error);
            }
        }

        private static void OnConnectSuccess()
        {
            FezapConsole.Print("Successfully connected to AP server.", FezapConsole.OutputType.Info);

            // Restore internal information
            itemManager.RestoreReceivedItems();
            locationManager.RestoreCollectedLocations();

            // Get slot data and restore item info
            slotData = session.DataStorage.GetSlotData(session.ConnectionInfo.Slot);

            // Bind events
            session.MessageLog.OnMessageReceived += HandleLogMsg;
            session.Socket.ErrorReceived += HandleErrorRecv;
            session.Socket.SocketClosed += HandleSocketClosed;
            session.Items.ItemReceived += HandleRecvItem;

            // Bind deathlink
            deathLinkService = session.CreateDeathLinkService();
            deathLinkService.OnDeathLinkReceived += deathManager.HandleDeathlink;
            if ((bool)slotData["death_link"])
            {
                deathLinkService.EnableDeathLink();
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
                    FezapConsole.Print(message.ToString());
                    break;
                default:
                    break;
            }
        }

        private static void HandleErrorRecv(Exception e, string message)
        {
            FezapConsole.Print($"Error: {message}\n{e}", FezapConsole.OutputType.Error);
        }

        private static void HandleSocketClosed(string reason)
        {
            if (reason != "")
            {
                FezapConsole.Print($"Socket closed: {reason}");
                // TODO: Reattempt connection logic with retry count
            }
        }

        public static async Task SendLocation(string name)
        {
            FezapConsole.Print($"Collected {name}");
            if (IsConnected())
            {
                var id = session.Locations.GetLocationIdFromName(gameName, name);
                var result = await session.Locations.ScoutLocationsAsync(id);
                ScoutedItemInfo item = result[0];
                await session.Locations.CompleteLocationChecksAsync(id);
                FezapConsole.Print($"Sent {item.ItemDisplayName} to {item.ItemGame}", FezapConsole.OutputType.Info);
            }
        }

        private static void HandleRecvItem(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                ItemInfo item = helper.DequeueItem();
                FezapConsole.Print($"Received {item.ItemDisplayName} from {item.ItemGame}", FezapConsole.OutputType.Info);
                itemManager.HandleReceivedItem(item);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsConnected())
            {
                deathManager.MonitorDeath();
                locationManager.HandleLocationChecking();
                MonitorGoal();
            }
        }

        private void MonitorGoal()
        {
            if ((string)slotData["goal"] == "32 Cubes")
            {
                if (GameState.SaveData.Finished32)
                {
                    FezapConsole.Print("Goal achieved");
                    session.SetGoalAchieved();
                }
            }
            else if ((string)slotData["goal"] == "64 Cubes")
            {
                if (GameState.SaveData.Finished64)
                {
                    FezapConsole.Print("Goal achieved");
                    session.SetGoalAchieved();
                }
            }
        }

        public void Initialize() { }
        public void DrawHUD(GameTime gameTime) { }
        public void DrawLevel(GameTime gameTime) { }
    }
}
