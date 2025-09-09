using FezEngine.Effects;
using FezEngine.Structure;
using FezEngine.Tools;
using FezGame.Services;
using FEZAP.Features.Console;
using FEZAP.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEZAP.Features
{
    internal class GomezHitboxDraw : IFezapFeature
    {
        private Mesh TrileBoundingBox;

        public static bool WireframesEnabled;


        [ServiceDependency]
        public IGameLevelManager LevelManager { private get; set; }

        [ServiceDependency]
        public IGameStateManager GameState { get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { private get; set; }

        public void Initialize()
        {
            DrawActionScheduler.Schedule(delegate
            {
                var effect = new DefaultEffect.LitVertexColored
                {
                    Specular = true,
                    Emissive = 1.0f,
                    AlphaIsEmissive = true
                };

                Color trileColor = Color.Red;

                TrileBoundingBox = new Mesh
                {
                    DepthWrites = false,
                    Blending = BlendingMode.Alphablending,
                    Culling = CullMode.CullClockwiseFace,
                    Effect = effect
                };

                Color c = trileColor;
                TrileBoundingBox.AddWireframeBox(Vector3.One, Vector3.Zero, new Color(c.R, c.G, c.B, 32), true);
                TrileBoundingBox.AddColoredBox(Vector3.One, Vector3.Zero, new Color(c.R, c.G, c.B, 32), true);

            });
        }

        public void Update(GameTime gameTime) { }

        public void DrawLevel(GameTime gameTime)
        {
            if (!WireframesEnabled || GameState.Loading || LevelManager.Name == null) return;

            DrawingTools.GraphicsDevice.PrepareStencilWrite(StencilMask.Gomez);

            var trilebb = TrileBoundingBox;
            trilebb.Position = PlayerManager.Position;
            trilebb.Scale = PlayerManager.Size;
            trilebb.Draw();

            DrawingTools.GraphicsDevice.PrepareStencilWrite(StencilMask.None);
        }

        public void DrawHUD(GameTime gameTime)
        {

        }




        class InvisibleTrilesDrawToggleCommand : IFezapCommand
        {
            public string Name => "gomezhitbox";

            public string HelpText => "gomezhitbox [on/off] - draws hitbox for Gomez";

            public List<string> Autocomplete(string[] args)
            {
                return new string[] { "on", "off" }.Where(s => s.StartsWith(args[0])).ToList();
            }

            public bool Execute(string[] args)
            {
                if (args.Length != 1)
                {
                    FezapConsole.Print($"Incorrect number of parameters: '{args.Length}'", FezapConsole.OutputType.Warning);
                    return false;
                }

                if(args[0] != "on" && args[0] != "off")
                {
                    FezapConsole.Print($"Invalid argument: '{args[0]}'", FezapConsole.OutputType.Warning);
                    return false;
                }

                WireframesEnabled = args[0] == "on";
                FezapConsole.Print($"Gomez hitbox wireframes have been {(WireframesEnabled ? "enabled" : "disabled")}.");
                return true;
            }
        }
    }
}
