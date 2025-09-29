using FezEngine.Services;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;

namespace FEZAP.Archipelago
{
    public class LocationManager
    {
        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public ILevelManager Level { get; set; }

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

        public bool IsCollected(Location location)
        {
            // Pre-load the level if needed
            if (!GameState.SaveData.World.ContainsKey(location.levelName))
            {
                GameState.SaveData.World.Add(location.levelName, new LevelSaveData());
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

        public List<Location> GetAllCollected()
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

        public void MonitorLocations()
        {
            // Get what was collected
            var diff = GetAllCollected().Except(allCollectedLocations);

            // Send if something was collected
            foreach (Location location in diff)
            {
                ArchipelagoManager.SendLocation(location.name);
                allCollectedLocations.Add(location);
            }

            // Remove what was collected
            if (diff.Count() > 0)
            {
                Fezap.itemManager.RestoreReceivedItems();
            }
        }

        public void MonitorGoal()
        {
            if ((goal == 0) && Level.Name == "HEX_REBUILD")
            {
                ArchipelagoManager.session.SetGoalAchieved();
            }
            else if ((goal == 1) && Level.Name == "GOMEZ_HOUSE_END_64")
            {
                ArchipelagoManager.session.SetGoalAchieved();
            }
        }
    }
}
