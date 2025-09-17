using FezEngine.Services;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using FEZUG.Features.Console;
using Color = Microsoft.Xna.Framework.Color;

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
            }
            FezugConsole.Print("Location data restored");
        }

        public bool IsCollected(Location location)
        {
            // TODO: Figure out a way to have GameState.SaveData.World already contain all levels
            return false;
            // LevelSaveData level = GameState.SaveData.World[location.levelName];
            // return level.DestroyedTriles.Contains(location.emplacement);
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
                case 0:
                    goalAchieved = GameState.SaveData.Finished32;
                    break;
                case 1:
                    goalAchieved = GameState.SaveData.Finished64;
                    break;
                default:
                    FezugConsole.Print("Incorrect slot data for goal", FezugConsole.OutputType.Error);
                    break;
            }

            if (goalAchieved)
            {
                ArchipelagoManager.session.SetGoalAchieved();
            }
        }
    }
}
