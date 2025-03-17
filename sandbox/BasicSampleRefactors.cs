using System.Reflection;
using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Sharprompt;
using Vogen;

[ValueObject<string>]
[Instance("Unspecified", "")]
[Instance("Substitutions", "Basic Substitutions")]
[Instance("JSX", "JSX to HTMX")]
public partial class MySamples;

public class BasicSampleRefactors : QueuedService
{
    public BasicSampleRefactors(ArgsMap argsMap)
    {
        if (argsMap.HasCommand("refactors") && argsMap.HasFlag("--basic"))
        {
            var options = new[] { MySamples.JSX, MySamples.Substitutions };
            var response = Prompt.Select(
                "which example would you like to run?",
                options.Select(x => x.Value)
            );
            Console.WriteLine(response);

            if (response == MySamples.JSX)
                steps.Add(RefactorJSX);
        }
    }

    private async Task<bool> RefactorJSX()
    {
        string cwd = Directory.GetCurrentDirectory();
        string jsx_filepath = Path.Combine(cwd, "sample_conversions", "react_form_to_htmx.jsx");
        // string outfile_path = jsx_filepath.Replace(@"\.jsx", ".html");

        var replacement_map = new Dictionary<string, string>
        {
            [@"^import.+;?$"] = "", // https://regex101.com/r/N5EQnu/2
            [@"value=\{(?<handler>.*)\}"] = @"name=""${handler}""", // https://regex101.com/r/m5bv9f/1

            // cruft-removal:
            [@"^.*ReactDOM.createRoot.*;?"] = "",
            [@"^.*root.render.*;?"] = "",
            // [@"^.*const.*=\s*\(.*?\)\s*=>.*;?"] = "" //works, but incomplete
        };

        string input = File.ReadAllText(jsx_filepath);

        string results = input.Split('\n').ReplaceAll(replacement_map).Rollup();

        Console.WriteLine(results);

        new SaveFile(results).To(cwd, "sample_conversions").As("react_form_to_htmx.html");

        return true;
    }

    private async Task<bool> DoBasicRefactors()
    {
        // here's a basic refactor:

        var replacement_map = new Dictionary<string, string>
        {
            ["@@foo"] = "bar",
            ["code"] = "cupcakes",
        };

        string input = @"@@foo anyone from seeing my code!";
        string results = input.AsArray().Rollup();

        Console.WriteLine(results);

        return true;
    }

    /// <summary>
    /// structs set using reflection example
    /// src: https://stackoverflow.com/questions/6280506/is-there-a-way-to-set-properties-on-struct-instances-using-reflection
    /// </summary>
    private async Task<int> ReflectionOnStruct()
    {
        Rectangle rectangle = new Rectangle();
        PropertyInfo propertyInfo = typeof(Rectangle).GetProperty("Height");
        object boxed = rectangle;
        propertyInfo.SetValue(boxed, 5, null);
        rectangle = (Rectangle)boxed;

        Console.WriteLine(rectangle.Height);
        return 1;
    }
}

public struct Rectangle
{
    public int Height { get; set; }
}
