using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using FEZAP.Features;
using FEZAP.Features.Console;

namespace FEZAP.Helpers
{
    public class Archipelago
    {
        public static readonly string gameName = "The Witness";  // TODO: Replace this with "Fez" once an apworld is available for testing
        public static ArchipelagoSession session;
        public static DeathLinkService deathLinkService;

        public static Dictionary<string, object> slotData;

        public static void Connect(string server, int port, string user, string pass)
        {
            session = ArchipelagoSessionFactory.CreateSession(server, port);

            LoginResult result = session.TryConnectAndLogin(gameName, user, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);

            if (result.Successful)
            {
                FezapConsole.Print("Successfully connected to AP server.", FezapConsole.OutputType.Info);

                // Get slot data
                slotData = session.DataStorage.GetSlotData(session.ConnectionInfo.Slot);

                // Bind events
                session.MessageLog.OnMessageReceived += HandleLogMsg;
                session.Socket.ErrorReceived += HandleErrorRecv;
                session.Socket.SocketClosed += HandleSocketClosed;
                session.Items.ItemReceived += HandleRecvItem;

                // Handle deathlink
                deathLinkService = session.CreateDeathLinkService();
                deathLinkService.OnDeathLinkReceived += HandleDeathlink;
                if ((bool)slotData["death_link"])
                {
                    deathLinkService.EnableDeathLink();
                }
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

        public static bool IsConnected()
        {
            if (session == null)
            {
                return false;
            }
            else
            {
                return session.Socket.Connected;
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

        private static void HandleErrorRecv(Exception e, string message)
        {
            FezapConsole.Print($"Error: {message}\n{e}");
        }

        private static void HandleSocketClosed(string reason)
        {
            FezapConsole.Print($"Socket closed: {reason}");
            // TODO: Reattempt connection logic with retry count
        }

        public static async Task SendLocation(string name)
        {
            var id = session.Locations.GetLocationIdFromName(gameName, name);
            var result = await session.Locations.ScoutLocationsAsync(id);
            ScoutedItemInfo item = result[0];
            await session.Locations.CompleteLocationChecksAsync(id);
            FezapConsole.Print($"Sent {item.ItemDisplayName} to {item.ItemGame}", FezapConsole.OutputType.Info);
        }

        private static void HandleRecvItem(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                ItemInfo item = helper.DequeueItem();
                FezapConsole.Print($"Received {item.ItemDisplayName} from {item.ItemGame}", FezapConsole.OutputType.Info);
                // TODO: Handle item
            }
        }

        private static void HandleDeathlink(DeathLink deathLink)
        {
            FezapConsole.Print(deathLink.Cause, FezapConsole.OutputType.Info);
            // TODO: Figure out a nicer way than creating an instance here.
            new Kill().Execute(null);
        }
    }
}
