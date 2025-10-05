using FezEngine.Tools;
using FezGame;
using Microsoft.Xna.Framework;
using FEZAP.Archipelago;
using FEZUG;

namespace FEZAP
{
    public readonly struct DelayedAction(TimeSpan time, Action action)
    {
        public readonly TimeSpan time = time;
        public readonly Action action = action;
    };

    public class Fezap : DrawableGameComponent
    {
        public static string Version = "v0.3.0";
        public readonly Fezug Fezug = new();
        public static readonly ArchipelagoManager archipelagoManager = new();
        public static readonly DeathManager deathManager = new();
        public static readonly DialogueManager dialogueManager = new();
        public static readonly DoorManager doorManager = new();
        public static readonly ItemManager itemManager = new();
        public static readonly LocationManager locationManager = new();
        public static List<DelayedAction> delayedActions = [];
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
            ServiceHelper.InjectServices(dialogueManager);
            ServiceHelper.InjectServices(doorManager);
            ServiceHelper.InjectServices(itemManager);
            ServiceHelper.InjectServices(locationManager);
        }

        public override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Fezug.Update(gameTime);
            archipelagoManager.Update();

            // Handle delayed actions
            for (int i = 0; i < delayedActions.Count; i++)
            {
                DelayedAction delayedAction = delayedActions[i];
                if (delayedAction.time <= gameTime.TotalGameTime)
                {
                    delayedAction.action.Invoke();
                    delayedActions.RemoveAt(i);
                    i--;  // Decrement index since an entry was removed
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Fezug.Draw(gameTime);
            // TODO: Figure out why this sometimes causes a crash. Needed for invisible wireframe drawing.
            // FezugInGameRendering.Draw(gameTime);
        }
    }
}
