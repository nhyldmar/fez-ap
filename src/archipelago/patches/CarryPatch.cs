using System.Reflection;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame;
using FezGame.Components;
using FezGame.Services;
using FezGame.Structure;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

/*
 * Prevents Gomez from being able to carry items if the Carry ability has not been unlocked in the Archipelago.
 */
namespace FEZAP.Archipelago
{
    public class CarryPatch : IFezapPatch
    {
        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        public IDotService DotService { private get; set; }

        [ServiceDependency]
        public IDotManager Dot { get; set; }

        private ILHook LiftTestConditionsHook;
        private Hook GrabTestConditionsHook;

        private bool DotTalking = false;

        public void Init()
        {
            Type LiftAction = typeof(Fez).Assembly.GetType("FezGame.Components.Actions.Lift");
            LiftTestConditionsHook = new ILHook(LiftAction.GetMethod("TestConditions", BindingFlags.NonPublic | BindingFlags.Instance), CreateLiftTestConditionsHook);

            Type GrabAction = typeof(Fez).Assembly.GetType("FezGame.Components.Actions.Grab");
            GrabTestConditionsHook = new Hook(GrabAction.GetMethod("TestConditions", BindingFlags.NonPublic | BindingFlags.Instance), GrabTestConditionsHooked);
        }

        private void CreateLiftTestConditionsHook(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel skipLabel = il.DefineLabel();

            cursor.GotoNext(MoveType.Before, [ // ActionType actionType = ((!trileInstance.Trile.ActorSettings.Type.IsLight()) ...;
                i => i.MatchLdloc(1) || i.MatchLdloc(2),
                i => i.MatchLdfld("FezEngine.Structure.TrileInstance", "Trile"),
                i => i.MatchCallvirt("FezEngine.Structure.Trile", "get_ActorSettings"),
                i => i.MatchCallvirt("FezEngine.Structure.TrileActorSettings", "get_Type"),
                i => i.MatchCall("FezEngine.Structure.ActorTypeExtensions", "IsLight"),
            ]);

            cursor.MoveAfterLabels(); // Change the label behavior when emitting the delegate so branches will properly hit it
            cursor.EmitDelegate(LiftTestConditionsHooked); // Call check method
            cursor.MoveBeforeLabels(); // Back to default label behavior
            cursor.Emit(OpCodes.Brfalse, skipLabel); // If we can't carry, skip to the return

            cursor.GotoNext(MoveType.Before, i => i.MatchRet()); // return;
            cursor.MarkLabel(skipLabel); // Mark the return as location to skip to
        }

        private bool LiftTestConditionsHooked()
        {
            if (ItemManager.ReceivedAbilityData.Carry)
                return true;

            if (!DotTalking)
            {
                DotTalking = true;
                DotService.Say("@You can't carry objects yet.", true, true).Ended = delegate { DotTalking = false; };

                if (PlayerManager.Action == ActionType.Grabbing || PlayerManager.Action == ActionType.Pushing)
                {
                    // You can't get the dot text while pushing so cancel it to immediately get the text
                    PlayerManager.Action = ActionType.Idle;
                    PlayerManager.PushedInstance = null;
                }
            }

            return false;
        }

        private void GrabTestConditionsHooked(Action<object> original, object self)
        {
            if (!DotTalking) // Prevent immediately re-grabbing the block (which would delay the dot text)
                original(self);
        }

        public void Dispose()
        {
            LiftTestConditionsHook.Dispose();
            GrabTestConditionsHook.Dispose();
        }
    }
}
