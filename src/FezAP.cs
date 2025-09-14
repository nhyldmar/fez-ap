using FezEngine.Tools;
using FezGame;
using FEZAP.Helpers;
using Microsoft.Xna.Framework;
using FEZAP.Archipelago;

namespace FEZAP
{
    public class Fezap : DrawableGameComponent
    {
        public static string Version = "v0.1.0";
        public static Fezap Instance { get; private set; }
        public static Fez Fez { get; private set; }

        public Fezap(Game game) : base(game)
        {
            Fez = (Fez)game;
            Instance = this;
            Enabled = true;
            Visible = true;
            DrawOrder = 99999;
        }

        public override void Initialize()
        {
            base.Initialize();
            DrawingTools.Init();

            // Inject all our code
            ServiceHelper.InjectServices(new ArchipelagoManager());
            ServiceHelper.InjectServices(new MenuManager());
            ServiceHelper.InjectServices(new DeathManager());
            ServiceHelper.InjectServices(new HudManager());
            ServiceHelper.InjectServices(new ItemManager());
            ServiceHelper.InjectServices(new LocationManager());
        }

        public override void Update(GameTime gameTime)
        {
            InputHelper.Update(gameTime);
            ArchipelagoManager.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            DrawingTools.BeginBatch();
            HudManager.DrawHUD();
            DrawingTools.EndBatch();
        }
    }
}
