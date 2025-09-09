using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using FEZAP.Features;
using FEZAP.Features.Console;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame.Services;
using Microsoft.Xna.Framework;

namespace FEZAP.Helpers
{
    public class Archipelago
    {
        public static readonly string gameName = "Fez";
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;
        public static Dictionary<string, object> slotData;
        private static List<string> EmotionalSupportMsgs = [
            " wants you to know you got this",
            " believes in you",
            " is cheering you on",
            " is rooting for you"
        ];

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
                // TODO: Find an event to bind to Gomez death
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

        /// Make Dot say a custom message
        public void DotSay(string msg, bool nearGomez, bool hideAfter)
        {
            var game_text = ServiceHelper.Get<IContentManagerProvider>().Global.Load<Dictionary<string, Dictionary<string, string>>>("Resources/GameText");
            game_text[""].Add("FEZAP_CUSTOM", msg);
            _ = DotService.Say("FEZAP_CUSTOM", nearGomez, hideAfter);
        }

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
                        CameraService.Rotate(RandomHelper.Random.Next(-2, 2));
                        break;
                    case "Sleep Trap":
                        PlayerManager.NextAction = FezGame.Structure.ActionType.IdleSleep;
                        PlayerManager.CanControl = false;
                        GameService.Wait(5);
                        PlayerManager.CanControl = true;
                        break;
                    case "Emotional Support":
                        string msg = item.Player.Name + RandomHelper.InList(EmotionalSupportMsgs);
                        DotSay(msg, true, true);
                        break;
                    default:
                        FezapConsole.Print($"Unknown item: {item.ItemDisplayName}");
                        break;
                }
            }
        }

        private static void HandleDeathlink(DeathLink deathLink)
        {
            FezapConsole.Print($"Death received: {deathLink.Cause}", FezapConsole.OutputType.Info);
            new Kill().Execute(null);
        }
    }

    internal sealed class LocationHandler : IFezapFeature
    {
        private IGomezService GomezService { get; set; }
        private IOwlService OwlService { get; set; }
        private IDotService DotService { get; set; }

        public LocationHandler()
        {
            ServiceHelper.InjectServices(this);
            // TODO: Figure out the crash from here
            // GomezService.Jumped += Test;
            // GomezService.CollectedSplitUpCube += HandleCollectBit;
            // GomezService.CollectedShard += HandleCollectCube;
            // GomezService.CollectedAnti += HandleCollectAnti;
            // GomezService.CollectedGlobalAnti += HandleCollectAnti;
            // GomezService.CollectedPieceOfHeart += HandleCollectHeart;
            // GomezService.OpenedTreasure += HandleCollectTreasure;
            // OwlService.OwlCollected += HandleCollectOwl;
        }

        public void Update(GameTime gameTime)
        {
            // TODO: Figure out the crash from here
            // if (!GomezService.Alive)
            // {
            //     // TODO: Customise Cause with PlayerManager.Action and checking ActionType
            //     var deathlink = new DeathLink(Archipelago.session.Players.ActivePlayer.Name);
            //     Archipelago.deathLinkService.SendDeathLink(deathlink);
            // }
        }
        public void Initialize() { }
        public void DrawHUD(GameTime gameTime) { }
        public void DrawLevel(GameTime gameTime) { }

        private void Test()
        {
            DotService.Say("Hello", true, true);
        }

        private void HandleCollectBit()
        {
            FezapConsole.Print("Collected cube bit");
        }

        private void HandleCollectCube()
        {
            FezapConsole.Print("Collected golden cube");
        }

        private void HandleCollectAnti()
        {
            FezapConsole.Print("Collected anti cube");
        }

        private void HandleCollectHeart()
        {
            FezapConsole.Print("Collected heart cube");
        }

        private void HandleCollectTreasure()
        {
            // TODO: Identify treasure
            FezapConsole.Print("Collected treasure");
        }

        private void HandleCollectOwl()
        {
            FezapConsole.Print("Collected owl");
        }
    }
}
