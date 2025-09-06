using FEZAP.Features.Console;

namespace FEZAP.Features
{
    internal class Connect : IFezapCommand
    {
        public string Name => "connect";

        public string HelpText => "connect <server> <port> <slot_name> <password> - connect to server";

        public List<string> Autocomplete(string[] args)
        {
            return null;
        }

        public bool Execute(string[] args)
        {
            // TODO: Make robust to int parsing and also avoid need for switch

            switch (args.Length)
            {
                case 3:
                    Helpers.Archipelago.Connect(args[0], int.Parse(args[1]), args[2], null);
                    break;
                case 4:
                    Helpers.Archipelago.Connect(args[0], int.Parse(args[1]), args[2], args[3]);
                    break;
                default:
                    FezapConsole.Print("Incorrect number of arguments", FezapConsole.OutputType.Warning);
                    break;
            }

            return true;
        }
    }

    internal class Say : IFezapCommand
    {
        public string Name => "say";

        public string HelpText => "say <message> - send message to server";

        public List<string> Autocomplete(string[] args)
        {
            return null;
        }

        public bool Execute(string[] args)
        {
            // TODO: Fix Socket.Connected being true even before session has been connected.
            if (Helpers.Archipelago.session.Socket.Connected)
            {
                Helpers.Archipelago.session.Say(string.Join(" ", args));
            }
            else
            {
                FezapConsole.Print("Unable to send. Not connected to a server. Use 'connect' command first.");
            }

            return true;
        }
    }
}
