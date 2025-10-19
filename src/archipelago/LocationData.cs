using FezEngine.Structure;

namespace FEZAP.Archipelago
{
    public enum LocationType
    {
        DestroyedTriles,  // bits, most golden cubes, most anti-cubes
        InactiveArtObjects,  // chests, clock anti-cubes
        InactiveNPCs,  // owls
        AchievementCode,  // for achievement code andit-cube
    }

    /// Location information container
    public readonly struct Location(string name, string levelName, LocationType type, List<int> emplacement = null, int count = 1, int index = 0)
    {
        public readonly string name = name;  // apworld location name
        public readonly string levelName = levelName;  // name of the containing fezlvl
        public readonly LocationType type = type;  // type of location (needed for different handling)
        public readonly TrileEmplacement emplacement = (emplacement != null) ?
                                                       new(emplacement[0], emplacement[1], emplacement[2]) :
                                                       new(0, 0, 0);  // for DestroyedTriles
        public readonly int count = count;  // for overlaping DestroyedTriles
        public readonly int index = index;  // for InactiveArtObjects and Inactive NPCs
    };

    public class LocationData
    {
        // 128 cube bits, usually converts to 8 full cubes
        private static readonly List<Location> cubeBitLocations = [
            new("Abandoned A Cube Bit", "ABANDONED_A", LocationType.DestroyedTriles, [11, 9, 10]),
            new("Abandoned B Cube Bit", "ABANDONED_B", LocationType.DestroyedTriles, [7, 3, 10]),
            new("Ancient Walls Cube Bit 1", "ANCIENT_WALLS", LocationType.DestroyedTriles, [37, 35, 3]),
            new("Ancient Walls Cube Bit 2", "ANCIENT_WALLS", LocationType.DestroyedTriles, [32, 18, 25]),
            new("Ancient Walls Cube Bit 3", "ANCIENT_WALLS", LocationType.DestroyedTriles, [38, 25, 5]),
            new("Arch Cube Bit 1", "ARCH", LocationType.DestroyedTriles, [26, 36, 9]),
            new("Arch Cube Bit 2", "ARCH", LocationType.DestroyedTriles, [18, 33, 4]),
            new("Arch Cube Bit 3", "ARCH", LocationType.DestroyedTriles, [19, 37, 15]),
            new("Bell Tower Cube Bit 1", "BELL_TOWER", LocationType.DestroyedTriles, [15, 32, 21]),
            new("Bell Tower Cube Bit 2", "BELL_TOWER", LocationType.DestroyedTriles, [16, 31, 16]),
            new("Big Owl Cube Bit", "BIG_OWL", LocationType.DestroyedTriles, [18, 13, 10]),
            new("Big Tower Cube Bit 1", "BIG_TOWER", LocationType.DestroyedTriles, [13, 61, 10]),
            new("Big Tower Cube Bit 2", "BIG_TOWER", LocationType.DestroyedTriles, [10, 63, 32]),
            new("Big Tower Cube Bit 3", "BIG_TOWER", LocationType.DestroyedTriles, [24, 53, 25]),
            new("Big Tower Cube Bit 4", "BIG_TOWER", LocationType.DestroyedTriles, [28, 41, 21]),
            new("Big Tower Cube Bit 5", "BIG_TOWER", LocationType.DestroyedTriles, [24, 45, 16]),
            new("Big Tower Cube Bit 6", "BIG_TOWER", LocationType.DestroyedTriles, [19, 49, 21]),
            new("Big Tower Cube Bit 7", "BIG_TOWER", LocationType.DestroyedTriles, [33, 57, 30]),
            new("Big Tower Cube Bit 8", "BIG_TOWER", LocationType.DestroyedTriles, [37, 59, 6]),
            new("Extractor A Cube Bit", "EXTRACTOR_A", LocationType.DestroyedTriles, [24, 15, 12]),
            new("Five Towers Cube Bit 1", "FIVE_TOWERS", LocationType.DestroyedTriles, [32, 34, 27]),
            new("Five Towers Cube Bit 2", "FIVE_TOWERS", LocationType.DestroyedTriles, [26, 38, 21]),
            new("Five Towers Cube Bit 3", "FIVE_TOWERS", LocationType.DestroyedTriles, [45, 58, 32]),
            new("Fox Cube Bit", "FOX", LocationType.DestroyedTriles, [19, 74, 9]),
            new("Fractal Cube Bit 1", "FRACTAL", LocationType.DestroyedTriles, [28, 48, 21]),
            new("Fractal Cube Bit 2", "FRACTAL", LocationType.DestroyedTriles, [33, 26, 19]),
            new("Fractal Cube Bit 3", "FRACTAL", LocationType.DestroyedTriles, [30, 26, 23]),
            new("Globe Cube Bit", "GLOBE", LocationType.DestroyedTriles, [23, 33, 19]),
            new("Graveyard A Cube Bit 1", "GRAVEYARD_A", LocationType.DestroyedTriles, [22, 39, 18]),
            new("Graveyard A Cube Bit 2", "GRAVEYARD_A", LocationType.DestroyedTriles, [15, 54, 34]),
            new("Graveyard A Cube Bit 3", "GRAVEYARD_A", LocationType.DestroyedTriles, [37, 73, 13]),
            new("Graveyard Gate Cube Bit 1", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [31, 30, 17]),
            new("Graveyard Gate Cube Bit 2", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [22, 25, 26]),
            new("Graveyard Gate Cube Bit 3", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [28, 19, 13]),
            new("Graveyard Gate Cube Bit 4", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [18, 29, 17]),
            new("Graveyard Gate Cube Bit 5", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [30, 40, 14]),
            new("Graveyard Gate Cube Bit 6", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [19, 42, 25]),
            new("Graveyard Gate Cube Bit 7", "GRAVEYARD_GATE", LocationType.DestroyedTriles, [25, 61, 20]),
            new("Graveyard Cabin Cube Bit 1", "GRAVE_CABIN", LocationType.DestroyedTriles, [12, 19, 36]),
            new("Graveyard Cabin Cube Bit 2", "GRAVE_CABIN", LocationType.DestroyedTriles, [13, 27, 37]),
            new("Graveyard Ghost Cube Bit 1", "GRAVE_GHOST", LocationType.DestroyedTriles, [15, 21, 12]),
            new("Graveyard Ghost Cube Bit 2", "GRAVE_GHOST", LocationType.DestroyedTriles, [9, 8, 15]),
            new("Graveyard Lesser Gate Cube Bit", "GRAVE_LESSER_GATE", LocationType.DestroyedTriles, [18, 12, 10]),
            new("Graveyard Treasure A Cube Bit 1", "GRAVE_TREASURE_A", LocationType.DestroyedTriles, [7, 45, 8]),
            new("Graveyard Treasure A Cube Bit 2", "GRAVE_TREASURE_A", LocationType.DestroyedTriles, [10, 40, 5]),
            new("Industrial Superspin Cube Bit 1", "INDUSTRIAL_SUPERSPIN", LocationType.DestroyedTriles, [14, 71, 58]),
            new("Industrial Superspin Cube Bit 2", "INDUSTRIAL_SUPERSPIN", LocationType.DestroyedTriles, [27, 60, 40]),
            new("Industrial Superspin Cube Bit 3", "INDUSTRIAL_SUPERSPIN", LocationType.DestroyedTriles, [29, 134, 9]),
            new("Industrial Abandoned A Cube Bit", "INDUST_ABANDONED_A", LocationType.DestroyedTriles, [7, 11, 8]),
            new("Kitchen Cube Bit", "KITCHEN", LocationType.DestroyedTriles, [7, 12, 13]),
            new("Lighthouse Cube Bit 1", "LIGHTHOUSE", LocationType.DestroyedTriles, [23, 39, 32]),
            new("Lighthouse Cube Bit 2", "LIGHTHOUSE", LocationType.DestroyedTriles, [8, 28, 45]),
            new("Lighthouse House A Cube Bit", "LIGHTHOUSE_HOUSE_A", LocationType.DestroyedTriles, [13, 7, 4]),
            new("Lighthouse Spin Cube Bit", "LIGHTHOUSE_SPIN", LocationType.DestroyedTriles, [10, 59, 15]),
            new("Mausoleum Cube Bit 1", "MAUSOLEUM", LocationType.DestroyedTriles, [26, 19, 26]),
            new("Mausoleum Cube Bit 2", "MAUSOLEUM", LocationType.DestroyedTriles, [36, 19, 40]),
            new("Mausoleum Cube Bit 3", "MAUSOLEUM", LocationType.DestroyedTriles, [31, 19, 11]),
            new("Mausoleum Cube Bit 4", "MAUSOLEUM", LocationType.DestroyedTriles, [5, 9, 2]),
            new("Mine A Cube Bit", "MINE_A", LocationType.DestroyedTriles, [20, 43, 26]),
            new("Mine Wrap Cube Bit 1", "MINE_WRAP", LocationType.DestroyedTriles, [31, 42, 18]),
            new("Mine Wrap Cube Bit 2", "MINE_WRAP", LocationType.DestroyedTriles, [28, 37, 15]),
            new("Nature Hub Cube Bit 1", "NATURE_HUB", LocationType.DestroyedTriles, [7, 25, 22]),
            new("Nature Hub Cube Bit 2", "NATURE_HUB", LocationType.DestroyedTriles, [2, 16, 26]),
            new("Nu Zu Abandoned A Cube Bit", "NUZU_ABANDONED_A", LocationType.DestroyedTriles, [8, 5, 8]),
            new("Nu Zu Abandoned B Cube Bit", "NUZU_ABANDONED_B", LocationType.DestroyedTriles, [13, 2, 9]),
            new("Oldschool Cube Bit", "OLDSCHOOL", LocationType.DestroyedTriles, [7, 5, 8]),
            new("Oldschool Ruins Cube Bit", "OLDSCHOOL_RUINS", LocationType.DestroyedTriles, [7, 5, 8]),
            new("Owl Cube Bit", "OWL", LocationType.DestroyedTriles, [18, 13, 10]),
            new("Pivot 1 Cube Bit 1", "PIVOT_ONE", LocationType.DestroyedTriles, [10, 52, 46]),
            new("Pivot 1 Cube Bit 2", "PIVOT_ONE", LocationType.DestroyedTriles, [30, 27, 36]),
            new("Pivot 1 Cube Bit 3", "PIVOT_ONE", LocationType.DestroyedTriles, [27, 74, 33]),
            new("Pivot 2 Cube Bit 1", "PIVOT_TWO", LocationType.DestroyedTriles, [24, 42, 17]),
            new("Pivot 2 Cube Bit 2", "PIVOT_TWO", LocationType.DestroyedTriles, [16, 46, 9]),
            new("Pivot 2 Cube Bit 3", "PIVOT_TWO", LocationType.DestroyedTriles, [32, 46, 25]),
            new("Pivot Watertower Cube Bit", "PIVOT_WATERTOWER", LocationType.DestroyedTriles, [18, 34, 11]),
            new("Purple Lodge Ruin Cube Bit", "PURPLE_LODGE_RUIN", LocationType.DestroyedTriles, [10, 4, 9]),
            new("School Cube Bit", "SCHOOL", LocationType.DestroyedTriles, [7, 3, 7]),
            new("Sewer Fork Cube Bit", "SEWER_FORK", LocationType.DestroyedTriles, [11, 42, 14]),
            new("Sewer Geyser Cube Bit", "SEWER_GEYSER", LocationType.DestroyedTriles, [19, 37, 12]),
            new("Sewer Hub Cube Bit 1", "SEWER_HUB", LocationType.DestroyedTriles, [34, 42, 35]),
            new("Sewer Hub Cube Bit 2", "SEWER_HUB", LocationType.DestroyedTriles, [8, 36, 9]),
            new("Sewer Pillars Cube Bit 1", "SEWER_PILLARS", LocationType.DestroyedTriles, [6, 24, 29]),
            new("Sewer Pillars Cube Bit 2", "SEWER_PILLARS", LocationType.DestroyedTriles, [30, 34, 5]),
            new("Sewer Pillars Cube Bit 3", "SEWER_PILLARS", LocationType.DestroyedTriles, [6, 16, 31]),
            new("Sewer Pivot Cube Bit", "SEWER_PIVOT", LocationType.DestroyedTriles, [20, 34, 20]),
            new("Sewer QR Cube Bit", "SEWER_QR", LocationType.DestroyedTriles, [15, 33, 11]),
            new("Sewer Start Cube Bit", "SEWER_START", LocationType.DestroyedTriles, [13, 21, 37]),
            new("Sewer to Lava Cube Bit", "SEWER_TO_LAVA", LocationType.DestroyedTriles, [14, 40, 10]),
            new("Showers Cube Bit", "SHOWERS", LocationType.DestroyedTriles, [7, 2, 8]),
            new("Skull Cube Bit 1", "SKULL", LocationType.DestroyedTriles, [8, 18, 10]),
            new("Skull Cube Bit 2", "SKULL", LocationType.DestroyedTriles, [32, 22, 34]),
            new("Skull Cube Bit 3", "SKULL", LocationType.DestroyedTriles, [22, 24, 21]),
            new("Spinning Plates Cube Bit", "SPINNING_PLATES", LocationType.DestroyedTriles, [11, 40, 12]),
            new("Stargate Ruins Cube Bit 1", "STARGATE_RUINS", LocationType.DestroyedTriles, [16, 75, 19]),
            new("Stargate Ruins Cube Bit 2", "STARGATE_RUINS", LocationType.DestroyedTriles, [9, 75, 11]),
            new("Throne Cube Bit", "THRONE", LocationType.DestroyedTriles, [30, 40, 28]),
            new("Tree Crumble Cube Bit", "TREE_CRUMBLE", LocationType.DestroyedTriles, [23, 52, 23]),
            new("Triple Pivot Cave Cube Bit 1", "TRIPLE_PIVOT_CAVE", LocationType.DestroyedTriles, [14, 46, 28]),
            new("Triple Pivot Cave Cube Bit 2", "TRIPLE_PIVOT_CAVE", LocationType.DestroyedTriles, [26, 46, 30]),
            new("Two Walls Cube Bit 1", "TWO_WALLS", LocationType.DestroyedTriles, [28, 20, 19]),
            new("Two Walls Cube Bit 2", "TWO_WALLS", LocationType.DestroyedTriles, [23, 30, 29]),
            new("Two Walls Cube Bit 3", "TWO_WALLS", LocationType.DestroyedTriles, [22, 33, 16]),
            new("Villageville Cube Bit 1", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [34, 36, 30]),
            new("Villageville Cube Bit 2", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [33, 53, 32]),
            new("Villageville Cube Bit 3", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [31, 44, 31]),
            new("Villageville Cube Bit 4", "VILLAGEVILLE_3D", LocationType.DestroyedTriles, [41, 23, 32]),
            new("Wall Hole Cube Bit 1", "WALL_HOLE", LocationType.DestroyedTriles, [13, 23, 19]),
            new("Wall Hole Cube Bit 2", "WALL_HOLE", LocationType.DestroyedTriles, [10, 6, 14]),
            new("Wall Interior A Cube Bit", "WALL_INTERIOR_A", LocationType.DestroyedTriles, [10, 12, 10]),
            new("Wall Kitchen Cube Bit", "WALL_KITCHEN", LocationType.DestroyedTriles, [10, 13, 14]),
            new("Wall School Cube Bit", "WALL_SCHOOL", LocationType.DestroyedTriles, [10, 8, 10]),
            new("Wall Village Cube Bit", "WALL_VILLAGE", LocationType.DestroyedTriles, [38, 35, 21]),
            new("Waterfall Cube Bit", "WATERFALL", LocationType.DestroyedTriles, [6, 25, 5]),
            new("Weightswitch Temple Cube Bit 1", "WEIGHTSWITCH_TEMPLE", LocationType.DestroyedTriles, [45, 36, 13]),
            new("Weightswitch Temple Cube Bit 2", "WEIGHTSWITCH_TEMPLE", LocationType.DestroyedTriles, [21, 36, 34]),
            new("Windmill Cave Cube Bit", "WINDMILL_CAVE", LocationType.DestroyedTriles, [16, 47, 14]),
            new("Zu Bridge Cube Bit 1", "ZU_BRIDGE", LocationType.DestroyedTriles, [38, 57, 41]),
            new("Zu Bridge Cube Bit 2", "ZU_BRIDGE", LocationType.DestroyedTriles, [21, 50, 7]),
            new("Zu City Cube Bit 1", "ZU_CITY", LocationType.DestroyedTriles, [44, 59, 37]),
            new("Zu City Cube Bit 2", "ZU_CITY", LocationType.DestroyedTriles, [45, 21, 34]),
            new("Zu Code Loop Cube Bit 1", "ZU_CODE_LOOP", LocationType.DestroyedTriles, [2, 31, 5]),
            new("Zu Code Loop Cube Bit 2", "ZU_CODE_LOOP", LocationType.DestroyedTriles, [2, 11, 2]),
            new("Zu House Empty Cube Bit", "ZU_HOUSE_EMPTY", LocationType.DestroyedTriles, [9, 6, 8]),
            new("Zu House Ruin Visitors Cube Bit", "ZU_HOUSE_RUIN_VISITORS", LocationType.DestroyedTriles, [6, 4, 10]),
            new("Zu House Scaffolding Cube Bit", "ZU_HOUSE_SCAFFOLDING", LocationType.DestroyedTriles, [12, 9, 5]),
            new("Zu Library Cube Bit", "ZU_LIBRARY", LocationType.DestroyedTriles, [27, 40, 25]),
            new("Zu Throne Ruins Cube Bit", "ZU_THRONE_RUINS", LocationType.DestroyedTriles, [9, 6, 8]),

            // Overlaps emplacement with one of the explodable walls
            new("Mine Bomb Pillar Cube Bit", "MINE_BOMB_PILLAR", LocationType.DestroyedTriles, [22, 55, 29], count: 2),
        ];

