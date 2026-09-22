using Archipelago.MultiClient.Net.Models;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FEZUG.Features;
using FEZUG.Features.Console;

namespace FEZAP.Archipelago
{
    public struct AbilityData(bool Carry, bool TurnObjects, bool Jetpack)
    {
        public bool Carry = Carry;
        public bool TurnObjects = TurnObjects;
        public bool Jetpack = Jetpack;
    }

    /// Collectible data container
    public struct CollectibleData(List<ActorType> Artifacts, int CollectedOwls, int CollectedParts, int CubeShards,
                                  int Keys, List<string> Maps, int PiecesOfHeart, int SecretCubes)
    {
        public List<ActorType> Artifacts = Artifacts;
        public int CollectedOwls = CollectedOwls;
        public int CollectedParts = CollectedParts;
        public int CubeShards = CubeShards;
        public int Keys = Keys;
        public List<string> Maps = Maps;
        public int PiecesOfHeart = PiecesOfHeart;
        public int SecretCubes = SecretCubes;
    };

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

        public static AbilityData ReceivedAbilityData = new(false, false, false);

        public static CollectibleData ReceivedCollectibleData = new([], 0, 0, 0, 0, [], 0, 0);

        public static bool IsOneTimeItem(string itemName)
        {
            return itemName.Contains("Trap") || (itemName == "Emotional Support");
        }

        private static readonly List<string> EmotionalSupportMsgs = [
            " wants you to know you got this",
            " believes in you",
            " is cheering you on",
            " is rooting for you"
        ];

