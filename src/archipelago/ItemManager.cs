using Archipelago.MultiClient.Net.Models;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
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
        public ILevelManager LevelManager { private get; set; }

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
                if (!(item.ItemName.Contains("Trap") || (item.ItemName == "Emotional Support")))
                {
                    HandleReceivedItem(item);
                }
            }

            LocationManager.receivedCollectibleData = new(
                GameState.SaveData.Artifacts,
                GameState.SaveData.CollectedOwls,
                GameState.SaveData.CollectedParts,
                GameState.SaveData.CubeShards,
                GameState.SaveData.Keys,
                GameState.SaveData.Maps,
                GameState.SaveData.PiecesOfHeart,
                GameState.SaveData.SecretCubes
            );
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
                case "Arch Map":
                    GameState.SaveData.Maps.Add("MAP_ARCH");
                    break;
                case "Crypt Map A":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_A");
                    break;
                case "Crypt Map B":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_B");
                    break;
                case "Crypt Map C":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_C");
                    break;
                case "Crypt Map D":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_D");
                    break;
                case "QR Code Map":
                    GameState.SaveData.Maps.Add("MAP_MYSTERY");
                    break;
                case "Pivot Map":
                    GameState.SaveData.Maps.Add("MAP_PIVOT");
                    break;
                case "Ritual Map":
                    GameState.SaveData.Maps.Add("MAP_RITUAL");
                    break;
                case "Tree Sky Map":
                    GameState.SaveData.Maps.Add("MAP_TREE_SKY");
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
                case "Boileroom Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("VILLAGEVILLE_3D", [35, 30, 36]));
                    break;
                case "Lighthouse Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("LIGHTHOUSE", [21, 20, 27]));
                    break;
                case "Tree Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("TREE", [41, 50, 2]));
                    break;
                case "Well Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("RAILS", [14, 21, 14]));
                    break;
                case "Windmill Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("PIVOT_ONE", [26, 61, 30]));
                    break;
                case "Mausoleum Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("MAUSOLEUM", [21, 13, 23]));
                    break;
                case "Sewer Hub Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("SEWER_HUB", [10, 42, 9]));
                    break;
                case "Sewer Pillars Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("SEWER_PILLARS", [8, 14, 30]));
                    break;
                case "Arch Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("NATURE_HUB", [16, 18, 15]));
                    DoorManager.lockedDoors.Remove(new("NATURE_HUB", [16, 18, 15]));
                    break;
                case "Bell Tower Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("NATURE_HUB", [0, 14, 27]));
                    DoorManager.lockedDoors.Remove(new("NATURE_HUB", [0, 14, 27]));
                    break;
                case "Cabin Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("TREE", [24, 59, 20]));
                    DoorManager.lockedDoors.Remove(new("TREE", [24, 59, 20]));
                    break;
                case "Throne Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("TREE_SKY", [11, 51, 9]));
                    DoorManager.lockedDoors.Remove(new("TREE_SKY", [11, 51, 9]));
                    break;
                case "Rotation Trap":
                    DoRotationTrap();
                    break;
                case "Sleep Trap":
                    DoSleepTrap();
                    break;
                case "Gravity Trap":
                    DoGravityTrap();
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
            // Go to sleep
            PlayerManager.Action = ActionType.IdleSleep;  // TODO: Prevent sleep animation from being stopped
            PlayerManager.CanControl = false;

            // Add delayed effect
            // TODO: Extend the timer rather than creating a new one if one exists already
            TimeSpan targetTime = Fezap.GameTime.TotalGameTime + new TimeSpan(0, 0, 15);  // schedule for 15 seconds later
            DelayedAction delayedAction = new(targetTime, () => { PlayerManager.CanControl = true; });
            Fezap.delayedActions.Add(delayedAction);
        }

        private void DoGravityTrap()
        {
            // Increase the gravity
            // TODO: Fix gravity trap causing doors to get stuck until level reload
            GameService.SetGravity(false, 4);

            // Add delayed effect
            // TODO: Extend the timer rather than creating a new one if one exists already
            TimeSpan targetTime = Fezap.GameTime.TotalGameTime + new TimeSpan(0, 0, 15);  // schedule for 15 seconds later
            DelayedAction delayedAction = new(targetTime, () => { GameService.SetGravity(false, 1);; });
            Fezap.delayedActions.Add(delayedAction);
        }

        public void DoEmotionalSupport(ItemInfo item)
        {
            string msg = item.Player.Name + RandomHelper.InList(EmotionalSupportMsgs);
            _ = DotService.Say($"@{msg}", true, true);
        }
    }
}
