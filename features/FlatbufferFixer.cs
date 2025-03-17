using System.Collections.Immutable;
using System.Text.RegularExpressions;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Microsoft.VisualBasic.FileIO;

public sealed class FlatbufferFixer : QueuedService
{
    private readonly ArgsMap arguments;

    public FlatbufferFixer(ArgsMap arguments)
    {
        this.arguments = arguments;
        // auto-run, unless there's specific needs for flags...
        if (this.arguments.HasCommand("buffers"))
            steps.Add(FixFlatbuffers);
    }

    private async Task FixFlatbuffers()
    {
        //1.  find all .fbs files.
        var gmix = RegexOptions.Compiled | RegexOptions.Multiline |
                   RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;

        string cwd = Directory.GetCurrentDirectory();
        string desktop = SpecialDirectories.Desktop;
        string projects_dir = Path.Combine(desktop, "projects");
        string root = projects_dir;

        var fbs_files = new Grepper
            {
                Recursive = true,
                RootPath = root, FileSearchMask = "*.fbs"
            }
            .GetFileNames();

        // fbs_files.Dump(nameof(fbs_files));

        var generated_fb_models = new Grepper()
            {
                Recursive = true,
                RootPath = root,
                FileSearchMask = "*.cs"
                // FileSearchLinePattern = FlatBufferPatterns
                // .HasVersionValidationMethod.Pattern  // todo: needs fixing.  Getmatchingfiles runs only an empty pattern when this is set!
            }
            .GetMatchingFiles(
                FlatBufferPatterns.HasVersionValidationMethod.CompiledRegex
            );

        // generated_fb_models.Dump(nameof(generated_fb_models));

        //2. map to all generated files w/ flatbuffer definitions.

        // 3. fix the Google import

        var version_regex = new Regex(
            @"<PackageReference\s+.*?Version=""""(?<version>.+?)""""\s+/>",
            gmix);

        Console.WriteLine(root);
        var flatbuffer_versions = new Grepper()
            {
                Recursive = true,
                RootPath = root,
                FileSearchMask = "*.csproj",
            }.GetMatchingFiles(version_regex)
            .Dump("matches")
            .Select(x => x.Line.Extract<Csproj>(version_regex))
            .Flatten()
            .Select(x => x.version.Replace(@"\.", "_"))
            .ToImmutableArray();

        flatbuffer_versions.Dump(nameof(flatbuffer_versions));

        //     <PackageReference Include="Google.FlatBuffers" Version="25.2.10" />
        // string csproj_text = "foo";
        //
        // string package_version = csproj_text.Extract<Csproj>(
        //         @"<PackageReference\s+.*?Version=""""(?<version>.+?)""""\s+/>")
        //     .FirstOrDefault().version;

        // 4. Fix the .net standard import.
        // 5. write to file.
    }

    private class FlatBufferPatterns : RegexEnumBase
    {
        public static FlatBufferPatterns HasVersionValidationMethod =
            new FlatBufferPatterns(1, nameof(HasVersionValidationMethod)
                , @"ValidateVersion\(\)");

        protected FlatBufferPatterns(int id, string name, string pattern,
            string uri = "") : base(id, name, pattern, uri)
        {
        }
    }
}

internal class Csproj
{
    public string version { get; set; } = string.Empty;
}

public struct GreppedLine
{
    public GreppedLine(string line, uint lineNumber, Regex matchingPattern)
    {
        this.line = line;
        line_number = lineNumber;
        is_matched = true;
        matching_pattern = matchingPattern;
    }

    public bool is_matched { get; set; }

    public uint line_number { get; set; } = 0;
    public string line { get; set; } = string.Empty;
    public Regex matching_pattern { get; set; }
}

public static class GrepperExtensions
{
    public static IEnumerable<Grepper.GrepResult> GetMatchingFiles(
        this Grepper grepper, Regex line_regex)
    {
        int line_number = 0;
        List<GreppedLine> grepped_lines = new List<GreppedLine>();

        foreach (var filePath in grepper.GetFileNames())
        {
            var lines = File.ReadAllLines(filePath);
            int total_lines = lines.Length;

            foreach (var line in lines)
            {
                if (line_regex.IsMatch(line))
                {
                    var grepped_line = new GreppedLine(
                        line
                        , (uint)line_number
                        , line_regex
                    );

                    grepped_lines.Add(grepped_line);

                    yield return new Grepper.GrepResult()
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        LineNumber = line_number,
                        Line = line,
                        LineCount = total_lines,
                        LineRegex = line_regex
                    };
                }

                line_number++;
            }
        }
    }
}