        public void HandleReceivedItem(ItemInfo item)
        {
            switch (item.ItemName)
            {
                case "Golden Cube":
                    ReceivedCollectibleData.CubeShards++;
                    GameState.SaveData.CubeShards = ReceivedCollectibleData.CubeShards;
                    GameState.OnHudElementChanged();
                    break;
                case "Anti-Cube":
                    ReceivedCollectibleData.SecretCubes++;
                    GameState.SaveData.SecretCubes = ReceivedCollectibleData.SecretCubes;
                    GameState.OnHudElementChanged();
                    break;
                case "Cube Bit":
                    ReceivedCollectibleData.CollectedParts++;
                    if (ReceivedCollectibleData.CollectedParts == 8)
                    {
                        ReceivedCollectibleData.CollectedParts = 0;
                        ReceivedCollectibleData.CubeShards++;
                        GameState.SaveData.CubeShards = ReceivedCollectibleData.CubeShards;
                    }
                    GameState.SaveData.CollectedParts = ReceivedCollectibleData.CollectedParts;
                    GameState.OnHudElementChanged();
                    break;
                case "Owl":
                    ReceivedCollectibleData.CollectedOwls++;
                    GameState.SaveData.CollectedOwls = ReceivedCollectibleData.CollectedOwls;
                    break;
                case "Heart Cube":
                    ReceivedCollectibleData.PiecesOfHeart++;
                    GameState.SaveData.PiecesOfHeart = ReceivedCollectibleData.PiecesOfHeart;
                    break;
                case "Arch Map":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_ARCH"))
                        ReceivedCollectibleData.Maps.Add("MAP_ARCH");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "Crypt Map A":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_CRYPT_A"))
                        ReceivedCollectibleData.Maps.Add("MAP_CRYPT_A");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "Crypt Map B":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_CRYPT_B"))
                        ReceivedCollectibleData.Maps.Add("MAP_CRYPT_B");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "Crypt Map C":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_CRYPT_C"))
                        ReceivedCollectibleData.Maps.Add("MAP_CRYPT_C");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "Crypt Map D":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_CRYPT_D"))
                        ReceivedCollectibleData.Maps.Add("MAP_CRYPT_D");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "QR Code Map":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_MYSTERY"))
                        ReceivedCollectibleData.Maps.Add("MAP_MYSTERY");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "Pivot Map":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_PIVOT"))
                        ReceivedCollectibleData.Maps.Add("MAP_PIVOT");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "Ritual Map":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_RITUAL"))
                        ReceivedCollectibleData.Maps.Add("MAP_RITUAL");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "Tree Sky Map":
                    if (!ReceivedCollectibleData.Maps.Contains("MAP_TREE_SKY"))
                        ReceivedCollectibleData.Maps.Add("MAP_TREE_SKY");
                    GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
                    break;
                case "The Writing Cube":
                    if (!ReceivedCollectibleData.Artifacts.Contains(ActorType.LetterCube))
                        ReceivedCollectibleData.Artifacts.Add(ActorType.LetterCube);
                    GameState.SaveData.Artifacts = [.. ReceivedCollectibleData.Artifacts];
                    break;
                case "The Counting Cube":
                    if (!ReceivedCollectibleData.Artifacts.Contains(ActorType.NumberCube))
                        ReceivedCollectibleData.Artifacts.Add(ActorType.NumberCube);
                    GameState.SaveData.Artifacts = [.. ReceivedCollectibleData.Artifacts];
                    break;
                case "The Tome Artifact":
                    if (!ReceivedCollectibleData.Artifacts.Contains(ActorType.Tome))
                        ReceivedCollectibleData.Artifacts.Add(ActorType.Tome);
                    GameState.SaveData.Artifacts = [.. ReceivedCollectibleData.Artifacts];
                    break;
                case "The Skull Artifact":
                    if (!ReceivedCollectibleData.Artifacts.Contains(ActorType.TriSkull))
                        ReceivedCollectibleData.Artifacts.Add(ActorType.TriSkull);
                    GameState.SaveData.Artifacts = [.. ReceivedCollectibleData.Artifacts];
                    break;
                case "Sunglasses":
                    GameState.SaveData.HasFPView = true;
                    break;
                case "Boileroom Door Unlocked":
                    DoorManager.BoileroomUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Lighthouse Door Unlocked":
                    DoorManager.LighthouseUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Tree Crumble Door Unlocked":
                    DoorManager.TreeCrumbleUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Well Door Unlocked":
                    DoorManager.WellUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Windmill Door Unlocked":
                    DoorManager.WindmillUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Mausoleum Door Unlocked":
                    DoorManager.MausoleumUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Sewer QR Door Unlocked":
                    DoorManager.SewerQrUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Sewer Fork Door Unlocked":
                    DoorManager.SewerForkUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Arch Door Unlocked":
                    DoorManager.ArchUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Bell Tower Door Unlocked":
                    DoorManager.BellTowerUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Cabin Door Unlocked":
                    DoorManager.CabinUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Throne Door Unlocked":
                    DoorManager.ThroneUnlocked = true;
                    Fezap.doorManager.HandleDoors();
                    break;
                case "Carry":
                    ReceivedAbilityData.Carry = true;
                    break;
                case "Turn Objects":
                    ReceivedAbilityData.TurnObjects = true;
                    break;
                case "Jetpack":
                    ReceivedAbilityData.Jetpack = true;
                    break;
                case "Rotation Trap":
                    DoRotationTrap();
                    break;
                case "Reload Trap":
                    DoReloadTrap();
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

        public void MonitorItems()
        {
            if (!GameState.SaveData.Artifacts.SequenceEqual(ReceivedCollectibleData.Artifacts))
            {
                #if DEBUG
                FezugConsole.Print("Artifacts mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.Artifacts = [.. ReceivedCollectibleData.Artifacts];
            }
            if (GameState.SaveData.CollectedOwls != ReceivedCollectibleData.CollectedOwls)
            {
                #if DEBUG
                FezugConsole.Print("CollectedOwls mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.CollectedOwls = ReceivedCollectibleData.CollectedOwls;
            }
            if (GameState.SaveData.CollectedParts != ReceivedCollectibleData.CollectedParts)
            {
                #if DEBUG
                FezugConsole.Print("CollectedParts mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.CollectedParts = ReceivedCollectibleData.CollectedParts;
            }
            if (GameState.SaveData.CubeShards != ReceivedCollectibleData.CubeShards)
            {
                #if DEBUG
                FezugConsole.Print("CubeShards mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.CubeShards = ReceivedCollectibleData.CubeShards;
            }
            if (GameState.SaveData.Keys != ReceivedCollectibleData.Keys)
            {
                #if DEBUG
                FezugConsole.Print("Keys mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.Keys = ReceivedCollectibleData.Keys;
            }
            if (!GameState.SaveData.Maps.SequenceEqual(ReceivedCollectibleData.Maps))
            {
                #if DEBUG
                FezugConsole.Print("Maps mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.Maps = [.. ReceivedCollectibleData.Maps];
            }
            if (GameState.SaveData.PiecesOfHeart != ReceivedCollectibleData.PiecesOfHeart)
            {
                #if DEBUG
                FezugConsole.Print("PiecesOfHeart mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.PiecesOfHeart = ReceivedCollectibleData.PiecesOfHeart;
            }
            if (GameState.SaveData.SecretCubes != ReceivedCollectibleData.SecretCubes)
            {
                #if DEBUG
                FezugConsole.Print("SecretCubes mismatch!", FezugConsole.OutputType.Warning);
                #endif // DEBUG
                GameState.SaveData.SecretCubes = ReceivedCollectibleData.SecretCubes;
            }
        }

        private void DoRotationTrap()
        {
            if (LevelManager.Flat)
                return;
            List<int> rotationOptions = [-2, -1, 1, 2];
            int index = RandomHelper.Random.Next(rotationOptions.Count);
            CameraService.Rotate(rotationOptions[index]);
        }

        private void DoReloadTrap()
        {
            #if DEBUG
            // TODO: Some triles are weirdly absent until an input is given
            WarpLevel.Warp(LevelManager.Name);
            #endif  // DEBUG
        }

        private void DoGravityTrap()
        {
            #if DEBUG
            int gravityTrapDuration = 15;

            // Increase the gravity
            // TODO: Fix gravity trap causing doors to get stuck until level reload
            GameService.SetGravity(false, 4);

            // Add delayed effect
            // TODO: Extend the timer rather than creating a new one if one exists already
            TimeSpan targetTime = Fezap.GameTime.TotalGameTime + new TimeSpan(0, 0, gravityTrapDuration);
            DelayedAction delayedAction = new(targetTime, () => { GameService.SetGravity(false, 1); });
            Fezap.delayedActions.Add(delayedAction);

            // Add countdown for when the gravity trap will end
            for (int i = 1; i <= gravityTrapDuration; i++)
            {
                TimeSpan timeOfMsg = Fezap.GameTime.TotalGameTime + new TimeSpan(0, 0, gravityTrapDuration - i);
                string msg = $"{i}";
                DelayedAction msgAction = new(timeOfMsg, () => { FezugConsole.Print(msg); });
                Fezap.delayedActions.Add(msgAction);
            }
            #endif  // DEBUG
        }

        private void DoEmotionalSupport(ItemInfo item)
        {
            string msg = item.Player.Name + RandomHelper.InList(EmotionalSupportMsgs);
            _ = DotService.Say($"@{msg}", true, true);
        }
    }
}
