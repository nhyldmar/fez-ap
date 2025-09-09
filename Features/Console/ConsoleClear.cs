namespace FEZAP.Features.Console
{
    internal class ConsoleClear : IFezapCommand
    {
        public string Name => "clear";
        public string HelpText => "clear - clears console output";

        public List<string> Autocomplete(string[] args) => null;

        public bool Execute(string[] args)
        {
            FezapConsole.Clear();
            return true;
        }
    }
}
