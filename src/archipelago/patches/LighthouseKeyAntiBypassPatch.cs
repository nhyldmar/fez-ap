using System.Reflection;
using FezEngine.Services;
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
 *
 * We also hook OpenDoor to prevent the WATER_TOWER door from being openable for a bit of polish.
 */
namespace FEZAP.Archipelago
{
    public class LighthouseKeyAntiBypassPatch : IFezapPatch
    {
        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        public ILevelManager LevelManager { private get; set; }

        [ServiceDependency]
        public IDotService DotService { private get; set; }

        private ILHook EnterDoorTestConditionsHook;
        private ILHook OpenDoorTestConditionsHook;

        private bool DotTalking = false;

        public void Init()
        {
            Type EnterDoor = typeof(Fez).Assembly.GetType("FezGame.Components.Actions.EnterDoor");
            EnterDoorTestConditionsHook = new ILHook(EnterDoor.GetMethod("TestConditions", BindingFlags.NonPublic | BindingFlags.Instance), CreateEnterDoorTestConditionsHook);

            Type OpenDoor = typeof(Fez).Assembly.GetType("FezGame.Components.Actions.OpenDoor");
            OpenDoorTestConditionsHook = new ILHook(OpenDoor.GetMethod("TestConditions", BindingFlags.NonPublic | BindingFlags.Instance), CreateOpenDoorTestConditionsHook);
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

        private void CreateOpenDoorTestConditionsHook(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel skipLabel = il.DefineLabel();

            cursor.GotoNext(MoveType.AfterLabel, [ // base.WalkTo.Destination = GetDestination;
                i => i.MatchLdarg(0),
                i => i.MatchCall("FezGame.Components.Actions.PlayerAction", "get_WalkTo"),
            ]);

            cursor.EmitDelegate(EnterDoorTestConditionsHooked); // Call check method
            cursor.Emit(OpCodes.Brfalse, skipLabel); // If we can't enter, skip to the return

            cursor.GotoNext(MoveType.Before, i => i.MatchRet());
            cursor.MarkLabel(skipLabel); // Mark the return as location to skip to
        }

        private bool EnterDoorTestConditionsHooked()
        {
            if (DoorManager.LighthouseUnlocked || !ArchipelagoManager.IsConnected())
                return true;

            if (LevelManager.Name != "LIGHTHOUSE" || PlayerManager.Position.Y < 30f)
                return true;

            // The player is in LIGHTHOUSE, too high, and doesn't have the key! Prevent them from entering the door

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
            OpenDoorTestConditionsHook.Dispose();
        }
    }
}
