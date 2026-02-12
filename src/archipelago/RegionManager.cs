using Archipelago.MultiClient.Net.Enums;
using FezEngine.Services;
using FezEngine.Tools;

namespace FEZAP.Archipelago
{

    /**
     * This class is solely used by the PopTracker Pack: https://github.com/maYayoh/GAT
     * Note that the 'regions' defined here corresponds solely to the tabs in the tracker.
     */
    public class RegionManager
    {

        [ServiceDependency]
        public ILevelManager Level { get; set; }
        private Region _currentRegion = Region.Village;
        
        // All levels and their tracker regions
        private static readonly Dictionary<string, Region> levelNames = new()
        {
            // Villageville
            { "GEEZER_HOUSE", Region.Village }, { "GEEZER_HOUSE_2D", Region.Village },
            { "KITCHEN", Region.Village }, { "KITCHEN_2D", Region.Village },
            { "PARLOR", Region.Village }, { "PARLOR_2D", Region.Village },
            { "SCHOOL", Region.Village }, { "SCHOOL_2D", Region.Village },
            { "GOMEZ_HOUSE", Region.Village }, { "GOMEZ_HOUSE_2D", Region.Village },
            { "BOILEROOM", Region.Village }, { "ABANDONED_A", Region.Village },
            { "ABANDONED_B", Region.Village }, { "ABANDONED_C", Region.Village },
            { "VILLAGEVILLE_2D", Region.Village }, { "VILLAGEVILLE_3D", Region.Village }, { "BIG_TOWER", Region.Village }, 
            
            // Memory Core
            { "MEMORY_CORE", Region.Core }, { "STARGATE", Region.Core },
            { "WALL_VILLAGE", Region.Core }, { "WALL_SCHOOL", Region.Core }, { "WALL_KITCHEN", Region.Core },
            { "WALL_INTERIOR_HOLE", Region.Core }, { "WALL_INTERIOR_A", Region.Core }, { "WALL_INTERIOR_B", Region.Core },
            { "INDUSTRIAL_CITY", Region.Core }, { "NUZU_DORM", Region.Core }, { "NUZU_SCHOOL", Region.Core },
            { "NUZU_ABANDONED_A", Region.Core }, { "NUZU_BOILERROOM", Region.Core }, { "SHOWERS", Region.Core },
            { "ZU_CITY", Region.Core }, { "OLDSCHOOL", Region.Core }, { "PURPLE_LODGE", Region.Core },
            { "ZU_HOUSE_EMPTY", Region.Core },{ "ZU_HOUSE_EMPTY_B", Region.Core }, { "ZU_HOUSE_SCAFFOLDING", Region.Core },
            
            // Natural Region
            { "NATURE_HUB", Region.Nature }, { "RITUAL", Region.Nature },
            { "ARCH", Region.Nature }, { "WEIGHTSWITCH_TEMPLE", Region.Nature },
            { "ZU_SWITCH", Region.Nature }, { "ZU_SWITCH_B", Region.Nature },
            { "FIVE_TOWERS", Region.Nature }, { "FIVE_TOWERS_CAVE", Region.Nature },
            { "WATERFALL", Region.Nature }, { "FOX", Region.Nature },
            { "ZU_CODE_LOOP", Region.Nature }, { "ZU_ZUISH", Region.Nature },
            { "MINE_A", Region.Nature }, { "MINE_BOMB_PILLAR", Region.Nature }, { "MINE_WRAP", Region.Nature },
            { "WATER_WHEEL", Region.Nature }, { "WATER_WHEEL_B", Region.Nature }, { "ZU_FORK", Region.Nature },
            { "CMY", Region.Nature }, { "CMY_B", Region.Nature }, { "CMY_FORK", Region.Nature }, 
            { "WATER_PYRAMID", Region.Nature }, { "TEMPLE_OF_LOVE", Region.Nature },
            { "BELL_TOWER", Region.Nature }, { "ZU_TETRIS", Region.Nature }, { "QUANTUM", Region.Nature },
            { "ANCIENT_WALLS", Region.Nature }, { "WALL_HOLE", Region.Nature }, { "TWO_WALLS", Region.Nature },
            { "FRACTAL", Region.Nature }, { "ZU_4_SIDE", Region.Nature }, { "ZU_HEADS", Region.Nature },
            { "LIGHTHOUSE", Region.Nature }, { "LIGHTHOUSE_HOUSE_A", Region.Nature }, { "LIGHTHOUSE_SPIN", Region.Nature },
            { "WATER_TOWER", Region.Nature }, { "WATERTOWER_SECRET", Region.Nature }, { "CABIN_INTERIOR_A", Region.Nature },
            { "TREE", Region.Nature }, { "TREE_CRUMBLE", Region.Nature }, { "TREE_ROOTS", Region.Nature },
            
            // Industrial District
            { "INDUSTRIAL_HUB", Region.Indust }, { "RAILS", Region.Indust }, { "WELL_2", Region.Indust },
            { "TRIPLE_PIVOT_CAVE", Region.Indust }, { "SPINNING_PLATES", Region.Indust },
            { "PIVOT_WATERTOWER", Region.Indust }, { "EXTRACTOR_A", Region.Indust },
            { "PIVOT_ONE", Region.Indust }, { "PIVOT_TWO", Region.Indust },
            { "PIVOT_THREE", Region.Indust }, { "PIVOT_THREE_CAVE", Region.Indust },
            { "WINDMILL_INT", Region.Indust }, { "WINDMILL_CAVE", Region.Indust },
            { "NUZU_ABANDONED_B", Region.Indust }, { "INDUST_ABANDONED_A", Region.Indust },
            { "INDUSTRIAL_SUPERSPIN", Region.Indust }, { "SUPERSPIN_CAVE", Region.Indust },
            
            // Sewers
            { "SEWER_HUB", Region.Sewer }, { "SEWER_START", Region.Sewer },
            { "SEWER_PIVOT", Region.Sewer }, { "SEWER_QR", Region.Sewer },
            { "SEWER_TREASURE_ONE", Region.Sewer }, { "SEWER_TREASURE_TWO", Region.Sewer },
            { "SEWER_TO_LAVA", Region.Sewer }, { "LAVA", Region.Sewer },
            { "LAVA_FORK", Region.Sewer }, { "LAVA_SKULL", Region.Sewer },
            { "SEWER_GEYSER", Region.Sewer }, { "SEWER_PILLARS", Region.Sewer },
            { "SEWER_FORK", Region.Sewer }, { "SEWER_LESSER_GATE_B", Region.Sewer },
            
            // Cemetery
            { "CABIN_INTERIOR_B", Region.Grave }, { "MAUSOLEUM", Region.Grave },
            { "CRYPT", Region.Grave }, { "TREE_OF_DEATH", Region.Grave },
            { "GRAVE_CABIN", Region.Grave }, { "GRAVEYARD_GATE", Region.Grave },
            { "SKULL", Region.Grave }, { "SKULL_B", Region.Grave },
            { "OWL", Region.Grave }, { "BIG_OWL", Region.Grave },
            { "GRAVE_GHOST", Region.Grave }, { "GRAVEYARD_A", Region.Grave },
            { "GRAVE_LESSER_GATE", Region.Grave }, { "GRAVE_TREASURE_A", Region.Grave },
            
            // Scientific Region
            { "ZU_CITY_RUINS", Region.Ruins }, { "STARGATE_RUINS", Region.Ruins },
            { "TREE_SKY", Region.Ruins }, { "THRONE", Region.Ruins },
            { "ZU_UNFOLD", Region.Ruins }, { "ZU_BRIDGE", Region.Ruins },
            { "PURPLE_LODGE_RUIN", Region.Ruins }, { "OLDSCHOOL_RUINS", Region.Ruins }, 
            { "ZU_HOUSE_QR", Region.Ruins }, { "ZU_HOUSE_RUIN_VISITORS", Region.Ruins },
            { "ZU_THRONE_RUINS", Region.Ruins }, { "CLOCK", Region.Ruins },
            { "ZU_LIBRARY", Region.Ruins }, { "LIBRARY_INTERIOR", Region.Ruins },
            { "GLOBE", Region.Ruins }, { "GLOBE_INT", Region.Ruins }, { "CODE_MACHINE", Region.Ruins },
            { "OBSERVATORY", Region.Ruins }, { "TELESCOPE", Region.Ruins }, { "VISITOR", Region.Ruins },
            { "ORRERY", Region.Ruins }, { "ORRERY_B", Region.Ruins },
            
            // Cutscenes and unused levels
            // { "GOMEZ_HOUSE_END_32", null }, { "GOMEZ_HOUSE_END_64", null },
            // { "VILLAGEVILLE_3D_END_32", null }, { "VILLAGEVILLE_3D_END_64", null },
            // { "DRUM", null }, { "ELDERS", null }, { "PYRAMID", null },
            // { "HEX_REBUILD", null }, { "OCTOHEAHEDRON", null },
            // { "WATERFALL_ALT", null }, { "ZU_HOUSE_RUIN_GATE", null },
        };

        public void UpdateCurrentRegion()
        {
            if (!ArchipelagoManager.IsConnected()) return;
            if (levelNames.TryGetValue(Level.Name, out var region) && !region.Equals(_currentRegion))
            {
                _currentRegion = region;
                ArchipelagoManager.session.DataStorage[Scope.Slot, "current_region"] = _currentRegion.ToString();
            }
        }

        private enum Region
        {
            Village,
            Core,
            Nature,
            Indust,
            Sewer,
            Grave,
            Ruins
        }
    }
}
