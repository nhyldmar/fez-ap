using System.Reflection;
using FezEngine.Structure;
using FezGame.Components;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;

/*
 * The in-game map shows if you still have a locked door you haven't unlocked yet, but due to the way the AP implements
 * doors, they get converted into regular doors and opening those doesn't count towards the count of locked doors. This
 * patch fudges the map nodes to make unlocked doors count as regular doors once you earn its Door Unlocked item.
 */
namespace FEZAP.Archipelago
{
    public class MapLockedDoorPatch : IFezapPatch
    {
        private Hook WorldMapBuildNodesHook;

        public void Init()
        {
            WorldMapBuildNodesHook = new Hook(typeof(WorldMap).GetMethod("BuildNodes", BindingFlags.NonPublic | BindingFlags.Instance), WorldMapBuildNodesHooked);
        }

        private void WorldMapBuildNodesHooked(Action<WorldMap, MapNode, MapNode.Connection, MapNode, Vector3, List<Matrix>> original, WorldMap self, MapNode node, MapNode.Connection parentConnection, MapNode parentNode, Vector3 offset, List<Matrix> instances)
        {
            string levelName = node.LevelName;

            if (ArchipelagoManager.IsConnected())
            {
                if ((levelName == "VILLAGEVILLE_3D" && DoorManager.BoileroomUnlocked)
                        || (levelName == "LIGHTHOUSE" && DoorManager.LighthouseUnlocked)
                        || (levelName == "RAILS" && DoorManager.WellUnlocked)
                        || (levelName == "PIVOT_ONE" && DoorManager.WindmillUnlocked)
                        || (levelName == "MAUSOLEUM" && DoorManager.MausoleumUnlocked)
                        || (levelName == "SEWER_HUB" && DoorManager.SewerQrUnlocked)
                        || (levelName == "SEWER_PILLARS" && DoorManager.SewerForkUnlocked)
                        || (levelName == "TREE" && DoorManager.TreeCrumbleUnlocked))
                {
                    // Turn the locked doors into regular doors for this node's win conditions
                    node.Conditions.UnlockedDoorCount += node.Conditions.LockedDoorCount;
                    node.Conditions.LockedDoorCount = 0;
                }

                if ((levelName == "TREE" && !DoorManager.CabinUnlocked)
                    || (levelName == "NATURE_HUB" && !(DoorManager.ArchUnlocked && DoorManager.BellTowerUnlocked))
                    || (levelName == "TREE_SKY" && !DoorManager.ThroneUnlocked))
                {
                    // Make sure it shows a locked door icon for the new locked doors
                    node.Conditions.LockedDoorCount = 999;
                }
            }
            original(self, node, parentConnection, parentNode, offset, instances);
        }

        public void Dispose()
        {
            WorldMapBuildNodesHook.Dispose();
        }
    }
}
