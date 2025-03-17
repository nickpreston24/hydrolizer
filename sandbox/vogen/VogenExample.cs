using CodeMechanic.RazorHAT;

/// <summary>
/// from: https://github.com/SteveDunn/Vogen
/// </summary>
public class VogenExample : QueuedService
{
    public async Task Run()
    {
        CustomerId custy_id = CustomerId.From(42);

        AircraftPart valid_part = AircraftPart.From("F8090");
        AircraftPart invalid_part = AircraftPart.From("F8090foo");

        Age valid_age = Age.From(18);
        Age invalid_age = Age.From(16);

        Console.WriteLine(custy_id.ToString());
        Console.WriteLine(valid_part.ToString());
        Console.WriteLine(invalid_part.ToString());
    }
}
