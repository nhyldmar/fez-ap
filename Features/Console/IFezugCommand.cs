using System.Collections.Generic;

namespace FEZAP.Features.Console
{
    public interface IFezapCommand
    {
        string Name { get; }
        string HelpText { get; }
        bool Execute(string[] args);
        List<string> Autocomplete(string[] args);
    }
}