        // 16 total, 2 in chests
        private static readonly List<Location> goldenCubeLocations = [
            new("Clock Cube", "CLOCK", LocationType.DestroyedTriles, [41, 71, 35]),
            new("Five Towers Cube", "FIVE_TOWERS", LocationType.DestroyedTriles, [45, 74, 19]),
            new("Graveyard Lesser Gate Cube", "GRAVE_LESSER_GATE", LocationType.DestroyedTriles, [18, 18, 14]),
            new("Graveyard Treasure A Cube", "GRAVE_TREASURE_A", LocationType.DestroyedTriles, [10, 56, 8]),
            new("Mine Wrap Cube", "MINE_WRAP", LocationType.DestroyedTriles, [34, 68, 13]),
            new("Observatory Cube", "OBSERVATORY", LocationType.DestroyedTriles, [5, 44, 6]),
            new("Pivot 3 Cube", "PIVOT_THREE", LocationType.DestroyedTriles, [14, 66, 14]),
            new("Sewer Lesser Gate B Cube", "SEWER_LESSER_GATE_B", LocationType.DestroyedTriles, [15, 35, 20]),
            new("Spinning Plates Cube", "SPINNING_PLATES", LocationType.DestroyedTriles, [12, 60, 12]),
            new("Superspin Cave Cube", "SUPERSPIN_CAVE", LocationType.DestroyedTriles, [7, 22, 6]),
            new("Two Walls Cube", "TWO_WALLS", LocationType.DestroyedTriles, [22, 46, 32]),
            new("Visitor Cube", "VISITOR", LocationType.DestroyedTriles, [26, 54, 21]),
            new("Zu Code Loop Cube", "ZU_CODE_LOOP", LocationType.DestroyedTriles, [5, 42, 5]),
            new("Zu Switch B Cube", "ZU_SWITCH_B", LocationType.DestroyedTriles, [17, 37, 20]),
        ];

