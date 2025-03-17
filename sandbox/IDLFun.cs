using System.Diagnostics;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.Shargs;

public class IDLFun : QueuedService
{
    private readonly ArgsMap arguments;

    public IDLFun(ArgsMap arguments)
    {
        this.arguments = arguments;
        if (arguments.HasCommand("idl") && arguments.HasFlag("--fun"))
        {
            steps.Add(GetAllIDLFilesAsync);
        }
    }

    /// <summary>
    /// wip on idl objects detection:
    /// https://regex101.com/r/wTuz5B/3
    /// </summary>
    private async Task GetAllIDLFilesAsync()
    {
        var watch = Stopwatch.StartNew();

        string cwd = Directory.GetCurrentDirectory();
        string projects_dir = cwd.AsDirectory().GoUpToDirectory("projects").FullName;

        var all_idl_files = new Grepper() { RootPath = projects_dir, FileSearchMask = "*.idl" }
            .GetFileNames()
            .ToArray();

        watch.Stop();

        all_idl_files.Take(5).Dump();
        Console.WriteLine($"Found {all_idl_files.Length} IDL files in {watch.Elapsed}.");
    }
}
