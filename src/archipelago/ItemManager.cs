using Archipelago.MultiClient.Net.Models;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FEZUG.Features;
using FEZUG.Features.Console;
using Microsoft.Xna.Framework.Audio;

namespace FEZAP.Archipelago
{
    internal enum ItemSound
    {
        Progression,
        NonProgression,
        Trap,
        Filler,
    }

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

        [ServiceDependency]
        public IContentManagerProvider ContentManagerProvider { private get; set; }

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
                    PlaySound(ItemSound.Progression);
                    break;
                case "Anti-Cube":
                    GameState.SaveData.SecretCubes += 1;
                    PlaySound(ItemSound.Progression);
                    break;
                case "Owl":
                    GameState.SaveData.CollectedOwls += 1;
                    PlaySound(ItemSound.Progression);
                    break;
                case "Heart Cube":
                    GameState.SaveData.PiecesOfHeart += 1;
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Arch Map":
                    GameState.SaveData.Maps.Add("MAP_ARCH");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Crypt Map A":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_A");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Crypt Map B":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_B");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Crypt Map C":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_C");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Crypt Map D":
                    GameState.SaveData.Maps.Add("MAP_CRYPT_D");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "QR Code Map":
                    GameState.SaveData.Maps.Add("MAP_MYSTERY");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Pivot Map":
                    GameState.SaveData.Maps.Add("MAP_PIVOT");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Ritual Map":
                    GameState.SaveData.Maps.Add("MAP_RITUAL");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Tree Sky Map":
                    GameState.SaveData.Maps.Add("MAP_TREE_SKY");
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "The Writing Cube":
                    GameState.SaveData.Artifacts.Add(ActorType.LetterCube);
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "The Counting Cube":
                    GameState.SaveData.Artifacts.Add(ActorType.NumberCube);
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "The Tome Artifact":
                    GameState.SaveData.Artifacts.Add(ActorType.Tome);
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "The Skull Artifact":
                    GameState.SaveData.Artifacts.Add(ActorType.TriSkull);
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Sunglasses":
                    GameState.SaveData.HasFPView = true;
                    PlaySound(ItemSound.NonProgression);
                    break;
                case "Boileroom Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("VILLAGEVILLE_3D", [35, 30, 36]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Lighthouse Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("LIGHTHOUSE", [21, 20, 27]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Tree Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("TREE", [41, 50, 2]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Well Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("RAILS", [14, 21, 14]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Windmill Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("PIVOT_ONE", [26, 61, 30]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Mausoleum Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("MAUSOLEUM", [21, 13, 23]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Sewer Hub Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("SEWER_HUB", [10, 42, 9]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Sewer Pillars Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("SEWER_PILLARS", [8, 14, 30]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Arch Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("NATURE_HUB", [16, 18, 15]));
                    DoorManager.lockedDoors.Remove(new("NATURE_HUB", [16, 18, 15]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Bell Tower Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("NATURE_HUB", [0, 14, 27]));
                    DoorManager.lockedDoors.Remove(new("NATURE_HUB", [0, 14, 27]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Cabin Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("TREE", [24, 59, 20]));
                    DoorManager.lockedDoors.Remove(new("TREE", [24, 59, 20]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Throne Door Unlocked":
                    DoorManager.unlockedDoors.Add(new("TREE_SKY", [11, 51, 9]));
                    DoorManager.lockedDoors.Remove(new("TREE_SKY", [11, 51, 9]));
                    PlaySound(ItemSound.Progression);
                    break;
                case "Rotation Trap":
                    DoRotationTrap();
                    PlaySound(ItemSound.Trap);
                    break;
                case "Reload Trap":
                    DoReloadTrap();
                    PlaySound(ItemSound.Trap);
                    break;
                case "Gravity Trap":
                    DoGravityTrap();
                    PlaySound(ItemSound.Trap);
                    break;
                case "Emotional Support":
                    DoEmotionalSupport(item);
                    PlaySound(ItemSound.Filler);
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

        private void DoReloadTrap()
        {
            // TODO: Some triles are weirdly absent until an input is given
            WarpLevel.Warp(LevelManager.Name);
        }

        private void DoGravityTrap()
        {
            // Increase the gravity
            // TODO: Fix gravity trap causing doors to get stuck until level reload
            GameService.SetGravity(false, 4);

            // Add delayed effect
            // TODO: Extend the timer rather than creating a new one if one exists already
            TimeSpan targetTime = Fezap.GameTime.TotalGameTime + new TimeSpan(0, 0, 15);  // schedule for 15 seconds later
            DelayedAction delayedAction = new(targetTime, () => { GameService.SetGravity(false, 1); ; });
            Fezap.delayedActions.Add(delayedAction);
        }

        public void DoEmotionalSupport(ItemInfo item)
        {
            string msg = item.Player.Name + RandomHelper.InList(EmotionalSupportMsgs);
            _ = DotService.Say($"@{msg}", true, true);
        }

        private void PlaySound(ItemSound sound)
        {
            string soundEffectPath = "";
            switch (sound)
            {
                case ItemSound.Progression:
                    soundEffectPath = "sounds/collects/splitupcube/assemble_a_maj";
                    break;
                case ItemSound.NonProgression:
                    soundEffectPath = "sounds/ui/mapbeacon";
                    break;
                case ItemSound.Trap:
                    soundEffectPath = "sounds/ui/worldmapmagnet";
                    break;
                case ItemSound.Filler:
                    soundEffectPath = "sounds/gomez/yawn";
                    break;
            }

            SoundEffect soundEffect = ContentManagerProvider.Global.Load<SoundEffect>(soundEffectPath);
            soundEffect.EmitAt(PlayerManager.Position).NoAttenuation = true;
        }
    }
}
