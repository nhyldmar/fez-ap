using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;

namespace FezAP
{
    public class Archipelago
    {
        public static void Connect(string server, string user, string pass)
        {
            var session = ArchipelagoSessionFactory.CreateSession(server);
            LoginResult result;

            try
            {
                result = session.TryConnectAndLogin("Fez",
                                                    user,
                                                    ItemsHandlingFlags.AllItems,
                                                    new Version(6, 3),
                                                    ["DeathLink"],
                                                    null, // Unique identifier randomly generated if null
                                                    pass,
                                                    true);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                var loginSuccess = (LoginSuccessful)result;

                Console.WriteLine(loginSuccess.ToString());
                return;
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
                Console.WriteLine(errorMessage);
            }
        }
    }
}
