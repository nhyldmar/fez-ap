using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using FezEngine.Services;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FEZUG.Features.Console;

namespace FEZAP.Archipelago
{
    /// Dialogue data container
    public readonly struct DialogueData(string levelName, string npcName, string locationName, string msgFormat)
    {
        public readonly string levelName = levelName;
        public readonly string npcName = npcName;
        public readonly string locationName = locationName;
        public readonly string msgFormat = msgFormat;
    };

    public class DialogueManager
    {
        [ServiceDependency]
        public IGameStateManager GameState { private get; set; }

        [ServiceDependency]
        public ILevelManager LevelManager { private get; set; }

        // TODO: Add more entries
        private static readonly List<DialogueData> allDialogueData = [
            new("Villageville 3D", "Mayor McMayor", "Big Owl Anti-Cube", "@I think {0} has {2}'s {1}"),
        ];


        // TODO: Fix this
        public void LoadNpcHintDialogue()
        {
            var levelDialogueData = allDialogueData.FindAll(data => data.levelName == LevelManager.Name);
            foreach (var dialogueData in levelDialogueData)
            {
                // Get hinted info
                long locationId = ArchipelagoManager.session.Locations.GetLocationIdFromName(ArchipelagoManager.gameName, dialogueData.locationName);
                var result = ArchipelagoManager.session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, [locationId]);
                ScoutedItemInfo hint = result.Result[0];

                // Format dialogue string
                string dialogue = string.Format(dialogueData.msgFormat, hint.LocationName, hint.ItemName, hint.Player.Alias);

                // Update NPC dialogue
                NpcInstance npc = LevelManager.NonPlayerCharacters.First(npc => npc.Value.Name == dialogueData.npcName).Value;
                npc.Speech.Add(new() { Text = dialogue });
            }
        }
    }
}
