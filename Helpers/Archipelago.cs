using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using FEZAP.Features.Console;

namespace FEZAP.Helpers
{
    public class Archipelago
    {
        private static readonly string gameName = "The Witness";  // TODO: Replace this with "Fez" once an apworld is available for testing
        public static ArchipelagoSession session;

        public static void Connect(string server, int port, string user, string pass)
        {
            session = ArchipelagoSessionFactory.CreateSession(server, port);

            LoginResult result = session.TryConnectAndLogin(gameName, user, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);

            if (result.Successful)
            {
                FezapConsole.Print("Successfully connected to AP server.", FezapConsole.OutputType.Info);

                // Bind events
                session.Items.ItemReceived += RecvItem;
                session.Locations.CheckedLocationsUpdated += null;  // TODO
                session.MessageLog.OnMessageReceived += HandleLogMsg;
                // session.Socket.SocketClosed += ReattemptConnection;  // TODO: Add reconnection handler
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
                FezapConsole.Print(errorMessage);
            }
        }

        private static void HandleLogMsg(LogMessage message)
        {
            switch (message)
            {
                case CountdownLogMessage:
                case ServerChatLogMessage:
                    FezapConsole.Print(message.ToString(), FezapConsole.OutputType.Info);
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
            FezapConsole.Print($"Sent {item.ItemDisplayName} to {item.ItemGame}", FezapConsole.OutputType.Info);
        }

        public static void RecvItem(ReceivedItemsHelper helper)
        {
            while (helper.PeekItem() != null)
            {
                ItemInfo item = helper.DequeueItem();
                FezapConsole.Print($"Received {item.ItemDisplayName} from {item.ItemGame}", FezapConsole.OutputType.Info);
            }
        }
    }
}
