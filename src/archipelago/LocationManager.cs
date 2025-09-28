using FezEngine.Services;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using FEZUG;
using FEZUG.Features;
using FEZUG.Features.Console;

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
                string name = ArchipelagoManager.session.Locations.GetLocationNameFromId(id);
                Location location = LocationData.allLocations.Find(location => location.name == name);
                allCollectedLocations.Add(location);

                // Remove location if already collected (requires re-entering the level if in it)
                if (!GameState.SaveData.World.ContainsKey(location.levelName))
                {
                    GameState.SaveData.World.Add(location.levelName, new LevelSaveData());
                }
                LevelSaveData levelData = GameState.SaveData.World[location.levelName];
                levelData.DestroyedTriles.Add(location.emplacement);
            }
        }

        public bool IsCollected(Location location)
        {
            bool levelExists = GameState.SaveData.World.TryGetValue(location.levelName, out LevelSaveData level);
            return levelExists && level.DestroyedTriles.Contains(location.emplacement);
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
            // Send if something was collected
            var diff = GetAllCollected().Except(allCollectedLocations);
            foreach (Location location in diff)
            {
                ArchipelagoManager.SendLocation(location.name);
                allCollectedLocations.Add(location);
            }

            // Remove whatever was collected
            if (diff.Count() > 0)
            {
                // TODO: Figure out why this is not being called
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
