using FEZAP.Features;
using FezEngine.Services;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using Microsoft.Xna.Framework;

namespace FEZAP.Helpers
{
    public struct Location(string name, string levelName, List<int> emplacement)
    {
        public string name = name;
        public string levelName = levelName;
        public TrileEmplacement emplacement = new(emplacement[0], emplacement[1], emplacement[2]);
    }

    public class CollectibleManager : IFezapFeature
    {
        [ServiceDependency]
        public IGameStateManager GameState { get; set; }
        [ServiceDependency]
        public ILevelManager Level { get; set; }

        public static readonly List<Location> allLocations = [
            new("Abandonded A Cube", "ABANDONED_A", [11, 9, 10])
            // TODO: Fill out all of these :(
        ];

        public static List<Location> allCollectedLocations = [];

        public bool IsCollected(Location location)
        {
            LevelSaveData currentLevel = GameState.SaveData.World[location.levelName];
            return currentLevel.DestroyedTriles.Contains(location.emplacement);
        }

        public List<Location> GetAllCollected()
        {
            List<Location> collectedLocations = [];
            foreach (Location location in allLocations)
            {
                if (IsCollected(location))
                {
                    collectedLocations.Add(location);
                }
            }
            return collectedLocations;
        }

        public void HandleCollectibles()
        {
            var diff = GetAllCollected().Except(allCollectedLocations);
            foreach (Location location in diff)
            {
                _ = Archipelago.SendLocation(location.name);
                allCollectedLocations.Add(location);
            }
        }

        public void RestoreCollectedLocations()
        {
            var serverCheckedIds = Archipelago.session.Locations.AllLocationsChecked;
            foreach (long id in serverCheckedIds)
            {
                string name = Archipelago.session.Locations.GetLocationNameFromId(id);
                Location location = allLocations.Find(location => location.name == name);
                allCollectedLocations.Add(location);
            }
        }

        public void Initialize() { }
        public void Update(GameTime gameTime) { }
        public void DrawHUD(GameTime gameTime) { }
        public void DrawLevel(GameTime gameTime) { }
    }
}
