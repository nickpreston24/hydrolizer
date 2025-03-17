using CodeMechanic.RegularExpressions;

public class AircraftPartNamePatterns : RegexEnumBase
{
    protected AircraftPartNamePatterns(int id, string name, string pattern, string uri = "")
        : base(id, name, pattern, uri) { }

    public static AircraftPartNamePatterns Base = new AircraftPartNamePatterns(
        1,
        nameof(Base),
        @"\bF\d{1,4}\b",
        @"https://regex101.com/r/HG4Xyh/1"
    // , RegexOptions.Compiled
    );
}
