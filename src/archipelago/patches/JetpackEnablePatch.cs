using System.Reflection;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame;
using FezGame.Services;
using FezGame.Structure;
using MonoMod.RuntimeDetour;

/*
 * Allows the jetpack to be enabled without adverse affects by temporarily fudging the Finished32 flag
 */
namespace FEZAP.Archipelago
{
    public class JetpackEnablePatch : IFezapPatch
    {
        [ServiceDependency]
        public IGameStateManager GameState { private get; set; }

        private Hook GameWideCodesTestInputMethodHook;

        public void Init()
        {
            Type GameWideCodes = typeof(Fez).Assembly.GetType("FezGame.Components.GameWideCodes");
            MethodInfo GameWideCodesTestInputMethod = GameWideCodes.GetMethod("TestInput", BindingFlags.NonPublic | BindingFlags.Instance);

            GameWideCodesTestInputMethodHook = new Hook(GameWideCodesTestInputMethod, GameWideCodesTestInputMethodHooked);
        }

        private void GameWideCodesTestInputMethodHooked(Action<object> original, object self)
        {
            bool origFinished32 = GameState.SaveData.Finished32;
            if (ItemManager.ReceivedAbilityData.Jetpack)
                GameState.SaveData.Finished32 = true;
            original(self);
            GameState.SaveData.Finished32 = origFinished32;
        }

        public void Dispose()
        {
            GameWideCodesTestInputMethodHook.Dispose();
        }
    }
}
