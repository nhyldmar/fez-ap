using FezEngine.Structure;

namespace FEZAP.Archipelago
{
    public enum LocationType
    {
        DestroyedTriles,  // bits, most golden cubes, floating anti-cubes
        InactiveArtObjects,  // chests, clock anti-cubes
        InactiveNPCs,  // owls
        AchievementCode,  // for achievement code andit-cube
        InactiveVolumesAndCollected, // code-activatable anti-cubes/heart pieces
        InactiveArtObjectsAndCollected, // black monolith
        InactiveArtObjectsAndDestroyedTriles, // tuning forks, block puzzles, bell
        SharedParlorZuQr, // shared anti-cube between Parlor and Zu QR Wall
        SharedThroneSewerQr, // shared anti-cube between the two Zu Throne rooms and the Sewer QR Code
        SharedWatertowerMapQr, // shared anti-cube between Watertower Secret and the QR Code Map
    }

    /// Location information container
    public readonly struct Location(string name, string levelName, LocationType type, List<int> emplacement = null, int count = 1, int index = 0, string notCollectedState = "NOT_COLLECTED", WinConditions winConditions = null)
    {
        public readonly string name = name;  // apworld location name
        public readonly string levelName = levelName;  // name of the containing fezlvl
        public readonly LocationType type = type;  // type of location (needed for different handling)
        public readonly TrileEmplacement emplacement = (emplacement != null) ?
                                                       new(emplacement[0], emplacement[1], emplacement[2]) :
                                                       new(0, 0, 0);  // for DestroyedTriles
        public readonly int count = count;  // for overlaping DestroyedTriles
        public readonly int index = index;  // for InactiveArtObjects, Inactive NPCs, and Inactive Volumes
        public readonly string notCollectedState = notCollectedState; // for InactiveVolumesAndCollected if there's a special case
        public readonly WinConditions winConditions = winConditions; // any win conditions that should be added (so the map appears gold)
    };

    public class LocationData
    {
        private static readonly WinConditions WC_BIT = new() { SplitUpCount = 1 };
        private static readonly WinConditions WC_GOLDEN = new() { CubeShardCount = 1 };
        private static readonly WinConditions WC_SECRET = new() { SecretCount = 1 };
        private static readonly WinConditions WC_OTHER = new() { OtherCollectibleCount = 1 };
        private static readonly WinConditions WC_CHEST = new() { ChestCount = 1 };