        // 32 total, only 6 don't have spawning conditions
        private static readonly List<Location> antiCubeLocations = [
            // Achievement
            new("Achievement Anti-Cube", "GOMEZ_HOUSE", LocationType.AchievementCode),

            // DestroyedTriles
            new("CMY Tune Fork Anti-Cube", "CMY_FORK", LocationType.DestroyedTriles, [7, 9, 5]),
            new("Lava Tune Fork Anti-Cube", "LAVA_FORK", LocationType.DestroyedTriles, [16, 45, 16]),
            new("Zu Tune Fork Anti-Cube", "ZU_FORK", LocationType.DestroyedTriles, [16, 18, 15]),
            new("Lighthouse Floor Anti-Cube", "LIGHTHOUSE", LocationType.DestroyedTriles, [46, 25, 9]),
            new("Tree Cabin Floor Anti-Cube", "TREE", LocationType.DestroyedTriles, [44, 60, 4]),
            new("Tree Sky Floor Anti-Cube", "TREE_SKY", LocationType.DestroyedTriles, [18, 50, 19]),
            new("Bell Tower Anti-Cube", "BELL_TOWER", LocationType.DestroyedTriles, [17, 44, 19]),
            new("Watertower Owl Anti-Cube", "WATERTOWER_SECRET", LocationType.DestroyedTriles, [9, 16, 12]),
            new("Telescope Anti-Cube", "TELESCOPE", LocationType.DestroyedTriles, [18, 36, 20]),
            new("Zu House QR Anti-Cube", "ZU_HOUSE_QR", LocationType.DestroyedTriles, [9, 6, 8]),
            new("Zu Unfold Anti-Cube", "ZU_UNFOLD", LocationType.DestroyedTriles, [9, 59, 12]),
            new("Code Machine Anti-Cube", "CODE_MACHINE", LocationType.DestroyedTriles, [35, 40, 10]),
            new("Boileroom Anti-Cube", "BOILEROOM", LocationType.DestroyedTriles, [10, 10, 12]),
            new("Nu Zu School Anti-Cube", "NUZU_SCHOOL", LocationType.DestroyedTriles, [5, 5, 5]),
            new("Big Owl Anti-Cube", "BIG_OWL", LocationType.DestroyedTriles, [18, 29, 14]),
            new("CMY B Anti-Cube", "CMY_B", LocationType.DestroyedTriles, [14, 62, 11]),
            new("Zu Tetris Anti-Cube", "ZU_TETRIS", LocationType.DestroyedTriles, [14, 20, 13]),
            new("Lava Skull Anti-Cube", "LAVA_SKULL", LocationType.DestroyedTriles, [10, 30, 8]),
            new("Quantum Anti-Cube", "QUANTUM", LocationType.DestroyedTriles, [44, 83, 38]),
            new("Skull B Anti-Cube", "SKULL_B", LocationType.DestroyedTriles, [20, 21, 19]),
            new("Zu Heads Anti-Cube", "ZU_HEADS", LocationType.DestroyedTriles, [9, 68, 9]),
            new("Parlor Anti-Cube", "PARLOR", LocationType.DestroyedTriles, [18, 4, 5]),
            new("Sewer Tune Fork Anti-Cube", "SEWER_FORK", LocationType.DestroyedTriles, [11, 41, 14]),

            // Use count due to overlap with other locations
            new("Zu Bridge Floor Anti-Cube", "ZU_BRIDGE", LocationType.DestroyedTriles, [38, 57, 41], count: 2),
            new("Zu Code Loop Anti-Cube", "ZU_CODE_LOOP", LocationType.DestroyedTriles, [5, 42, 5], count: 2),

            // Throne cube accessible in 3 locations
            new("Throne Anti-Cube", "SEWER_QR", LocationType.DestroyedTriles, [15, 41, 14]),
            new("Throne Anti-Cube", "ZU_HOUSE_EMPTY", LocationType.DestroyedTriles, [9, 6, 5]),
            new("Throne Anti-Cube", "ZU_THRONE_RUINS", LocationType.DestroyedTriles, [9, 5, 5]),

            // Use InactiveArtObjects since DestroyedTriles doesn't work for these since they're all [0, 0, 0]
            new("Clock Tower Minute Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 53),
            new("Clock Tower Hour Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 54),
            new("Clock Tower Day Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 55),
            new("Clock Tower Week Anti-Cube", "CLOCK", LocationType.InactiveArtObjects, index: 56),
        ];

        private static readonly List<Location> heartCubeLocations = [
            new("Black Monolith Heart Cube", "RITUAL", LocationType.DestroyedTriles, [8, 61, 8]),
            new("Telescope Heart Cube", "TELESCOPE", LocationType.DestroyedTriles, [21, 36, 20]),
            new("Security Question Heart Cube", "ZU_ZUISH", LocationType.DestroyedTriles, [13, 59, 15]),
        ];

        // 24 total
        private static readonly List<Location> chestLocations = [
            new("Arch Chest 1", "ARCH", LocationType.InactiveArtObjects, index: 12),
            new("Arch Chest 2", "ARCH", LocationType.InactiveArtObjects, index: 62),
            new("Five Towers Cave Chest", "FIVE_TOWERS_CAVE", LocationType.InactiveArtObjects, index: 18),
            new("Fox Chest", "FOX", LocationType.InactiveArtObjects, index: 10),
            new("Globe Interior Chest", "GLOBE_INT", LocationType.InactiveArtObjects, index: 5),
            new("Industrial Superspin Chest", "INDUSTRIAL_SUPERSPIN", LocationType.InactiveArtObjects, index: 144),
            new("Lighthouse House A Chest", "LIGHTHOUSE_HOUSE_A", LocationType.InactiveArtObjects, index: 6),
            new("Mausoleum Chest", "MAUSOLEUM", LocationType.InactiveArtObjects, index: 43),
            new("Mine Bomb Pillar Chest", "MINE_BOMB_PILLAR", LocationType.InactiveArtObjects, index: 2),
            new("Orrery B Chest", "ORRERY_B", LocationType.InactiveArtObjects, index: 25),
            new("Parlor Chest", "PARLOR", LocationType.InactiveArtObjects, index: 5),
            new("Pivot Watertower Chest", "PIVOT_WATERTOWER", LocationType.InactiveArtObjects, index: 8),
            new("Sewer Pivot Chest", "SEWER_PIVOT", LocationType.InactiveArtObjects, index: 13),
            new("Sewer Treasure 1 Chest", "SEWER_TREASURE_ONE", LocationType.InactiveArtObjects, index: 1),
            new("Sewer Treasure 2 Chest", "SEWER_TREASURE_TWO", LocationType.InactiveArtObjects, index: 2),
            new("Tree Crumble Chest", "TREE_CRUMBLE", LocationType.InactiveArtObjects, index: 17),
            new("Tree of Death Chest", "TREE_OF_DEATH", LocationType.InactiveArtObjects, index: 1),
            new("Tree Sky Chest", "TREE_SKY", LocationType.InactiveArtObjects, index: 14),
            new("Villageville Chest", "VILLAGEVILLE_3D", LocationType.InactiveArtObjects, index: 26),
            new("Wall Hole Chest", "WALL_HOLE", LocationType.InactiveArtObjects, index: 0),
            new("Water Wheel B Chest", "WATER_WHEEL_B", LocationType.InactiveArtObjects, index: 23),
            new("Windmill Cave Chest", "WINDMILL_CAVE", LocationType.InactiveArtObjects, index: 15),
            new("Zu City Ruins Chest", "ZU_CITY_RUINS", LocationType.InactiveArtObjects, index: 121),
            new("Zu House Empty B Chest", "ZU_HOUSE_EMPTY_B", LocationType.InactiveArtObjects, index: 10),
        ];

        // 4 total
        private static readonly List<Location> owlLocations = [
            new("Waterfall Owl", "WATERFALL", LocationType.InactiveNPCs, index: 0),
            new("Visitor Owl", "VISITOR", LocationType.InactiveNPCs, index: 0),
            new("Pivot 1 Owl", "PIVOT_ONE", LocationType.InactiveNPCs, index: 0),
            new("Tree Owl", "TREE", LocationType.InactiveNPCs, index: 6),
        ];

        // Misc locations
        private static readonly List<Location> miscLocations = [
            new("Boileroom Map", "BOILEROOM", LocationType.InactiveArtObjects, index: 7),
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
