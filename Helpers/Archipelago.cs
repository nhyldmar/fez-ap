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
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using FezGame.Tools;
using Microsoft.Xna.Framework;

namespace FEZAP.Helpers
{
    public struct ItemSaveData
    {
        public int bits;
        public int cubes;
        public int antis;
        public int hearts;
        public int keys;
        public int owls;
        public List<string> maps;
        public List<ActorType> artifacts;
    }

    public class Archipelago : IFezapFeature
    {
        public static readonly string gameName = "Fez";
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;
        private static Dictionary<string, object> slotData;
        private static readonly List<string> EmotionalSupportMsgs = [
            " wants you to know you got this",
            " believes in you",
            " is cheering you on",
            " is rooting for you"
        ];
        private static bool sentDeath;
        private static ItemSaveData receivedItems = new();
        private static ItemSaveData collectedItems = new();
        private static Dictionary<string, Dictionary<string, string>> gameText = ServiceHelper.Get<IContentManagerProvider>().Global.Load<Dictionary<string, Dictionary<string, string>>>("Resources/GameText");

        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        private IGomezService GomezService { get; set; }

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

                RestoreItemInfo();

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
            FezapConsole.Print($"Collected {name}");
            if (IsConnected())
            {
                var id = session.Locations.GetLocationIdFromName(gameName, name);
                var result = await session.Locations.ScoutLocationsAsync(id);
                ScoutedItemInfo item = result[0];
                await session.Locations.CompleteLocationChecksAsync(id);
                FezapConsole.Print($"Sent {item.ItemDisplayName} to {item.ItemGame}", FezapConsole.OutputType.Info);
            }
            else
            {
                // TODO: Store items to be released in next Connect() call
            }
        }

