using Archipelago.MultiClient.Net.Models;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FEZUG.Features.Console;

namespace FEZAP.Archipelago
{
    public class ItemManager
    {
        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public ICameraService CameraService { private get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        public IGameService GameService { private get; set; }

        [ServiceDependency]
        public IDotService DotService { private get; set; }

        private static readonly List<string> EmotionalSupportMsgs = [
            " wants you to know you got this",
            " believes in you",
            " is cheering you on",
            " is rooting for you"
        ];

        private void ClearCollectibleSaveData()
        {
            GameState.SaveData.Artifacts = [];
            GameState.SaveData.CollectedOwls = 0;
            GameState.SaveData.CollectedParts = 0;
            GameState.SaveData.CubeShards = 0;
            GameState.SaveData.Keys = 0;
            GameState.SaveData.Maps = [];
            GameState.SaveData.PiecesOfHeart = 0;
            GameState.SaveData.SecretCubes = 0;
        }

        public void RestoreReceivedItems()
        {
            ClearCollectibleSaveData();
            List<ItemInfo> itemsReceived = [.. ArchipelagoManager.session.Items.AllItemsReceived];
            foreach (ItemInfo item in itemsReceived)
            {
                HandleReceivedItem(item);
            }
            FezugConsole.Print("Item data restored");
        }

        public void HandleReceivedItem(ItemInfo item)
        {
            switch (item.ItemName)
            {
                case "Golden Cube":
                    GameState.SaveData.CubeShards += 1;
                    break;
                case "Anti-Cube":
                    GameState.SaveData.SecretCubes += 1;
                    break;
                case "Owl":
                    GameState.SaveData.CollectedOwls += 1;
                    break;
                case "Heart Cube":
                    GameState.SaveData.PiecesOfHeart += 1;
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
                    GameState.SaveData.Artifacts.Add(ActorType.LetterCube);
                    break;
                case "The Counting Cube":
                    GameState.SaveData.Artifacts.Add(ActorType.NumberCube);
                    break;
                case "The Tome Artifact":
                    GameState.SaveData.Artifacts.Add(ActorType.Tome);
                    break;
                case "The Skull Artifact":
                    GameState.SaveData.Artifacts.Add(ActorType.TriSkull);
                    break;
                case "Sunglasses":
                    GameState.SaveData.HasFPView = true;
                    break;
                case "... Door Unlocked":
                    // TODO: Add door unlock logic for each of the 8 doors
                    break;
                case "Rotation Trap":
                    DoRotationTrap();
                    break;
                case "Sleep Trap":
                    DoSleepTrap();
                    break;
                case "Emotional Support":
                    DoEmotionalSupport(item);
                    break;
                default:
                    FezugConsole.Print($"Unknown item: {item.ItemDisplayName}", FezugConsole.OutputType.Error);
                    break;
            }
        }

        private void DoRotationTrap()
        {
            CameraService.Rotate(RandomHelper.Random.Next(-2, 2));
        }

        private void DoSleepTrap()
        {
            PlayerManager.NextAction = FezGame.Structure.ActionType.IdleSleep;
            PlayerManager.CanControl = false;
            GameService.Wait(5);
            PlayerManager.CanControl = true;
        }

        public void DoEmotionalSupport(ItemInfo item)
        {
            string msg = item.Player.Name + RandomHelper.InList(EmotionalSupportMsgs);
            _ = DotService.Say($"@{msg}", true, true);
        }
    }
}