        // 128 cube bits, usually converts to 8 full cubes
        private static readonly List<Location> cubeBitLocations = [
            new("Abandoned A Cube Bit", "ABANDONED_A", LocationType.DestroyedTriles, [11, 9, 10], winConditions: WC_BIT),
            new("Abandoned B Cube Bit", "ABANDONED_B", LocationType.DestroyedTriles, [7, 3, 10], winConditions: WC_BIT),
            new("Ancient Walls Cube Bit 1", "ANCIENT_WALLS", LocationType.DestroyedTriles, [37, 35, 3], winConditions: WC_BIT),
            new("Ancient Walls Cube Bit 2", "ANCIENT_WALLS", LocationType.DestroyedTriles, [32, 18, 25], winConditions: WC_BIT),
            new("Ancient Walls Cube Bit 3", "ANCIENT_WALLS", LocationType.DestroyedTriles, [38, 25, 5], winConditions: WC_BIT),
            new("Arch Cube Bit 1", "ARCH", LocationType.DestroyedTriles, [26, 36, 9], winConditions: WC_BIT),
            new("Arch Cube Bit 2", "ARCH", LocationType.DestroyedTriles, [18, 33, 4], winConditions: WC_BIT),
            new("Arch Cube Bit 3", "ARCH", LocationType.DestroyedTriles, [19, 37, 15], winConditions: WC_BIT),
            new("Bell Tower Cube Bit 1", "BELL_TOWER", LocationType.DestroyedTriles, [15, 32, 21], winConditions: WC_BIT),
            new("Bell Tower Cube Bit 2", "BELL_TOWER", LocationType.DestroyedTriles, [16, 31, 16], winConditions: WC_BIT),
            new("Big Owl Cube Bit", "BIG_OWL", LocationType.DestroyedTriles, [18, 13, 10], winConditions: WC_BIT),
            new("Big Tower Cube Bit 1", "BIG_TOWER", LocationType.DestroyedTriles, [13, 61, 10], winConditions: WC_BIT),
            new("Big Tower Cube Bit 2", "BIG_TOWER", LocationType.DestroyedTriles, [10, 63, 32], winConditions: WC_BIT),
            new("Big Tower Cube Bit 3", "BIG_TOWER", LocationType.DestroyedTriles, [24, 53, 25], winConditions: WC_BIT),
            new("Big Tower Cube Bit 4", "BIG_TOWER", LocationType.DestroyedTriles, [28, 41, 21], winConditions: WC_BIT),
            new("Big Tower Cube Bit 5", "BIG_TOWER", LocationType.DestroyedTriles, [24, 45, 16], winConditions: WC_BIT),
            new("Big Tower Cube Bit 6", "BIG_TOWER", LocationType.DestroyedTriles, [19, 49, 21], winConditions: WC_BIT),
            new("Big Tower Cube Bit 7", "BIG_TOWER", LocationType.DestroyedTriles, [33, 57, 30], winConditions: WC_BIT),
            new("Big Tower Cube Bit 8", "BIG_TOWER", LocationType.DestroyedTriles, [37, 59, 6], winConditions: WC_BIT),
            new("Extractor A Cube Bit", "EXTRACTOR_A", LocationType.DestroyedTriles, [24, 15, 12], winConditions: WC_BIT),
            new("Five Towers Cube Bit 1", "FIVE_TOWERS", LocationType.DestroyedTriles, [32, 34, 27], winConditions: WC_BIT),
            new("Five Towers Cube Bit 2", "FIVE_TOWERS", LocationType.DestroyedTriles, [26, 38, 21], winConditions: WC_BIT),
            new("Five Towers Cube Bit 3", "FIVE_TOWERS", LocationType.DestroyedTriles, [45, 58, 32], winConditions: WC_BIT),
            new("Fox Cube Bit", "FOX", LocationType.DestroyedTriles, [19, 74, 9], winConditions: WC_BIT),
            new("Fractal Cube Bit 1", "FRACTAL", LocationType.DestroyedTriles, [28, 48, 21], winConditions: WC_BIT),
            new("Fractal Cube Bit 2", "FRACTAL", LocationType.DestroyedTriles, [33, 26, 19], winConditions: WC_BIT),
            new("Fractal Cube Bit 3", "FRACTAL", LocationType.DestroyedTriles, [30, 26, 23], winConditions: WC_BIT),
            new("Globe Cube Bit", "GLOBE", LocationType.DestroyedTriles, [23, 33, 19], winConditions: WC_BIT),
            new("Graveyard A Cube Bit 1", "GRAVEYARD_A", LocationType.DestroyedTriles, [22, 39, 18], winConditions: WC_BIT),
            new("Graveyard A Cube Bit 2", "GRAVEYARD_A", LocationType.DestroyedTriles, [15, 54, 34], winConditions: WC_BIT),
            new("Graveyard A Cube Bit 3", "GRAVEYARD_A", LocationType.DestroyedTriles, [37, 73, 13], winConditions: WC_BIT),
            new("Graveyard Gate Cube Bit 1", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [31, 30, 17], winConditions: WC_BIT),
            new("Graveyard Gate Cube Bit 2", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [22, 25, 26], winConditions: WC_BIT),
            new("Graveyard Gate Cube Bit 3", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [28, 19, 13], winConditions: WC_BIT),
            new("Graveyard Gate Cube Bit 4", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [18, 29, 17], winConditions: WC_BIT),
            new("Graveyard Gate Cube Bit 5", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [30, 40, 14], winConditions: WC_BIT),
            new("Graveyard Gate Cube Bit 6", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [19, 42, 25], winConditions: WC_BIT),
            new("Graveyard Gate Cube Bit 7", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [25, 61, 20], winConditions: WC_BIT),
            new("Graveyard Cabin Cube Bit 1", "GRAVE_CABIN", LocationType.DestroyedTriles, [12, 19, 36], winConditions: WC_BIT),
            new("Graveyard Cabin Cube Bit 2", "GRAVE_CABIN", LocationType.DestroyedTriles, [13, 27, 37], winConditions: WC_BIT),
            new("Graveyard Ghost Cube Bit 1", "GRAVE_GHOST", LocationType.DestroyedTriles, [15, 21, 12], winConditions: WC_BIT),
            new("Graveyard Ghost Cube Bit 2", "GRAVE_GHOST", LocationType.DestroyedTriles, [9, 8, 15], winConditions: WC_BIT),
            new("Graveyard Lesser Gate Cube Bit", "GRAVE_LESSER_GATE", LocationType.DestroyedTriles, [18, 12, 10], winConditions: WC_BIT),
            new("Graveyard Treasure A Cube Bit 1", "GRAVE_TREASURE_A", LocationType.DestroyedTriles, [7, 45, 8], winConditions: WC_BIT),
            new("Graveyard Treasure A Cube Bit 2", "GRAVE_TREASURE_A", LocationType.DestroyedTriles, [10, 40, 5], winConditions: WC_BIT),
            new("Industrial Superspin Cube Bit 1", "INDUSTRIAL_SUPERSPIN", LocationType.DestroyedTriles, [14, 71, 58], winConditions: WC_BIT),
            new("Industrial Superspin Cube Bit 2", "INDUSTRIAL_SUPERSPIN", LocationType.DestroyedTriles, [27, 60, 40], winConditions: WC_BIT),
            new("Industrial Superspin Cube Bit 3", "INDUSTRIAL_SUPERSPIN", LocationType.DestroyedTriles, [29, 134, 9], winConditions: WC_BIT),
            new("Industrial Abandoned A Cube Bit", "INDUST_ABANDONED_A", LocationType.DestroyedTriles, [7, 11, 8], winConditions: WC_BIT),
            new("Kitchen Cube Bit", "KITCHEN", LocationType.DestroyedTriles, [7, 12, 13], winConditions: WC_BIT),
            new("Lighthouse Cube Bit 1", "LIGHTHOUSE", LocationType.DestroyedTriles, [23, 39, 32], winConditions: WC_BIT),
            new("Lighthouse Cube Bit 2", "LIGHTHOUSE", LocationType.DestroyedTriles, [8, 28, 45], winConditions: WC_BIT),
            new("Lighthouse House A Cube Bit", "LIGHTHOUSE_HOUSE_A", LocationType.DestroyedTriles, [13, 7, 4], winConditions: WC_BIT),
            new("Lighthouse Spin Cube Bit", "LIGHTHOUSE_SPIN", LocationType.DestroyedTriles, [10, 59, 15], winConditions: WC_BIT),
            new("Mausoleum Cube Bit 1", "MAUSOLEUM", LocationType.DestroyedTriles, [26, 19, 26], winConditions: WC_BIT),
            new("Mausoleum Cube Bit 2", "MAUSOLEUM", LocationType.DestroyedTriles, [36, 19, 40], winConditions: WC_BIT),
            new("Mausoleum Cube Bit 3", "MAUSOLEUM", LocationType.DestroyedTriles, [31, 19, 11], winConditions: WC_BIT),
            new("Mausoleum Cube Bit 4", "MAUSOLEUM", LocationType.DestroyedTriles, [5, 9, 2], winConditions: WC_BIT),
            new("Mine A Cube Bit", "MINE_A", LocationType.DestroyedTriles, [20, 43, 26], winConditions: WC_BIT),
            new("Mine Wrap Cube Bit 1", "MINE_WRAP", LocationType.DestroyedTriles, [31, 42, 18], winConditions: WC_BIT),
            new("Mine Wrap Cube Bit 2", "MINE_WRAP", LocationType.DestroyedTriles, [28, 37, 15], winConditions: WC_BIT),
            new("Nature Hub Cube Bit 1", "NATURE_HUB", LocationType.DestroyedTriles, [7, 25, 22], winConditions: WC_BIT),
            new("Nature Hub Cube Bit 2", "NATURE_HUB", LocationType.DestroyedTriles, [2, 16, 26], winConditions: WC_BIT),
            new("Nu Zu Abandoned A Cube Bit", "NUZU_ABANDONED_A", LocationType.DestroyedTriles, [8, 5, 8], winConditions: WC_BIT),
            new("Nu Zu Abandoned B Cube Bit", "NUZU_ABANDONED_B", LocationType.DestroyedTriles, [13, 2, 9], winConditions: WC_BIT),
            new("Oldschool Cube Bit", "OLDSCHOOL", LocationType.DestroyedTriles, [7, 5, 8], winConditions: WC_BIT),
            new("Oldschool Ruins Cube Bit", "OLDSCHOOL_RUINS", LocationType.DestroyedTriles, [7, 5, 8], winConditions: WC_BIT),
            new("Owl Cube Bit", "OWL", LocationType.DestroyedTriles, [18, 13, 10], winConditions: WC_BIT),
            new("Pivot 1 Cube Bit 1", "PIVOT_ONE", LocationType.DestroyedTriles, [10, 52, 46], winConditions: WC_BIT),
            new("Pivot 1 Cube Bit 2", "PIVOT_ONE", LocationType.DestroyedTriles, [30, 27, 36], winConditions: WC_BIT),
            new("Pivot 1 Cube Bit 3", "PIVOT_ONE", LocationType.DestroyedTriles, [27, 74, 33], winConditions: WC_BIT),
            new("Pivot 2 Cube Bit 1", "PIVOT_TWO", LocationType.DestroyedTriles, [24, 42, 17], winConditions: WC_BIT),
            new("Pivot 2 Cube Bit 2", "PIVOT_TWO", LocationType.DestroyedTriles, [16, 46, 9], winConditions: WC_BIT),
            new("Pivot 2 Cube Bit 3", "PIVOT_TWO", LocationType.DestroyedTriles, [32, 46, 25], winConditions: WC_BIT),
            new("Pivot Watertower Cube Bit", "PIVOT_WATERTOWER", LocationType.DestroyedTriles, [18, 34, 11], winConditions: WC_BIT),
            new("Purple Lodge Ruin Cube Bit", "PURPLE_LODGE_RUIN", LocationType.DestroyedTriles, [10, 4, 9], winConditions: WC_BIT),
            new("School Cube Bit", "SCHOOL", LocationType.DestroyedTriles, [7, 3, 7], winConditions: WC_BIT),
            new("Sewer Fork Cube Bit", "SEWER_FORK", LocationType.DestroyedTriles, [11, 42, 14], winConditions: WC_BIT),
            new("Sewer Geyser Cube Bit", "SEWER_GEYSER", LocationType.DestroyedTriles, [19, 37, 12], winConditions: WC_BIT),
            new("Sewer Hub Cube Bit 1", "SEWER_HUB", LocationType.DestroyedTriles, [34, 42, 35], winConditions: WC_BIT),
            new("Sewer Hub Cube Bit 2", "SEWER_HUB", LocationType.DestroyedTriles, [8, 36, 9], winConditions: WC_BIT),
            new("Sewer Pillars Cube Bit 1", "SEWER_PILLARS", LocationType.DestroyedTriles, [6, 24, 29], winConditions: WC_BIT),
            new("Sewer Pillars Cube Bit 2", "SEWER_PILLARS", LocationType.DestroyedTriles, [30, 34, 5], winConditions: WC_BIT),
            new("Sewer Pillars Cube Bit 3", "SEWER_PILLARS", LocationType.DestroyedTriles, [6, 16, 31], winConditions: WC_BIT),
            new("Sewer Pivot Cube Bit", "SEWER_PIVOT", LocationType.DestroyedTriles, [20, 34, 20], winConditions: WC_BIT),
            new("Sewer QR Cube Bit", "SEWER_QR", LocationType.DestroyedTriles, [15, 33, 11], winConditions: WC_BIT),
            new("Sewer Start Cube Bit", "SEWER_START", LocationType.DestroyedTriles, [13, 21, 37], winConditions: WC_BIT),
            new("Sewer to Lava Cube Bit", "SEWER_TO_LAVA", LocationType.DestroyedTriles, [14, 40, 10], winConditions: WC_BIT),
            new("Showers Cube Bit", "SHOWERS", LocationType.DestroyedTriles, [7, 2, 8], winConditions: WC_BIT),
            new("Skull Cube Bit 1", "SKULL", LocationType.DestroyedTriles, [8, 18, 10], winConditions: WC_BIT),
            new("Skull Cube Bit 2", "SKULL", LocationType.DestroyedTriles, [32, 22, 34], winConditions: WC_BIT),
            new("Skull Cube Bit 3", "SKULL", LocationType.DestroyedTriles, [22, 24, 21], winConditions: WC_BIT),
            new("Spinning Plates Cube Bit", "SPINNING_PLATES", LocationType.DestroyedTriles, [11, 40, 12], winConditions: WC_BIT),
            new("Stargate Ruins Cube Bit 1", "STARGATE_RUINS", LocationType.DestroyedTriles, [16, 75, 19], winConditions: WC_BIT),
            new("Stargate Ruins Cube Bit 2", "STARGATE_RUINS", LocationType.DestroyedTriles, [9, 75, 11], winConditions: WC_BIT),
            new("Throne Cube Bit", "THRONE", LocationType.DestroyedTriles, [30, 40, 28], winConditions: WC_BIT),
            new("Tree Crumble Cube Bit", "TREE_CRUMBLE", LocationType.DestroyedTriles, [23, 52, 23], winConditions: WC_BIT),
            new("Triple Pivot Cave Cube Bit 1", "TRIPLE_PIVOT_CAVE", LocationType.DestroyedTriles, [14, 46, 28], winConditions: WC_BIT),
            new("Triple Pivot Cave Cube Bit 2", "TRIPLE_PIVOT_CAVE", LocationType.DestroyedTriles, [26, 46, 30], winConditions: WC_BIT),
            new("Two Walls Cube Bit 1", "TWO_WALLS", LocationType.DestroyedTriles, [28, 20, 19], winConditions: WC_BIT),
            new("Two Walls Cube Bit 2", "TWO_WALLS", LocationType.DestroyedTriles, [23, 30, 29], winConditions: WC_BIT),
            new("Two Walls Cube Bit 3", "TWO_WALLS", LocationType.DestroyedTriles, [22, 33, 16], winConditions: WC_BIT),
            new("Villageville Cube Bit 1", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [34, 36, 30], winConditions: WC_BIT),
            new("Villageville Cube Bit 2", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [33, 53, 32], winConditions: WC_BIT),
            new("Villageville Cube Bit 3", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [31, 44, 31], winConditions: WC_BIT),
            new("Villageville Cube Bit 4", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [41, 23, 32], winConditions: WC_BIT),
            new("Wall Hole Cube Bit 1", "WALL_HOLE", LocationType.DestroyedTriles, [13, 23, 19], winConditions: WC_BIT),
            new("Wall Hole Cube Bit 2", "WALL_HOLE", LocationType.DestroyedTriles, [10, 6, 14], winConditions: WC_BIT),
            new("Wall Interior A Cube Bit", "WALL_INTERIOR_A", LocationType.DestroyedTriles, [10, 12, 10], winConditions: WC_BIT),
            new("Wall Kitchen Cube Bit", "WALL_KITCHEN", LocationType.DestroyedTriles, [10, 13, 14], winConditions: WC_BIT),
            new("Wall School Cube Bit", "WALL_SCHOOL", LocationType.DestroyedTriles, [10, 8, 10], winConditions: WC_BIT),
            new("Wall Village Cube Bit", "WALL_VILLAGE", LocationType.DestroyedTriles, [38, 35, 21], winConditions: WC_BIT),
            new("Waterfall Cube Bit", "WATERFALL", LocationType.DestroyedTriles, [6, 25, 5], winConditions: WC_BIT),
            new("Weightswitch Temple Cube Bit 1", "WEIGHTSWITCH_TEMPLE", LocationType.DestroyedTriles, [45, 36, 13], winConditions: WC_BIT),
            new("Weightswitch Temple Cube Bit 2", "WEIGHTSWITCH_TEMPLE", LocationType.DestroyedTriles, [21, 36, 34], winConditions: WC_BIT),
            new("Windmill Cave Cube Bit", "WINDMILL_CAVE", LocationType.DestroyedTriles, [16, 47, 14], winConditions: WC_BIT),
            new("Zu Bridge Cube Bit 1", "ZU_BRIDGE", LocationType.DestroyedTriles, [38, 57, 41], winConditions: WC_BIT),
            new("Zu Bridge Cube Bit 2", "ZU_BRIDGE", LocationType.DestroyedTriles, [21, 50, 7], winConditions: WC_BIT),
            new("Zu City Cube Bit 1", "ZU_CITY", LocationType.DestroyedTriles, [44, 59, 37], winConditions: WC_BIT),
            new("Zu City Cube Bit 2", "ZU_CITY", LocationType.DestroyedTriles, [45, 21, 34], winConditions: WC_BIT),
            new("Zu Code Loop Cube Bit 1", "ZU_CODE_LOOP", LocationType.DestroyedTriles, [2, 31, 5], winConditions: WC_BIT),
            new("Zu Code Loop Cube Bit 2", "ZU_CODE_LOOP", LocationType.DestroyedTriles, [2, 11, 2], winConditions: WC_BIT),
            new("Zu House Empty Cube Bit", "ZU_HOUSE_EMPTY", LocationType.DestroyedTriles, [9, 6, 8], winConditions: WC_BIT),
            new("Zu House Ruin Visitors Cube Bit", "ZU_HOUSE_RUIN_VISITORS", LocationType.DestroyedTriles, [6, 4, 10], winConditions: WC_BIT),
            new("Zu House Scaffolding Cube Bit", "ZU_HOUSE_SCAFFOLDING", LocationType.DestroyedTriles, [12, 9, 5], winConditions: WC_BIT),
            new("Zu Library Cube Bit", "ZU_LIBRARY", LocationType.DestroyedTriles, [27, 40, 25], winConditions: WC_BIT),
            new("Zu Throne Ruins Cube Bit", "ZU_THRONE_RUINS", LocationType.DestroyedTriles, [9, 6, 8], winConditions: WC_BIT),

            // Overlaps emplacement with one of the explodable walls
            new("Mine Bomb Pillar Cube Bit", "MINE_BOMB_PILLAR", LocationType.DestroyedTriles, [22, 55, 29], count: 2, winConditions: WC_BIT),
        ];