        public void DotSay(string msg, bool nearGomez = true, bool hideAfter = true)
        {
            gameText[""]["FEZAP_CUSTOM"] = msg;
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
                        receivedItems.bits += 1;
                        break;
                    case "Anti-Cube":
                        GameState.SaveData.SecretCubes += 1;
                        receivedItems.antis += 1;
                        break;
                    case "Key":
                        GameState.SaveData.Keys += 1;
                        receivedItems.keys += 1;
                        break;
                    case "Owl":
                        GameState.SaveData.CollectedOwls += 1;
                        receivedItems.owls += 1;
                        break;
                    case "Red Map":
                        GameState.SaveData.Maps.Add("Red Map");
                        receivedItems.maps.Add("Red Map");
                        break;
                    case "Purple Map":
                        GameState.SaveData.Maps.Add("Purple Map");
                        receivedItems.maps.Add("Purple Map");
                        break;
                    case "Tower Map":
                        GameState.SaveData.Maps.Add("Tower Map");
                        receivedItems.maps.Add("Tower Map");
                        break;
                    case "QR Code Map":
                        GameState.SaveData.Maps.Add("QR Code Map");
                        receivedItems.maps.Add("QR Code Map");
                        break;
                    case "Burned Map":
                        GameState.SaveData.Maps.Add("Burned Map");
                        receivedItems.maps.Add("Burned Map");
                        break;
                    case "Cemetery Map 1":
                        GameState.SaveData.Maps.Add("Cemetery Map 1");
                        receivedItems.maps.Add("Cemetery Map 1");
                        break;
                    case "Cemetery Map 2":
                        GameState.SaveData.Maps.Add("Cemetery Map 2");
                        receivedItems.maps.Add("Cemetery Map 2");
                        break;
                    case "Cemetery Map 3":
                        GameState.SaveData.Maps.Add("Cemetery Map 3");
                        receivedItems.maps.Add("Cemetery Map 3");
                        break;
                    case "Cemetery Map 4":
                        GameState.SaveData.Maps.Add("Cemetery Map 4");
                        receivedItems.maps.Add("Cemetery Map 4");
                        break;
                    case "The Writing Cube":
                        GameState.SaveData.Artifacts.Add(ActorType.LetterCube);
                        receivedItems.artifacts.Add(ActorType.LetterCube);
                        break;
                    case "The Counting Cube":
                        GameState.SaveData.Artifacts.Add(ActorType.NumberCube);
                        receivedItems.artifacts.Add(ActorType.NumberCube);
                        break;
                    case "The Tome Artifact":
                        GameState.SaveData.Artifacts.Add(ActorType.Tome);
                        receivedItems.artifacts.Add(ActorType.Tome);
                        break;
                    case "The Skull Artifact":
                        GameState.SaveData.Artifacts.Add(ActorType.TriSkull);
                        receivedItems.artifacts.Add(ActorType.TriSkull);
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
                        DotSay(item.Player.Name + RandomHelper.InList(EmotionalSupportMsgs));
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

        private void RestoreItemInfo()
        {
            if (IsConnected())
            {
                receivedItems = new();
                collectedItems = new();

                foreach (var item in session.Items.AllItemsReceived)
                {
                    if (item.ItemName == "Golden Cube")
                    {
                        receivedItems.cubes += 1;
                    }
                    else if (item.ItemName == "Anti-Cube")
                    {
                        receivedItems.antis += 1;
                    }
                    else if (item.ItemName == "Heart Cube")
                    {
                        receivedItems.hearts += 1;
                    }
                    else if (item.ItemName == "Key")
                    {
                        receivedItems.keys += 1;
                    }
                    else if (item.ItemName == "Owl")
                    {
                        receivedItems.owls += 1;
                    }
                    else if (item.ItemName.Contains("Map"))
                    {
                        receivedItems.maps.Add(item.ItemName);
                    }
                    else if (item.ItemName == "The Writing Cube")
                    {
                        receivedItems.artifacts.Add(ActorType.LetterCube);
                    }
                    else if (item.ItemName == "The Counting Cube")
                    {
                        receivedItems.artifacts.Add(ActorType.NumberCube);
                    }
                    else if (item.ItemName == "The Tome Artifact")
                    {
                        receivedItems.artifacts.Add(ActorType.Tome);
                    }
                    else if (item.ItemName == "The Skull Artifact")
                    {
                        receivedItems.artifacts.Add(ActorType.TriSkull);
                    }
                }

                foreach (var locationId in session.Locations.AllLocationsChecked)
                {
                    string locationName = session.Locations.GetLocationNameFromId(locationId);
                    if (locationName.Contains("Golden Cube"))
                    {
                        collectedItems.cubes += 1;
                    }
                    else if (locationName.Contains("Anti-Cube"))
                    {
                        collectedItems.antis += 1;
                    }
                    else if (locationName.Contains("Heart Cube"))
                    {
                        collectedItems.hearts += 1;
                    }
                    else if (locationName.Contains("Key"))
                    {
                        collectedItems.keys += 1;
                    }
                    else if (locationName.Contains("Owl"))
                    {
                        collectedItems.owls += 1;
                    }
                    else if (locationName.Contains("Map"))
                    {
                        collectedItems.maps.Add(locationName);
                    }
                    else if (locationName == "The Writing Cube")
                    {
                        collectedItems.artifacts.Add(ActorType.LetterCube);
                    }
                    else if (locationName == "The Counting Cube")
                    {
                        collectedItems.artifacts.Add(ActorType.NumberCube);
                    }
                    else if (locationName == "The Tome Artifact")
                    {
                        collectedItems.artifacts.Add(ActorType.Tome);
                    }
                    else if (locationName == "The Skull Artifact")
                    {
                        collectedItems.artifacts.Add(ActorType.TriSkull);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsConnected())
            {
                MonitorDeath();
                MonitorCollectibles();
                MonitorGoal();
            }
        }

        private void MonitorDeath()
        {
            // sentDeath is used to avoid continuously sending deathlinks
            if (GomezService.Alive)
            {
                sentDeath = false;
            }
            else if (!GomezService.Alive && !sentDeath)
            {
                // TODO: Customise Cause with PlayerManager.Action and checking ActionType
                var deathlink = new DeathLink(session.Players.ActivePlayer.Name);
                deathLinkService.SendDeathLink(deathlink);
                FezapConsole.Print("Death sent");
                sentDeath = true;
            }
        }

        private void MonitorCollectibles()
        {
            int bitDiff = GameState.SaveData.CollectedParts - receivedItems.bits;
            int cubeDiff = GameState.SaveData.CubeShards - receivedItems.cubes;
            int antiDiff = GameState.SaveData.SecretCubes - receivedItems.antis;
            int heartDiff = GameState.SaveData.PiecesOfHeart - receivedItems.hearts;
            int keyDiff = GameState.SaveData.Keys - receivedItems.keys;
            int owlDiff = GameState.SaveData.CollectedOwls - receivedItems.owls;
            // TODO: Fix null problem
            // List<string> mapDiff = [.. GameState.SaveData.Maps.Except(receivedItems.maps)];
            // List<ActorType> artifactDiff = [.. GameState.SaveData.Artifacts.Except(receivedItems.artifacts)];

            if (bitDiff > 0)
            {
                GameState.SaveData.CollectedParts -= bitDiff;
                for (int i = 1; i <= bitDiff; i++)
                {
                    int count = collectedItems.bits + i;
                    _ = SendLocation($"Cube Bit {count}");
                }
                collectedItems.bits += bitDiff;
            }

            if (cubeDiff > 0)
            {
                GameState.SaveData.CubeShards -= cubeDiff;
                for (int i = 1; i <= cubeDiff; i++)
                {
                    int count = collectedItems.cubes + i;
                    _ = SendLocation($"Golden Cube {count}");
                }
                collectedItems.cubes += cubeDiff;
            }

            if (antiDiff > 0)
            {
                GameState.SaveData.SecretCubes -= antiDiff;
                for (int i = 1; i <= antiDiff; i++)
                {
                    int count = collectedItems.antis + i;
                    _ = SendLocation($"Anti-Cube {count}");
                }
                collectedItems.antis += antiDiff;
            }

            if (heartDiff > 0)
            {
                GameState.SaveData.PiecesOfHeart -= heartDiff;
                for (int i = 1; i <= heartDiff; i++)
                {
                    int count = collectedItems.hearts + i;
                    _ = SendLocation($"Heart Cube {count}");
                }
                collectedItems.hearts += heartDiff;
            }

            if (keyDiff > 0)
            {
                GameState.SaveData.Keys -= keyDiff;
                for (int i = 1; i <= keyDiff; i++)
                {
                    int count = collectedItems.keys + i;
                    _ = SendLocation($"Key {count}");
                }
                collectedItems.keys += keyDiff;
            }

            if (owlDiff > 0)
            {
                GameState.SaveData.CollectedOwls -= owlDiff;
                for (int i = 1; i <= owlDiff; i++)
                {
                    int count = collectedItems.owls + i;
                    _ = SendLocation($"Owl {count}");
                }
                collectedItems.owls += owlDiff;
            }

            // if (mapDiff.Count > 0)
            // {
            //     foreach (string map in mapDiff)
            //     {
            //         GameState.SaveData.Maps.Remove(map);
            //         collectedItems.maps.Add(map);
            //         _ = SendLocation(map);
            //     }
            // }

            // if (artifactDiff.Count > 0)
            // {
            //     foreach (ActorType artifact in artifactDiff)
            //     {
            //         GameState.SaveData.Artifacts.Remove(artifact);
            //         collectedItems.artifacts.Add(artifact);
            //         string artifactName = "";
            //         switch (artifact)
            //         {
            //             case ActorType.LetterCube:
            //                 artifactName = "The Writing Cube";
            //                 break;
            //             case ActorType.NumberCube:
            //                 artifactName = "The Counting Cube";
            //                 break;
            //             case ActorType.Tome:
            //                 artifactName = "The Tome Artifact";
            //                 break;
            //             case ActorType.TriSkull:
            //                 artifactName = "The Skull Artifact";
            //                 break;
            //         }
            //         _ = SendLocation(artifactName);
            //     }
            // }
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
