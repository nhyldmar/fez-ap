using System.Reflection;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame;
using FezGame.Services;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

/*
 * Prevents players from CHEATING!!! by using the jetpack to bypass the Lighthouse key.
 *
 * EnterDoor.TestConditions already has a bunch of level-specific hacks in it to prevent the player from entering a
 * door if certain conditions aren't met, e.g. you can't enter SKULL_B if you didn't rotate all four tombstones to the
 * same direction. We're adding another one that checks if you're entering any of the post-lighthouse key rooms and
 * cancelling it if you don't have the key yet.
 */
namespace FEZAP.Archipelago
{
    public class LighthouseKeyAntiBypassPatch : IFezapPatch
    {
        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        public IDotService DotService { private get; set; }

        private ILHook EnterDoorTestConditionsHook;

        private bool DotTalking = false;

        public void Init()
        {
            Type EnterDoor = typeof(Fez).Assembly.GetType("FezGame.Components.Actions.EnterDoor");
            EnterDoorTestConditionsHook = new ILHook(EnterDoor.GetMethod("TestConditions", BindingFlags.NonPublic | BindingFlags.Instance), CreateEnterDoorTestConditionsHook);
        }

        private void CreateEnterDoorTestConditionsHook(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel skipLabel = il.DefineLabel();

            cursor.GotoNext(MoveType.AfterLabel, [ // base.GameState.SkipLoadScreen = (skipFade = base.LevelManager.DestinationVolumeId.HasValue && ...);
                i => i.MatchLdarg(0),
                i => i.MatchCall("FezGame.Components.Actions.PlayerAction", "get_GameState"),
                i => i.MatchLdarg(0),
                i => i.MatchLdarg(0),
            ]);

            cursor.EmitDelegate(EnterDoorTestConditionsHooked); // Call check method
            cursor.Emit(OpCodes.Brfalse, skipLabel); // If we can't enter, skip to the return

            cursor.GotoNext(MoveType.Before, i => i.MatchRet());
            cursor.MarkLabel(skipLabel); // Mark the return as location to skip to
        }

        private bool EnterDoorTestConditionsHooked()
        {
            if (PlayerManager.NextLevel != "LIGHTHOUSE_SPIN" && PlayerManager.NextLevel != "WATER_TOWER")
                return true;

            if (DoorManager.LighthouseUnlocked)
                return true;

            if (!DotTalking)
            {
                DotTalking = true;
                DotService.Say("@No cheating!!! Come back when you have Lighthouse Door Unlocked.", true, true).Ended = delegate { DotTalking = false; };
            }
            return false;
        }

        public void Dispose()
        {
            EnterDoorTestConditionsHook.Dispose();
        }
    }
}
