using FezEngine.Services;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;

namespace FEZAP.Archipelago
{
    /// Door information container
    public readonly struct DoorInfo(string levelName, List<int> emplacement = null)
    {
        public readonly string levelName = levelName;
        public readonly TrileEmplacement emplacement = new(emplacement[0], emplacement[1], emplacement[2]);
    };

    public class DoorManager
    {
        [ServiceDependency]
        public IGameStateManager GameState { private get; set; }

        [ServiceDependency]
        public ILevelManager LevelManager { private get; set; }

        public static List<DoorInfo> lockedDoors = [
            new("NATURE_HUB", [16, 18, 15]),
            new("NATURE_HUB", [0, 14, 27]),
            new("TREE", [24, 59, 20]),
            new("TREE_SKY", [11, 51, 9]),
        ];
        public static List<DoorInfo> unlockedDoors = [];

        public void LockDoors()
        {
            var doorsInLevel = lockedDoors.Where(door => door.levelName == LevelManager.Name);
            foreach (var door in doorsInLevel)
            {
                // TODO: Fix visuals (currently black bottom trile and regular top trile)
                TrileEmplacement emplacement = door.emplacement;
                TrileInstance doorTrile = LevelManager.TrileInstanceAt(ref emplacement);
                var lockedDoorTrile = LevelManager.TrileSet.Triles.First(trile => trile.Value.Name == "Locked Door");
                LevelManager.SwapTrile(doorTrile, lockedDoorTrile.Value);
            }
        }

        public void UnlockDoors()
        {
            var doorsInLevel = unlockedDoors.Where(door => door.levelName == LevelManager.Name);
            foreach (var door in doorsInLevel)
            {
                LevelSaveData levelData = GameState.SaveData.World[door.levelName];
                levelData.InactiveTriles.Add(door.emplacement);
                // LevelManager.Rebuild();  // TODO: Fix this
            }
        }
    }
}
