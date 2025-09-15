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
        public static readonly DeathManager deathManager = new();
        public static readonly ItemManager itemManager = new();
        public static readonly LocationManager locationManager = new();
        public static Fez Fez { get; private set; }
        public static GameTime GameTime { get; private set; }

        public Fezap(Game game) : base(game)
        {
            Fez = (Fez)game;
            Enabled = true;
            Visible = true;
            DrawOrder = 99999;
        }

        public override void Initialize()
        {
            base.Initialize();
            DrawingTools.Init();

            // Inject all our code
            AnnoyanceRemoval annoyanceRemoval = new();
            ServiceHelper.InjectServices(annoyanceRemoval);
            ServiceHelper.InjectServices(new ArchipelagoManager());
            ServiceHelper.InjectServices(new MenuManager());
            ServiceHelper.InjectServices(deathManager);
            ServiceHelper.InjectServices(new HudManager());
            ServiceHelper.InjectServices(itemManager);
            ServiceHelper.InjectServices(locationManager);

            // Run post-injection inits
            annoyanceRemoval.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            InputHelper.Update(gameTime);
            AnnoyanceRemoval.Update();
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
