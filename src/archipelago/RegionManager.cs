using Archipelago.MultiClient.Net.Enums;
using FezEngine.Services;
using FezEngine.Tools;

namespace FEZAP.Archipelago
{

    public class RegionManager
    {

        [ServiceDependency]
        public ILevelManager Level { get; set; }
        
        // the levels' names that should trigger an update to the AP data storage (to avoid spamming updates)
        private static readonly Dictionary<string, Region> levelNamesUpdate = new()
        {
            { "GOMEZ_HOUSE", Region.Village }, { "BIG_TOWER", Region.Village }, { "MEMORY_CORE", Region.Core },
            { "NATURE_HUB", Region.Nature }, { "LIGHTHOUSE", Region.Nature }, { "TREE", Region.Nature }, { "TREE_ROOTS", Region.Nature },
            { "INDUSTRIAL_HUB", Region.Indust }, { "WATER_TOWER", Region.Indust }, { "WELL_2", Region.Indust },
            { "SEWER_HUB", Region.Sewer }, { "SEWER_START", Region.Sewer }, { "SEWER_TO_LAVA", Region.Sewer },
            { "GRAVEYARD_GATE", Region.Grave }, { "CABIN_INTERIOR_B", Region.Grave }, { "MAUSOLEUM", Region.Grave },
            { "ZU_CITY_RUINS", Region.Ruins }, { "TREE_SKY", Region.Ruins }
        };

        public void UpdateCurrentRegion()
        {
            if (!ArchipelagoManager.IsConnected()) return;
            if (levelNamesUpdate.TryGetValue(Level.Name, out var currentRegion))
            {
                ArchipelagoManager.session.DataStorage[Scope.Slot, "current_region"] = currentRegion.ToString();
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
