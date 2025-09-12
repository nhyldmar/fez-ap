using FezEngine.Structure;

namespace FEZAP.Helpers
{
    /// Location information container
    public readonly struct Location(string name, string levelName, List<int> emplacement)
    {
        public readonly string name = name;  // apworld location name
        public readonly string levelName = levelName;  // name of the containing fezlvl
        public readonly TrileEmplacement emplacement = new(emplacement[0], emplacement[1], emplacement[2]);
    };

    public class LocationData
    {
        public static readonly List<Location> allLocations = [
            new("Abandonded A Cube", "ABANDONED_A", [11, 9, 10])
            // TODO: Fill out all of these :(
        ];
    }
}
