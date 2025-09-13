using FezEngine.Services;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using Color = Microsoft.Xna.Framework.Color;

namespace FEZAP.Helpers
{
    public class LocationManager
    {
        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public ILevelManager Level { get; set; }

        public static List<Location> allCollectedLocations = [];
        public static string goal;

        public void RestoreCollectedLocations()
        {
            var serverCheckedIds = ArchipelagoManager.session.Locations.AllLocationsChecked;
            foreach (long id in serverCheckedIds)
            {
                string name = ArchipelagoManager.session.Locations.GetLocationNameFromId(id);
                Location location = LocationData.allLocations.Find(location => location.name == name);
                allCollectedLocations.Add(location);
            }
        }

        public bool IsCollected(Location location)
        {
            LevelSaveData currentLevel = GameState.SaveData.World[location.levelName];
            return currentLevel.DestroyedTriles.Contains(location.emplacement);
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
            var diff = GetAllCollected().Except(allCollectedLocations);
            foreach (Location location in diff)
            {
                _ = ArchipelagoManager.SendLocation(location.name);
                allCollectedLocations.Add(location);
            }
        }

        public void MonitorGoal()
        {
            bool goalAchieved = false;
            switch (goal)
            {
                case "32 Cubes":
                    goalAchieved = GameState.SaveData.Finished32;
                    break;
                case "64 Cubes":
                    goalAchieved = GameState.SaveData.Finished64;
                    break;
                default:
                    HudManager.Print("Incorrect slot data for goal", Color.Red);
                    break;
            }

            if (goalAchieved)
            {
                ArchipelagoManager.session.SetGoalAchieved();
            }
        }
    }
}
