using System.Reflection;
using FezGame.Components;
using FEZUG.Features.Hud;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

/*
 * The in-game HUD is in the top left corner, which is the same place as the FEZUG/FEZAP HUD. This patch fudges the
 * position of the in-game hud to be down a bit so that the two HUDs don't overlap.
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

                cursor.GotoNext(MoveType.After, i => i.MatchLdcR4(58f)); // This is the 58f of the line `Vector2 vector = new Vector2(50f, 58f);`
                cursor.EmitDelegate(FudgeHeight); // Call our fuction to add to the height
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
