using FezEngine.Services;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using FEZUG.Features.Console;
using FEZUG.Helpers;
using Microsoft.Xna.Framework;

namespace FEZAP.Archipelago
{
    /// Collectible data container
    public readonly struct CollectibleData(List<ActorType> artifacts, int collectedOwls, int collectedParts, int cubeShards,
                                           int keys, List<string> maps, int piecesOfHeart, int secretCubes)
    {
        public readonly List<ActorType> artifacts = artifacts;
        public readonly int collectedOwls = collectedOwls;
        public readonly int collectedParts = collectedParts;
        public readonly int cubeShards = cubeShards;
        public readonly int keys = keys;
        public readonly List<string> maps = maps;
        public readonly int piecesOfHeart = piecesOfHeart;
        public readonly int secretCubes = secretCubes;
    };

    public class LocationManager
    {
        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public ILevelManager Level { get; set; }

        public static CollectibleData receivedCollectibleData = new();
        public static List<Location> allCollectedLocations = [];
        public static int goal;  // 0 is 32 Cubes and 1 is 64 Cubes

        public void RestoreCollectedLocations()
        {
            var serverCheckedIds = ArchipelagoManager.session.Locations.AllLocationsChecked;
            foreach (long id in serverCheckedIds)
            {
                // Identify and add location to our collected
                string name = ArchipelagoManager.session.Locations.GetLocationNameFromId(id);
                Location location = LocationData.allLocations.Find(location => location.name == name);
                allCollectedLocations.Add(location);

                // Pre-load the level if needed
                if (!GameState.SaveData.World.ContainsKey(location.levelName))
                {
                    GameState.SaveData.World.Add(location.levelName, new LevelSaveData());
                }

                // Remove the location from the save state
                LevelSaveData levelData = GameState.SaveData.World[location.levelName];
                switch (location.type)
                {
                    case LocationType.DestroyedTriles:
                        levelData.DestroyedTriles.Add(location.emplacement);
                        break;
                    case LocationType.InactiveArtObjects:
                        levelData.InactiveArtObjects.Add(location.index);
                        break;
                    case LocationType.InactiveVolumes:
                        levelData.InactiveVolumes.Add(location.index);
                        break;
                    case LocationType.InactiveNPCs:
                        levelData.InactiveNPCs.Add(location.index);
                        break;
                    case LocationType.AchievementCode:
                        GameState.SaveData.AchievementCheatCodeDone = true;
                        break;
                }
            }
        }

        private bool IsCollected(Location location)
        {
            // If level doesn't exist, it's not collected in this save
            if (!GameState.SaveData.World.ContainsKey(location.levelName))
            {
                // NOTE: Don't preload levels since that early unlocks link doors
                return false;
            }

            // Check if location has been collected
            LevelSaveData levelData = GameState.SaveData.World[location.levelName];
            return location.type switch
            {
                LocationType.DestroyedTriles => levelData.DestroyedTriles.Contains(location.emplacement),
                LocationType.InactiveArtObjects => levelData.InactiveArtObjects.Contains(location.index),
                LocationType.InactiveVolumes => levelData.InactiveVolumes.Contains(location.index),
                LocationType.InactiveNPCs => levelData.InactiveNPCs.Contains(location.index),
                LocationType.AchievementCode => GameState.SaveData.AchievementCheatCodeDone,
                _ => false,
            };
        }

        private List<Location> GetAllCollected()
        {
            List<Location> collectedLocations = [];
            foreach (Location location in LocationData.allLocations)
            {
                if (IsCollected(location))
                {
                    collectedLocations.Add(location);
                }
            }
            return collectedLocations;
        }

        public void MonitorCollectibles()
        {
            CollectibleData currentCollectibleData = new(
                GameState.SaveData.Artifacts,
                GameState.SaveData.CollectedOwls,
                GameState.SaveData.CollectedParts,
                GameState.SaveData.CubeShards,
                GameState.SaveData.Keys,
                GameState.SaveData.Maps,
                GameState.SaveData.PiecesOfHeart,
                GameState.SaveData.SecretCubes
            );

            // Remove what was collected
            if (!currentCollectibleData.Equals(receivedCollectibleData))
            {
                Fezap.itemManager.RestoreReceivedItems();
            }
        }

        public void MonitorLocations()
        {
            // Get what was collected
            var diff = GetAllCollected().Except(allCollectedLocations);

            // Safety check for if someone selects the wrong save
            if (diff.Count() > 10)
            {
                // NOTE: This could be improved with a mechanism for confirmation and no wall of text, but it gets the point across.
                FezugConsole.Print($"Collected {diff.Count()} locations since last send. Swap to the correct or a new save file.", FezugConsole.OutputType.Warning);
                return;
            }

            // Send if something was collected
            foreach (Location location in diff)
            {
                ArchipelagoManager.SendLocation(location.name);
                allCollectedLocations.Add(location);
            }
        }

        public void MonitorGoal()
        {
            int totalCubes = GameState.SaveData.CubeShards + GameState.SaveData.SecretCubes;
            if ((goal == 0) && Level.Name == "HEX_REBUILD" && totalCubes >= 32)
            {
                ArchipelagoManager.session.SetGoalAchieved();
                FezugConsole.Print("Victory!");
            }
            else if ((goal == 1) && Level.Name == "GOMEZ_HOUSE_END_64" && totalCubes >= 64)
            {
                ArchipelagoManager.session.SetGoalAchieved();
                FezugConsole.Print("Victory!");
            }
        }
    }
}
