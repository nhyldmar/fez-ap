using FEZAP.Features;
using FezEngine.Services;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using Microsoft.Xna.Framework;

namespace FEZAP.Helpers
{
    public class LocationManager : IFezapFeature
    {
        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public ILevelManager Level { get; set; }

        public static List<Location> allCollectedLocations = [];

        public static void RestoreCollectedLocations()
        {
            var serverCheckedIds = Archipelago.session.Locations.AllLocationsChecked;
            foreach (long id in serverCheckedIds)
            {
                string name = Archipelago.session.Locations.GetLocationNameFromId(id);
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

        public void HandleLocationChecking()
        {
            var diff = GetAllCollected().Except(allCollectedLocations);
            foreach (Location location in diff)
            {
                _ = Archipelago.SendLocation(location.name);
                allCollectedLocations.Add(location);
            }
        }

        public void Initialize() { }
        public void Update(GameTime gameTime) { }
        public void DrawHUD(GameTime gameTime) { }
        public void DrawLevel(GameTime gameTime) { }
    }
}
