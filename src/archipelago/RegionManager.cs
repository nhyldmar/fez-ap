using FezEngine.Services;
using FezEngine.Tools;
using FEZUG.Features.Console;

namespace FEZAP.Archipelago
{

    public class RegionManager
    {

        [ServiceDependency]
        public ILevelManager Level { get; set; }
        
        // the levels' names that should trigger an update to the AP data storage (to avoid spamming updates)
        private static readonly List<string> levelNamesUpdate = [
            "BIG_TOWER", "MEMORY_CORE",
            "NATURE_HUB", "LIGHTHOUSE", "TREE",
            "INDUSTRIAL_HUB", "WATER_TOWER", "WELL_2",
            "SEWER_HUB", "SEWER_START",
            "GRAVEYARD_GATE", "GRAVE_CABIN",
            "ZU_CITY_RUINS", "TREE_SKY"
        ];

        public void UpdateCurrentLevel()
        {
            if (!ArchipelagoManager.IsConnected() || !levelNamesUpdate.Contains(Level.Name)) return;
            // put the region we just loaded into in AP data storage for tracking
            var slotData = ArchipelagoManager.session.DataStorage.GetSlotData(ArchipelagoManager.session.ConnectionInfo.Slot);
            slotData["current_level"] = Level.Name;
            FezugConsole.Print("Updated CurrentLevel to "+Level.Name+" in AP data storage");
        }
    }
}