        // 16 total, 2 in chests
        private static readonly List<Location> goldenCubeLocations = [
            new("Clock Cube", "CLOCK", LocationType.DestroyedTriles, [41, 71, 35], winConditions: WC_GOLDEN),
            new("Five Towers Cube", "FIVE_TOWERS", LocationType.DestroyedTriles, [45, 74, 19], winConditions: WC_GOLDEN),
            new("Graveyard Lesser Gate Cube", "GRAVE_LESSER_GATE", LocationType.DestroyedTriles, [18, 18, 14], winConditions: WC_GOLDEN),
            new("Graveyard Treasure A Cube", "GRAVE_TREASURE_A", LocationType.DestroyedTriles, [10, 56, 8], winConditions: WC_GOLDEN),
            new("Mine Wrap Cube", "MINE_WRAP", LocationType.DestroyedTriles, [34, 68, 13], winConditions: WC_GOLDEN),
            new("Observatory Cube", "OBSERVATORY", LocationType.DestroyedTriles, [5, 44, 6], winConditions: WC_GOLDEN),
            new("Pivot 3 Cube", "PIVOT_THREE", LocationType.DestroyedTriles, [14, 66, 14], winConditions: WC_GOLDEN),
            new("Sewer Lesser Gate B Cube", "SEWER_LESSER_GATE_B", LocationType.DestroyedTriles, [15, 35, 20], winConditions: WC_GOLDEN),
            new("Spinning Plates Cube", "SPINNING_PLATES", LocationType.DestroyedTriles, [12, 60, 12], winConditions: WC_GOLDEN),
            new("Superspin Cave Cube", "SUPERSPIN_CAVE", LocationType.DestroyedTriles, [7, 22, 6], winConditions: WC_GOLDEN),
            new("Two Walls Cube", "TWO_WALLS", LocationType.DestroyedTriles, [22, 46, 32], winConditions: WC_GOLDEN),
            new("Visitor Cube", "VISITOR", LocationType.DestroyedTriles, [26, 54, 21], winConditions: WC_GOLDEN),
            new("Zu Code Loop Cube", "ZU_CODE_LOOP", LocationType.DestroyedTriles, [5, 42, 5], winConditions: WC_GOLDEN),
            new("Zu Switch B Cube", "ZU_SWITCH_B", LocationType.DestroyedTriles, [17, 37, 20], winConditions: WC_GOLDEN),
        ];

