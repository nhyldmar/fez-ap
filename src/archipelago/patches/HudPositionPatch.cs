using System.Reflection;
using FezGame.Components;
using FEZUG.Features.Hud;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

/*
 * The in-game HUD is in the top left corner, which is the same place as the FEZUG/FEZAP HUD. This patch fudges the
 * position of the in-game hud to be down a bit so that the two HUDs don't overlap.
 *
 * We also hide the key icon while connected to an AP since AP doesn't use keys and thus it's impossible to get any.
 */
namespace FEZAP.Archipelago
{
    public class HudPositionPatch : IFezapPatch
    {
        private ILHook DrawHudHook;

        public void Init()
        {
            DrawHudHook = new ILHook(typeof(HeadsUpDisplay).GetMethod("DrawHud", BindingFlags.NonPublic | BindingFlags.Instance), il =>
            {
                ILCursor cursor = new(il);
                ILLabel skipLabel = il.DefineLabel();

                cursor.GotoNext(MoveType.After, i => i.MatchLdcR4(58f)); // This is the 58f of the line `Vector2 vector = new Vector2(50f, 58f);`
                cursor.EmitDelegate(FudgeHeight); // Call our fuction to add to the height

                cursor.GotoNext(MoveType.AfterLabel, [ // spriteBatch.Draw(keyIcon, ...);
                    i => i.MatchLdarg(0),
                    i => i.MatchLdfld("FezGame.Components.HeadsUpDisplay", "spriteBatch"),
                    i => i.MatchLdarg(0),
                    i => i.MatchLdfld("FezGame.Components.HeadsUpDisplay", "keyIcon"),
                ]);

                cursor.EmitDelegate(ArchipelagoManager.IsConnected); // If we're connected to an AP...
                cursor.Emit(OpCodes.Brtrue, skipLabel); // ...skip past the line which draws the key icon

                cursor.GotoNext(MoveType.Before, i => i.MatchRet());
                cursor.MarkLabel(skipLabel); // Skip to the return
            });
        }

        private float FudgeHeight(float height)
        {
            return Math.Max(height, TextHud.ItemHudHeight);
        }

        public void Dispose()
        {
            DrawHudHook.Dispose();
        }
    }
}
