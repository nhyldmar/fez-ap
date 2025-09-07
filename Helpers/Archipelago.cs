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

namespace FEZAP.Helpers
{
    public class Archipelago
    {
        public static readonly string gameName = "Fez";
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;
        public static Dictionary<string, object> slotData;

        public void Connect(string server, int port, string user, string pass)
        {
            session = ArchipelagoSessionFactory.CreateSession(server, port);

            LoginResult result = session.TryConnectAndLogin(gameName, user, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);

            if (result.Successful)
            {
                FezapConsole.Print("Successfully connected to AP server.", FezapConsole.OutputType.Info);

                // Get slot data
                slotData = session.DataStorage.GetSlotData(session.ConnectionInfo.Slot);

                // Bind events
                session.MessageLog.OnMessageReceived += HandleLogMsg;
                session.Socket.ErrorReceived += HandleErrorRecv;
                session.Socket.SocketClosed += HandleSocketClosed;
                session.Items.ItemReceived += HandleRecvItem;

                // Handle deathlink
                deathLinkService = session.CreateDeathLinkService();
                deathLinkService.OnDeathLinkReceived += HandleDeathlink;
                if ((bool)slotData["death_link"])
                {
                    deathLinkService.EnableDeathLink();
                }
            }
            else
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server}:{port} as {user}";
                if (pass != null)
                {
                    errorMessage += $"with password: {pass}";
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
            var id = session.Locations.GetLocationIdFromName(gameName, name);
            var result = await session.Locations.ScoutLocationsAsync(id);
            ScoutedItemInfo item = result[0];
            await session.Locations.CompleteLocationChecksAsync(id);
            FezapConsole.Print($"Sent {item.ItemDisplayName} to {item.ItemGame}", FezapConsole.OutputType.Info);
        }

        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public ICameraService CameraService { private get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        private void HandleRecvItem(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                ItemInfo item = helper.DequeueItem();
                FezapConsole.Print($"Received {item.ItemDisplayName} from {item.ItemGame}", FezapConsole.OutputType.Info);
                switch (item.ItemName)
                {
                    case "Golden Cube":
                        GameState.SaveData.CubeShards += 1;
                        break;
                    case "Anti-Cube":
                        GameState.SaveData.SecretCubes += 1;
                        break;
                    case "Key":
                        GameState.SaveData.Keys += 1;
                        break;
                    case "Owl":
                        GameState.SaveData.CollectedOwls += 1;
                        break;
                    case "Red Map":
                        GameState.SaveData.Maps.Add("Red Map");
                        break;
                    case "Purple Map":
                        GameState.SaveData.Maps.Add("Purple Map");
                        break;
                    case "Tower Map":
                        GameState.SaveData.Maps.Add("Tower Map");
                        break;
                    case "QR Code Map":
                        GameState.SaveData.Maps.Add("QR Code Map");
                        break;
                    case "Burned Map":
                        GameState.SaveData.Maps.Add("Burned Map");
                        break;
                    case "Cemetery Map 1":
                        GameState.SaveData.Maps.Add("Cemetery Map 1");
                        break;
                    case "Cemetery Map 2":
                        GameState.SaveData.Maps.Add("Cemetery Map 2");
                        break;
                    case "Cemetery Map 3":
                        GameState.SaveData.Maps.Add("Cemetery Map 3");
                        break;
                    case "Cemetery Map 4":
                        GameState.SaveData.Maps.Add("Cemetery Map 4");
                        break;
                    case "The Writing Cube":
                        GameState.SaveData.Artifacts.Add(FezEngine.Structure.ActorType.LetterCube);
                        break;
                    case "The Counting Cube":
                        GameState.SaveData.Artifacts.Add(FezEngine.Structure.ActorType.NumberCube);
                        break;
                    case "The Tome Artifact":
                        GameState.SaveData.Artifacts.Add(FezEngine.Structure.ActorType.Tome);
                        break;
                    case "The Skull Artifact":
                        GameState.SaveData.Artifacts.Add(FezEngine.Structure.ActorType.TriSkull);
                        break;
                    case "Heart Cube":
                        GameState.SaveData.PiecesOfHeart += 1;
                        break;
                    case "Rotation Trap":
                        // TODO: Move to some central random source
                        Random random = new();
                        CameraService.Rotate(random.Next(-2, 2));
                        break;
                    case "Sleep Trap":
                        PlayerManager.NextAction = FezGame.Structure.ActionType.IdleSleep;
                        PlayerManager.CanControl = false;
                        // TODO: Wait a few seconds in a non-blocking way
                        PlayerManager.CanControl = true;
                        break;
                    case "Emotional Support":
                        break;
                    default:
                        FezapConsole.Print($"Unknown item: {item.ItemDisplayName}");
                        break;
                }
            }
        }

        private static void HandleDeathlink(DeathLink deathLink)
        {
            FezapConsole.Print(deathLink.Cause, FezapConsole.OutputType.Info);
            // TODO: Figure out a nicer way than creating an instance here.
            new Kill().Execute(null);
        }
    }
}
