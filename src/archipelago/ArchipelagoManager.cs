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
using FEZUG.Features;
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
        public ILevelManager LevelManager { private get; set; }

        public void Connect(string server, int port, string user, string pass = null)
        {
            if (!IsSaveLoaded())
            {
                FezugConsole.Print("Select a save before connecting.", FezugConsole.OutputType.Error);
                return;
            }

            connectInitFinished = false;
            connectionInfo = new(server, port, user, pass);
            session = ArchipelagoSessionFactory.CreateSession(server, port);
            LoginResult result = session.TryConnectAndLogin(gameName, user, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);

            if (result.Successful)
            {
                OnConnectSuccess();
                connectInitFinished = true;
                return;
            }
            else
            {
                OnConnectFailed(result);
            }
        }

        private bool IsSaveLoaded()
        {
            // NOTE: This flags when you have selected a slot, but does not check if you've loaded into the slot yet.
            int slot = GameState?.SaveSlot ?? -1;
            return slot >= 0;
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
            LevelManager.LevelChanged += Fezap.locationManager.MonitorGoal;
            string goalStr = LocationManager.goal == 0 ? "32 Cube Ending" : "64 Cube Ending";
            FezugConsole.Print($"Goal set to {goalStr}");

            // Disable visual pain if in options
            if (Convert.ToBoolean(slotData["disable_visual_pain"]))
            {
                LevelManager.LevelChanging += HandleVisualPainRemoval;
                FezugConsole.Print("Visual pain disabled");
            }

            // Setup deathlink if enabled
            DeathManager.deathlinkOn = Convert.ToBoolean(slotData["death_link"]);
            deathLinkService = session.CreateDeathLinkService();
            deathLinkService.OnDeathLinkReceived += Fezap.deathManager.HandleDeathlink;
            if (DeathManager.deathlinkOn)
            {
                deathLinkService.EnableDeathLink();
                FezugConsole.Print("Deathlink enabled");
            }
        }

        private void OnConnectFailed(LoginResult result)
        {
            LoginFailure failure = (LoginFailure)result;
            string errorMessage = $"Failed to Connect to {connectionInfo.server}:{connectionInfo.port} as {connectionInfo.user}";
            if (connectionInfo.pass != null)
            {
                errorMessage += $" with password: {connectionInfo.pass}";
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
                case HintItemSendLogMessage:
                    var hintMsg = (HintItemSendLogMessage)message;
                    if (hintMsg.IsRelatedToActivePlayer)
                    {
                        FezugConsole.Print(hintMsg.ToString());
                    }
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

        public static void SendLocation(string name)
        {
            if (IsConnected())
            {
                // Ask server if it knows the location
                var id = session.Locations.GetLocationIdFromName(gameName, name);
                if (id == -1)
                {
                    FezugConsole.Print($"Server does not know location '{name}'. Check that the correct apworld was used to generate.");
                    return;
                }

                // Send location
                session.Locations.CompleteLocationChecks([id]);

                // Get location info
                var result = session.Locations.ScoutLocationsAsync(false, [id]);
                ScoutedItemInfo item = result.Result[id];
                if (item.Player.Name == connectionInfo.user)
                {
                    FezugConsole.Print($"Found your own {item.ItemName} ({item.LocationName})");
                }
                else
                {
                    FezugConsole.Print($"Sent {item.ItemName} to {item.Player.Alias} ({item.LocationName})");
                }
            }
        }

        private static void HandleRecvItem(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                ItemInfo item = helper.DequeueItem();
                if (item.Player.Name != connectionInfo.user)
                {
                    FezugConsole.Print($"Received {item.ItemDisplayName} from {item.Player.Alias} ({item.LocationName})");
                }
                Fezap.itemManager.HandleReceivedItem(item);
            }
        }

        private void HandleVisualPainRemoval()
        {
            // Remove quantum effect
            if (LevelManager.Quantum)
            {
                LevelManager.Quantum = false;
            }

            // Remove lightning flashes and make invisible triles visible
            InvisibleTrilesDraw.WireframesEnabled = LevelManager.Rainy;
            if (LevelManager.Rainy)
            {
                LevelManager.Rainy = false;
            }

            // NOTE: Remove other sources of visual pain as requested
        }

        private static int updateCounter = 0;  // Used to reduce the frequency of the update counter for performance
        public void Update()
        {
            if (updateCounter++ > 10 && IsConnected())
            {
                updateCounter = 0;
                Fezap.locationManager.MonitorCollectibles();
                Fezap.locationManager.MonitorLocations();
                Fezap.deathManager.MonitorDeath();
            }
        }
    }
}
