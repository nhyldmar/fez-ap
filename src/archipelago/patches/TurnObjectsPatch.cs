using System.Reflection;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame;
using FezGame.Services;
using FezGame.Structure;
using MonoMod.RuntimeDetour;

/*
 * Prevents Gomez from being able to turn various types of pivotable objects if the Turn Objects ability has not been
 * unlocked in the Archipelago.
 */
namespace FEZAP.Archipelago
{
    public class TurnObjectsPatch : IFezapPatch
    {
        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        [ServiceDependency]
        public IDotService DotService { private get; set; }

        private Hook PivotStateSpinHook;
        private Hook ValveStateGrabOntoHook;
        private Hook TombstoneStateGrabOntoHook;

        private bool DotTalking = false;

        public void Init()
        {
            Type PivotsHost = typeof(Fez).Assembly.GetType("FezGame.Components.PivotsHost");
            Type PivotState = PivotsHost.GetNestedType("PivotState", BindingFlags.NonPublic);
            PivotStateSpinHook = new Hook(PivotState.GetMethod("Spin", BindingFlags.Public | BindingFlags.Instance), TurnObjectsAllowedHooked);

            Type ValvesBoltsHost = typeof(Fez).Assembly.GetType("FezGame.Components.ValvesBoltsTimeswitchesHost");
            Type ValveState = ValvesBoltsHost.GetNestedType("ValveState", BindingFlags.NonPublic);
            ValveStateGrabOntoHook = new Hook(ValveState.GetMethod("GrabOnto", BindingFlags.Public | BindingFlags.Instance), TurnObjectsAllowedHooked);

            Type TombstonesHost = typeof(Fez).Assembly.GetType("FezGame.Components.TombstonesHost");
            Type TombstoneState = TombstonesHost.GetNestedType("TombstoneState", BindingFlags.NonPublic);
            TombstoneStateGrabOntoHook = new Hook(TombstoneState.GetMethod("GrabOnto", BindingFlags.Public | BindingFlags.Instance), TurnObjectsAllowedHooked);
        }

        private void TurnObjectsAllowedHooked(Action<object> original, object self)
        {
            if (ItemManager.ReceivedAbilityData.TurnObjects)
            {
                original(self);
                return;
            }

            if (!DotTalking)
            {
                DotTalking = true;
                DotService.Say("@You can't turn objects yet.", true, true).Ended = delegate { DotTalking = false; };
            }
        }

        public void Dispose()
        {
            PivotStateSpinHook.Dispose();
            ValveStateGrabOntoHook.Dispose();
            TombstoneStateGrabOntoHook.Dispose();
        }
    }
}
