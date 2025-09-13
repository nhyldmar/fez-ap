using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using FezEngine.Services.Scripting;
using FezEngine.Tools;
using FezGame.Services;
using FezGame.Structure;

namespace FEZAP.Archipelago
{
    public class DeathManager
    {
        [ServiceDependency]
        public IGomezService GomezService { get; set; }

        [ServiceDependency]
        public IPlayerManager PlayerManager { get; set; }

        public static bool isDeathlink;
        private static bool handlingDeath;  // Used to avoid sending more deathlinks than intended

        public void HandleDeathlink(DeathLink deathLink)
        {
            handlingDeath = true;
            HudManager.Print($"Death received: {deathLink.Cause}");
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
                HudManager.Print("Death sent");
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
                ActionType.Suffering => "Got too close to danger",
                _ => "Died of death",
            };
        }
    }
}
