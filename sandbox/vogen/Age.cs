using Vogen;

[ValueObject]
[Instance("Unspecified", -1)]
public readonly partial struct Age
{
    public static Validation Validate(int value) =>
        value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
}
