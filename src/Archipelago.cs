using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;

namespace FezAP
{
    public class Archipelago
    {
        private static readonly string gameName = "The Witness";  // TODO: Replace this with "Fez" once an apworld is available for testing
        private static ArchipelagoSession session;

        public static void Connect(string server, int port, string user, string pass)
        {
            session = ArchipelagoSessionFactory.CreateSession(server, port);

            LoginResult result = session.TryConnectAndLogin(gameName, user, ItemsHandlingFlags.AllItems,
                                                            new Version(6, 0), ["AP", "DeathLink"],
                                                            null,  // Unique identifier randomly generated if null
                                                            pass, true);

            if (result.Successful)
            {
                session.Say("Hello :)");

                // Bind events
                session.Items.ItemReceived += RecvItem;
                session.Locations.CheckedLocationsUpdated += null;  // TODO
                session.MessageLog.OnMessageReceived += HandleLogMsg;

                // TODO: Move past login screen to start game.
            }
            else
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                // TODO: Display the error message in login screen
            }
        }

        private static void HandleLogMsg(LogMessage message)
        {
            switch (message)
            {
                case CountdownLogMessage:
                case ServerChatLogMessage:
                    // TODO: Print chat message to display
                    break;
                default:
                    break;
            }
        }

        public static async Task SendLocation(string name)
        {
            var id = session.Locations.GetLocationIdFromName(gameName, name);
            var result = await session.Locations.ScoutLocationsAsync(id);
            ScoutedItemInfo item = result[0];
            await session.Locations.CompleteLocationChecksAsync(id);
            session.Say("Sent item");
            DotManager.Dialog = ["Sent %s to %s", item.ItemDisplayName, item.ItemGame];
        }

        public static FezGame.Components.IDotManager DotManager { private get; set;  }

        public static void RecvItem(ReceivedItemsHelper helper)
        {
            while (helper.PeekItem() != null)
            {
                ItemInfo item = helper.DequeueItem();
                FezGame.SpeedRun.AddCube(false);
                DotManager.ComeOut();
                DotManager.Dialog = ["Received %s from %s", item.ItemDisplayName, item.ItemGame];
                DotManager.TimeToWait = 5;
                DotManager.Burrow();
                session.Say("Recv");
            }
        }
    }
}
