using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Shargs;

public class PinesToHydroConverter : QueuedService
{
    private readonly ArgsMap arguments;

    public PinesToHydroConverter(ArgsMap arguments)
    {
        this.arguments = arguments;
        if (arguments.HasFlag("--pines-test"))
            steps.Add(TestConversion);
    }

    public async Task TestConversion()
    {
        bool debug = arguments.HasFlag("--debug");
        string cwd = Directory.GetCurrentDirectory();
        var filenames = new Grepper()
            {
                RootPath = cwd, Recursive = true,
                FileSearchMask = "*.html",
                // FileNamePattern = @"\.html"
            }
            .GetFileNames()
            .Where(x => !x.Contains(".converted."));

        //if(debug) filenames.Dump("files found ");

        bool prepend_model = arguments.HasFlag("--to-razor");

        foreach (var html_file in filenames)
        {
            string[] lines = File.ReadAllLines(html_file);

            if (prepend_model)
                lines = lines.Prepend("@model object").ToArray();

            var map = new Dictionary<string, string>()
            {
                [@"@click"] = "x-on:click",
                [@"@mouseenter"] = "x-on:mousenter",
                [@"@mouseleave"] = "x-on:mouseleave",
                [@"@keydown"] = "x-on:keydown",
                [@"@keyup"] = "x-on:keyup",
                /// etc.
            };

            string updated_html = lines
                .ReplaceAll(map)
                .Rollup();

            string save_path = html_file
                .Replace(@".html",
                    ".html.template"); // rename it to .html.template,
            // which may be later translated to all manner of things like blazor,
            // razor, Hydro, htmx, etc.

            if (debug) Console.WriteLine($"saving to '{save_path}'");
            File.WriteAllText(save_path, updated_html);
        }
    }
}