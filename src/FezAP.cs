using FezEngine.Tools;
using FezGame;
using Microsoft.Xna.Framework;
using FEZAP.Archipelago;
using FEZUG;

namespace FEZAP
{
    public class Fezap : DrawableGameComponent
    {
        public static string Version = "v0.1.0";
        public readonly Fezug Fezug = new();
        public static readonly ArchipelagoManager archipelagoManager = new();
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
            Fezug.Initialize();

            // Inject all our code
            ServiceHelper.InjectServices(archipelagoManager);
            ServiceHelper.InjectServices(deathManager);
            ServiceHelper.InjectServices(itemManager);
            ServiceHelper.InjectServices(locationManager);
        }

        public override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Fezug.Update(gameTime);
            archipelagoManager.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            Fezug.Draw(gameTime);
            // TODO: Figure out why this sometimes causes a crash. Needed for invisible wireframe drawing.
            // FezugInGameRendering.Draw(gameTime);
        }
    }
}
