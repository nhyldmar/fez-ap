using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using FEZAP.Features;
using FEZAP.Features.Console;
using FezEngine.Components;
using FezEngine.Services;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace FEZAP.Helpers
{
    public class DeathManager : IFezapFeature
    {
        [ServiceDependency]
        public IGomezService GomezService { get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { get; set; }

        private bool handlingDeath;  // Used to avoid sending more deathlinks than intended

        public void HandleDeathlink(DeathLink deathLink)
        {
            handlingDeath = true;
            FezapConsole.Print($"Death received: {deathLink.Cause}", FezapConsole.OutputType.Info);
            PlayerManager.Action = PlayerManager.Grounded ? ActionType.Dying : ActionType.FreeFalling;
        }

        public void MonitorDeath()
        {
            if (GomezService.Alive)
            {
                handlingDeath = false;
            }
            else if (!GomezService.Alive && !handlingDeath)
            {
                handlingDeath = true;
                string playerName = ArchipelagoManager.session.Players.ActivePlayer.Name;
                string cause = GetCause(PlayerManager.Action);
                DeathLink deathlink = new(playerName, cause);
                ArchipelagoManager.deathLinkService.SendDeathLink(deathlink);
                FezapConsole.Print("Death sent");
            }
        }

        private static string GetCause(ActionType action)
        {
            return action switch
            {
                ActionType.FreeFalling => "Fell off the map",
                ActionType.Dying => "Splatted into the ground",
                ActionType.Sinking => "Fell into deadly liquid",
                ActionType.CrushHorizontal => "Squashed horizontally",
                ActionType.CrushVertical => "Squashed vertically",
                ActionType.SuckedIn => "Sucked into a black hole",
                _ => "Died of death",
            };
        }

        public void Initialize() { }
        public void Update(GameTime gameTime) { }
        public void DrawHUD(GameTime gameTime) { }
        public void DrawLevel(GameTime gameTime) { }
    }
}
