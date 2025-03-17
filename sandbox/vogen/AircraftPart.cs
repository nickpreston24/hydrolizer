using Vogen;

[ValueObject<string>]
[Instance("Unspecified", "")]
public partial struct AircraftPart
{
    public static Validation Validate(string value) =>
        AircraftPartNamePatterns.Base.CompiledRegex.IsMatch(value)
            ? Validation.Ok
            : Validation.Invalid("Must be greater than zero.");
}
