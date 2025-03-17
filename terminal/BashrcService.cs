using System.Text.RegularExpressions;
using CodeMechanic.Bash;
using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.Shargs;
using Vogen;

/// <summary>
/// Manages your local .bashrc and updates variables like exports, aliases, etc.
/// </summary>
public class BashrcService : QueuedService
{
    public BashrcService(ArgsMap argsMap)
    {
        if (argsMap.HasCommand("bash"))
            steps.Add(GetDeviceBashrcFilePath);
    }

    private async Task<string> GetDeviceBashrcFilePath()
    {
        Console.WriteLine("Looking for .bashrc files");
        string result = await "echo ~".Bash(verbose: true);
        string root_dir = result.Trim();
        Console.WriteLine("Root directory: " + root_dir);

        string root_bash_filepath = Path.Combine(root_dir, ".bashrc")
            .Replace("\\", "/") // if on Linux
            .Replace(
                @"\/",
                @"\"
            ) // if on Windows
        ;

        Console.WriteLine($"looking for file at '{root_bash_filepath}'");

        if (File.Exists(root_bash_filepath))
        {
            Console.WriteLine("bashrc found at: " + root_bash_filepath);
            string contents = File.ReadAllText(root_bash_filepath);
            Console.WriteLine(contents);
        }

        // string c_drive = Environment.SpecialFolder.MyDocuments.ToString();
        // var files = new Grepper() { FileNamePattern = ".bashrc" }.GetFileNames();
        // files.Dump("bashrc files found");

        return root_bash_filepath;
    }
}

[ValueObject<string>]
[Instance("Unspecified", "")]
[Instance(
    "AppendOnly",
    "Appends to the file.  No checks or validations. You're on your own if you mess up .bashrc"
)]
[Instance(
    "Patch",
    "Attempts to update your .bashrc file and runs source on it.  Corrects what it can."
)]
public partial class BashrcMode;

[ValueObject<string>]
[Instance("Unspecified", "")]
[Instance("Echo", "Basic echo line")]
[Instance("EchoInline", "a variant of echo where it's inline.")]
[Instance("Alias", "Basic aliases")]
public partial class BashrcLineType;
