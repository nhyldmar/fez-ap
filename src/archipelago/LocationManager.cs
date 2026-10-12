using FezEngine.Services;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
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
        public static int goal;  // number of cubes required to unlock the goal
        public static bool shuffleClockAntis;

        public void RestoreCollectedLocations()
        {
            var serverCheckedIds = ArchipelagoManager.session.Locations.AllLocationsChecked;
            foreach (long id in serverCheckedIds)
            {
                MarkLocationCollected(id);
            }
        }

        public void MarkLocationCollected(long id)
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
            int count;
            switch (location.type)
            {
                case LocationType.DestroyedTriles:
                    count = levelData.DestroyedTriles.Count(x => x == location.emplacement);
                    if (count < location.count)
                    {
                        levelData.DestroyedTriles.AddRange(Enumerable.Repeat(location.emplacement, location.count - count));
                        AddWinConditions(levelData.FilledConditions, location.winConditions);
                    }
                    break;
                case LocationType.InactiveArtObjects:
                    if (!levelData.InactiveArtObjects.Contains(location.index))
                    {
                        levelData.InactiveArtObjects.Add(location.index);
                        AddWinConditions(levelData.FilledConditions, location.winConditions);
                    }
                    break;
                case LocationType.InactiveNPCs:
                    if (!levelData.InactiveNPCs.Contains(location.index))
                    {
                        levelData.InactiveNPCs.Add(location.index);
                        AddWinConditions(levelData.FilledConditions, location.winConditions);
                    }
                    break;
                case LocationType.AchievementCode:
                    GameState.SaveData.AchievementCheatCodeDone = true;
                    break;
                case LocationType.InactiveVolumesAndCollected:
                    if (!levelData.InactiveVolumes.Contains(location.index))
                    {
                        levelData.InactiveVolumes.Add(location.index);
                        AddWinConditions(levelData.FilledConditions, location.winConditions);
                    }
                    levelData.ScriptingState = null;
                    break;
                case LocationType.InactiveArtObjectsAndCollected:
                    if (!levelData.InactiveArtObjects.Contains(location.index))
                    {
                        levelData.InactiveArtObjects.Add(location.index);
                        AddWinConditions(levelData.FilledConditions, location.winConditions);
                    }
                    levelData.ScriptingState = null;
                    break;
                case LocationType.InactiveArtObjectsAndDestroyedTriles:
                    if (!levelData.InactiveArtObjects.Contains(location.index))
                    {
                        levelData.InactiveArtObjects.Add(location.index);
                        AddWinConditions(levelData.FilledConditions, location.winConditions);
                    }
                    count = levelData.DestroyedTriles.Count(x => x == location.emplacement);
                    if (count < location.count)
                        levelData.DestroyedTriles.AddRange(Enumerable.Repeat(location.emplacement, location.count - count));
                    break;
                case LocationType.SharedParlorZuQr:
                    CollectSharedParlorZuQrCube();
                    break;
                case LocationType.SharedThroneSewerQr:
                    CollectSharedThroneSewerQrCube();
                    break;
                case LocationType.SharedWatertowerMapQr:
                    CollectSharedWatertowerMapQrCube();
                    break;
            }
        }

        private void CollectSharedParlorZuQrCube()
        {
            // Loosely based on TrialAndAwards.ResolveZuQR
            if (!GameState.SaveData.World.ContainsKey("PARLOR"))
                GameState.SaveData.World.Add("PARLOR", new LevelSaveData());
            LevelSaveData levelData = GameState.SaveData.World["PARLOR"];
            if (!levelData.InactiveVolumes.Contains(4))
            {
                levelData.InactiveVolumes.Add(4);
                levelData.FilledConditions.SecretCount++;
            }
            levelData.ScriptingState = null;

            if (!GameState.SaveData.World.ContainsKey("ZU_HOUSE_QR"))
                GameState.SaveData.World.Add("ZU_HOUSE_QR", new LevelSaveData());
            levelData = GameState.SaveData.World["ZU_HOUSE_QR"];
            if (!levelData.InactiveVolumes.Contains(0))
            {
                levelData.InactiveVolumes.Add(0);
                levelData.FilledConditions.SecretCount++;
            }
            levelData.ScriptingState = null;
        }

        private void CollectSharedThroneSewerQrCube()
        {
            // Loosely based on TrialAndAwards.ResolveSewerQR
            if (!GameState.SaveData.World.ContainsKey("SEWER_QR"))
                GameState.SaveData.World.Add("SEWER_QR", new LevelSaveData());
            LevelSaveData levelData = GameState.SaveData.World["SEWER_QR"];
            if (!levelData.InactiveArtObjects.Contains(0))
            {
                levelData.InactiveArtObjects.Add(0);
                levelData.FilledConditions.SecretCount++;
            }
            levelData.ScriptingState = null;

            if (!GameState.SaveData.World.ContainsKey("ZU_THRONE_RUINS"))
                GameState.SaveData.World.Add("ZU_THRONE_RUINS", new LevelSaveData());
            levelData = GameState.SaveData.World["ZU_THRONE_RUINS"];
            if (!levelData.InactiveVolumes.Contains(2))
            {
                levelData.InactiveVolumes.Add(2);
                levelData.FilledConditions.SecretCount++;
            }
            levelData.ScriptingState = null;

            if (!GameState.SaveData.World.ContainsKey("ZU_HOUSE_EMPTY"))
                GameState.SaveData.World.Add("ZU_HOUSE_EMPTY", new LevelSaveData());
            levelData = GameState.SaveData.World["ZU_HOUSE_EMPTY"];
            if (!levelData.InactiveVolumes.Contains(2))
            {
                levelData.InactiveVolumes.Add(2);
                levelData.FilledConditions.SecretCount++;
            }
            levelData.ScriptingState = null;
        }

        private void CollectSharedWatertowerMapQrCube()
        {
            GameState.SaveData.MapCheatCodeDone = true;

            if (!GameState.SaveData.World.ContainsKey("WATERTOWER_SECRET"))
                GameState.SaveData.World.Add("WATERTOWER_SECRET", new LevelSaveData());
            LevelSaveData levelData = GameState.SaveData.World["WATERTOWER_SECRET"];
            if (!levelData.InactiveVolumes.Contains(2))
            {
                levelData.InactiveVolumes.Add(2);
                levelData.FilledConditions.SecretCount++;
            }
            levelData.ScriptingState = null;
        }

        private void AddWinConditions(WinConditions main, WinConditions additions)
        {
            if (main == null || additions == null)
                return;
            main.LockedDoorCount += additions.LockedDoorCount;
            main.UnlockedDoorCount += additions.UnlockedDoorCount;
            main.ChestCount += additions.ChestCount;
            main.CubeShardCount += additions.CubeShardCount;
            main.OtherCollectibleCount += additions.OtherCollectibleCount;
            main.SplitUpCount += additions.SplitUpCount;
            foreach (int scriptId in additions.ScriptIds ?? [])
            {
                if (!main.ScriptIds.Contains(scriptId))
                {
                    main.ScriptIds.Add(scriptId);
                    main.ScriptIds.Sort();
                }
            }
            main.SecretCount += additions.SecretCount;
        }

        public void HandleDisabledClockTower()
        {
            if (shuffleClockAntis)
                return;
            var clockTowerLocations = LocationData.allLocations.Where(location => location.name.Contains("Clock Tower"));
            foreach (Location location in clockTowerLocations)
            {
                if (!GameState.SaveData.World.ContainsKey(location.levelName))
                    GameState.SaveData.World.Add(location.levelName, new LevelSaveData());
                LevelSaveData levelData = GameState.SaveData.World[location.levelName];

                if (!levelData.InactiveArtObjects.Contains(location.index))
                {
                    levelData.InactiveArtObjects.Add(location.index);
                    levelData.FilledConditions.SecretCount++;
                }
            }
        }

        private bool IsDestroyedTrileCollected(Location location, LevelSaveData levelData)
        {
            // Some collectibles overlap with other destroyed triles so this method checks their collection
            // by seeing if there's two instances of an emplacement.
            int emplacementCount = levelData.DestroyedTriles.FindAll(x => x == location.emplacement).Count();
            return emplacementCount == location.count;
        }

        private bool IsCollected(Location location)
        {
            // If we're not shuffling clock tower antis and this is one, don't count it as collected
            if (!shuffleClockAntis && location.name.Contains("Clock Tower"))
                return false;

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
                LocationType.DestroyedTriles => IsDestroyedTrileCollected(location, levelData),
                LocationType.InactiveArtObjects => levelData.InactiveArtObjects.Contains(location.index),
                LocationType.InactiveNPCs => levelData.InactiveNPCs.Contains(location.index),
                LocationType.AchievementCode => GameState.SaveData.AchievementCheatCodeDone,
                LocationType.InactiveVolumesAndCollected => levelData.InactiveVolumes.Contains(location.index) && levelData.ScriptingState != location.notCollectedState,
                LocationType.InactiveArtObjectsAndCollected => levelData.InactiveArtObjects.Contains(location.index) && levelData.ScriptingState != location.notCollectedState,
                LocationType.InactiveArtObjectsAndDestroyedTriles => levelData.InactiveArtObjects.Contains(location.index) && IsDestroyedTrileCollected(location, levelData),
                LocationType.SharedParlorZuQr => IsSharedParlorZuQrCubeCollected(),
                LocationType.SharedThroneSewerQr => IsSharedThroneSewerQrCubeCollected(),
                LocationType.SharedWatertowerMapQr => IsSharedWatertowerMapQrCubeCollected(),
                _ => false,
            };
        }

        private bool IsSharedParlorZuQrCubeCollected()
        {
            LevelSaveData parlorData = GameState.SaveData.World.ContainsKey("PARLOR") ? GameState.SaveData.World["PARLOR"] : null;
            LevelSaveData qrData = GameState.SaveData.World.ContainsKey("ZU_HOUSE_QR") ? GameState.SaveData.World["ZU_HOUSE_QR"] : null;

            if (parlorData != null && parlorData.ScriptingState == "NOT_COLLECTED")
                return false;
            if (qrData != null && qrData.ScriptingState == "NOT_COLLECTED")
                return false;

            if (parlorData != null && parlorData.InactiveVolumes.Contains(4))
                return true;
            if (qrData != null && qrData.InactiveVolumes.Contains(0))
                return true;

            return false;
        }

        private bool IsSharedThroneSewerQrCubeCollected()
        {
            LevelSaveData qrData = GameState.SaveData.World.ContainsKey("SEWER_QR") ? GameState.SaveData.World["SEWER_QR"] : null;
            LevelSaveData throne1Data = GameState.SaveData.World.ContainsKey("ZU_THRONE_RUINS") ? GameState.SaveData.World["ZU_THRONE_RUINS"] : null;
            LevelSaveData throne2Data = GameState.SaveData.World.ContainsKey("ZU_HOUSE_EMPTY") ? GameState.SaveData.World["ZU_HOUSE_EMPTY"] : null;

            if (qrData != null && qrData.ScriptingState == "NOT_COLLECTED")
                return false;
            if (throne1Data != null && throne1Data.ScriptingState == "NOT_COLLECTED")
                return false;
            if (throne2Data != null && throne2Data.ScriptingState == "NOT_COLLECTED")
                return false;

            if (qrData != null && qrData.InactiveArtObjects.Contains(0))
                return true;
            if (throne1Data != null && throne1Data.InactiveVolumes.Contains(2))
                return true;
            if (throne2Data != null && throne2Data.InactiveVolumes.Contains(2))
                return true;

            return false;
        }

        private bool IsSharedWatertowerMapQrCubeCollected()
        {
            LevelSaveData watertowerData = GameState.SaveData.World.ContainsKey("WATERTOWER_SECRET") ? GameState.SaveData.World["WATERTOWER_SECRET"] : null;

            if (watertowerData != null && watertowerData.ScriptingState == "NOT_COLLECTED")
                return false;

            return GameState.SaveData.MapCheatCodeDone;
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

        public void MonitorLocations()
        {
            // Get what was collected (we don't use Except since the throne anti-cube has 3 locations)
            var diff = GetAllCollected().Where(x => allCollectedLocations.All(y => x.name != y.name));

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
            if (Level.Name == "HEX_REBUILD" && totalCubes >= goal)
            {
                ArchipelagoManager.session.SetGoalAchieved();
                FezugConsole.Print("Victory!");
            }
        }
    }
}