        // 32 total, only 6 don't have spawning conditions
        private static readonly List<Location> antiCubeLocations = [
            // Achievement
            new("Achievement Anti-Cube", "GOMEZ_HOUSE", LocationType.AchievementCode),

            // DestroyedTriles
            new("CMY Tune Fork Anti-Cube", "CMY_FORK", LocationType.InactiveArtObjectsAndDestroyedTriles, [7, 9, 5], index: 11, winConditions: WC_SECRET),
            new("Lava Tune Fork Anti-Cube", "LAVA_FORK", LocationType.InactiveArtObjectsAndDestroyedTriles, [16, 45, 16], index: 1, winConditions: WC_SECRET),
            new("Zu Tune Fork Anti-Cube", "ZU_FORK", LocationType.InactiveArtObjectsAndDestroyedTriles, [16, 18, 15], index: 29, winConditions: WC_SECRET),
            new("Lighthouse Floor Anti-Cube", "LIGHTHOUSE", LocationType.InactiveVolumesAndCollected, index: 18, winConditions: new() { SecretCount = 1, ScriptIds = [17] }),
            new("Tree Cabin Floor Anti-Cube", "TREE", LocationType.InactiveVolumesAndCollected, index: 14, winConditions: new() { SecretCount = 1, ScriptIds = [11] }),
            new("Tree Sky Floor Anti-Cube", "TREE_SKY", LocationType.InactiveVolumesAndCollected, index: 6, winConditions: new() { SecretCount = 1, ScriptIds = [6] }),
            new("Bell Tower Anti-Cube", "BELL_TOWER", LocationType.InactiveArtObjectsAndDestroyedTriles, [17, 44, 19], index: 0, winConditions: WC_SECRET),
            new("Telescope Anti-Cube", "TELESCOPE", LocationType.InactiveVolumesAndCollected, index: 4, notCollectedState: "ANC", winConditions: new() { SecretCount = 1, ScriptIds = [4] }),
            new("Zu Unfold Anti-Cube", "ZU_UNFOLD", LocationType.InactiveArtObjectsAndDestroyedTriles, [9, 59, 12], index: 0, winConditions: WC_SECRET),
            new("Code Machine Anti-Cube", "CODE_MACHINE", LocationType.InactiveVolumesAndCollected, index: 2, winConditions: new() { SecretCount = 1, ScriptIds = [4] }),
            new("Boileroom Anti-Cube", "BOILEROOM", LocationType.InactiveVolumesAndCollected, index: 0, winConditions: new() { SecretCount = 1, ScriptIds = [4] }),
            new("Nu Zu School Anti-Cube", "NUZU_SCHOOL", LocationType.InactiveVolumesAndCollected, index: 0, winConditions: new() { SecretCount = 1, ScriptIds = [5] }),
            new("Big Owl Anti-Cube", "BIG_OWL", LocationType.DestroyedTriles, [18, 29, 14], winConditions: WC_OTHER),
            new("CMY B Anti-Cube", "CMY_B", LocationType.DestroyedTriles, [14, 62, 11], winConditions: WC_OTHER),
            new("Zu Tetris Anti-Cube", "ZU_TETRIS", LocationType.InactiveArtObjectsAndDestroyedTriles, [14, 20, 13], index: 0, winConditions: WC_SECRET),
            new("Lava Skull Anti-Cube", "LAVA_SKULL", LocationType.DestroyedTriles, [10, 30, 8], winConditions: WC_OTHER),
            new("Quantum Anti-Cube", "QUANTUM", LocationType.DestroyedTriles, [44, 83, 38], winConditions: WC_OTHER),
            new("Skull B Anti-Cube", "SKULL_B", LocationType.DestroyedTriles, [20, 21, 19], winConditions: WC_OTHER),
            new("Zu Heads Anti-Cube", "ZU_HEADS", LocationType.DestroyedTriles, [9, 68, 9], winConditions: WC_OTHER),
            new("Sewer Tune Fork Anti-Cube", "SEWER_FORK", LocationType.InactiveArtObjectsAndDestroyedTriles, [11, 41, 14], index: 1, winConditions: WC_SECRET),
            new("Zu Bridge Floor Anti-Cube", "ZU_BRIDGE", LocationType.InactiveVolumesAndCollected, index: 2, winConditions: new() { SecretCount = 1, ScriptIds = [5] }),
            new("Zu Code Loop Anti-Cube", "ZU_CODE_LOOP", LocationType.InactiveVolumesAndCollected, index: 2, winConditions: new() { SecretCount = 1, ScriptIds = [4] }),

            // Special case shared cubes which can be collected in multiple locations
            new("Parlor Anti-Cube", "GOMEZ_HOUSE", LocationType.SharedParlorZuQr),
            new("Throne Anti-Cube", "GOMEZ_HOUSE", LocationType.SharedThroneSewerQr),
            new("Watertower Secret Anti-Cube", "GOMEZ_HOUSE", LocationType.SharedWatertowerMapQr),

            // Use InactiveArtObjects since DestroyedTriles doesn't work for these since they're all [0, 0, 0]
            new("Clock Tower Minute Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 53, winConditions: WC_SECRET),
            new("Clock Tower Hour Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 54, winConditions: WC_SECRET),
            new("Clock Tower Day Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 55, winConditions: WC_SECRET),
            new("Clock Tower Week Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 56, winConditions: WC_SECRET),
        ];

        private static readonly List<Location> heartCubeLocations = [
            new("Black Monolith Heart Cube", "RITUAL", LocationType.InactiveArtObjectsAndCollected, index: -11, winConditions: new() { SecretCount = 2, ScriptIds = [5] }), // I can't figure out where -11 is coming from in the game's data files the levelinfo command is showing it and it works??
            new("Telescope Heart Cube", "TELESCOPE", LocationType.InactiveVolumesAndCollected, index: 1, notCollectedState: "HNC", winConditions: new() { SecretCount = 1, ScriptIds = [7] }),
            new("Security Question Heart Cube", "ZU_ZUISH", LocationType.InactiveArtObjectsAndDestroyedTriles, [13, 59, 15], index: 0, winConditions: WC_SECRET),
        ];

        // 24 total
        private static readonly List<Location> chestLocations = [
            new("Arch Chest 1", "ARCH", LocationType.InactiveArtObjects, index: 12, winConditions: WC_CHEST),
            new("Arch Chest 2", "ARCH", LocationType.InactiveArtObjects, index: 62, winConditions: WC_CHEST),
            new("Five Towers Cave Chest", "FIVE_TOWERS_CAVE", LocationType.InactiveArtObjects, index: 18, winConditions: WC_CHEST),
            new("Fox Chest", "FOX", LocationType.InactiveArtObjects, index: 10, winConditions: WC_CHEST),
            new("Globe Interior Chest", "GLOBE_INT", LocationType.InactiveArtObjects, index: 5, winConditions: WC_CHEST),
            new("Industrial Superspin Chest", "INDUSTRIAL_SUPERSPIN", LocationType.InactiveArtObjects, index: 144, winConditions: WC_CHEST),
            new("Lighthouse House A Chest", "LIGHTHOUSE_HOUSE_A", LocationType.InactiveArtObjects, index: 6, winConditions: WC_CHEST),
            new("Mausoleum Chest", "MAUSOLEUM", LocationType.InactiveArtObjects, index: 43, winConditions: WC_CHEST),
            new("Mine Bomb Pillar Chest", "MINE_BOMB_PILLAR", LocationType.InactiveArtObjects, index: 2, winConditions: WC_CHEST),
            new("Orrery B Chest", "ORRERY_B", LocationType.InactiveArtObjects, index: 25, winConditions: WC_CHEST),
            new("Parlor Chest", "PARLOR", LocationType.InactiveArtObjects, index: 5, winConditions: WC_CHEST),
            new("Pivot Watertower Chest", "PIVOT_WATERTOWER", LocationType.InactiveArtObjects, index: 8, winConditions: WC_CHEST),
            new("Sewer Pivot Chest", "SEWER_PIVOT", LocationType.InactiveArtObjects, index: 13, winConditions: WC_CHEST),
            new("Sewer Treasure 1 Chest", "SEWER_TREASURE_ONE", LocationType.InactiveArtObjects, index: 1, winConditions: WC_CHEST),
            new("Sewer Treasure 2 Chest", "SEWER_TREASURE_TWO", LocationType.InactiveArtObjects, index: 2, winConditions: WC_CHEST),
            new("Tree Crumble Chest", "TREE_CRUMBLE", LocationType.InactiveArtObjects, index: 17, winConditions: WC_CHEST),
            new("Tree of Death Chest", "TREE_OF_DEATH", LocationType.InactiveArtObjects, index: 1, winConditions: WC_CHEST),
            new("Tree Sky Chest", "TREE_SKY", LocationType.InactiveArtObjects, index: 14, winConditions: WC_CHEST),
            new("Villageville Chest", "VILLAGEVILLE_3D", LocationType.InactiveArtObjects, index: 26, winConditions: WC_CHEST),
            new("Wall Hole Chest", "WALL_HOLE", LocationType.InactiveArtObjects, index: 0, winConditions: WC_CHEST),
            new("Water Wheel B Chest", "WATER_WHEEL_B", LocationType.InactiveArtObjects, index: 23, winConditions: WC_CHEST),
            new("Windmill Cave Chest", "WINDMILL_CAVE", LocationType.InactiveArtObjects, index: 15, winConditions: WC_CHEST),
            new("Zu City Ruins Chest", "ZU_CITY_RUINS", LocationType.InactiveArtObjects, index: 121, winConditions: WC_CHEST),
            new("Zu House Empty B Chest", "ZU_HOUSE_EMPTY_B", LocationType.InactiveArtObjects, index: 10, winConditions: WC_CHEST),
        ];

        // 4 total
        private static readonly List<Location> owlLocations = [
            new("Waterfall Owl", "WATERFALL", LocationType.InactiveNPCs, index: 0, winConditions: WC_SECRET),
            new("Visitor Owl", "VISITOR", LocationType.InactiveNPCs, index: 0, winConditions: WC_SECRET),
            new("Pivot 1 Owl", "PIVOT_ONE", LocationType.InactiveNPCs, index: 0, winConditions: WC_SECRET),
            new("Tree Owl", "TREE", LocationType.InactiveNPCs, index: 6, winConditions: WC_SECRET),
        ];

        // Misc locations
        private static readonly List<Location> miscLocations = [
            new("Boileroom Map", "BOILEROOM", LocationType.InactiveArtObjects, index: 7, winConditions: WC_OTHER),
        ];

        public static readonly List<Location> allLocations = [.. cubeBitLocations,
                                                              .. goldenCubeLocations,
                                                              .. antiCubeLocations,
                                                              .. heartCubeLocations,
                                                              .. chestLocations,
                                                              .. owlLocations,
                                                              .. miscLocations];
    }
}
