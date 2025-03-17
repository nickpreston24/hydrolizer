using CodeMechanic.Bash;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Sharprompt;

/// <summary>
/// Generates a brand new RazorHAT Daemon from
/// </summary>
public class RazorHATProjectGenerator : QueuedService
{
    private readonly ArgsMap arguments;

    public RazorHATProjectGenerator(ArgsMap arguments)
    {
        this.arguments = arguments;
        steps.Add(CloneAndForklift);
    }

    private async Task CloneAndForklift()
    {
        (bool should_clone, string url_to_clone) =
            arguments.WithFlags("--clone");

        // Console.WriteLine("should we clone?" +
        //                   should_clone); // todo: not correct.  WithFlags need to check for null.
        // Console.WriteLine(url_to_clone);

        if (!should_clone) return;

        if (url_to_clone.IsEmpty())
        {
            url_to_clone = PromptForUrl();
        }

        if (!url_to_clone.EndsWith(".git") || url_to_clone.IsEmpty())
        {
            Console.WriteLine(
                "WARNING: invalid url for cloning. Only '.git' links are supported");

            url_to_clone = PromptForUrl();
        }

        Console.WriteLine($"Cloning into repo '{url_to_clone}' ... ");

        await "mkdir .forklift".Bash();
        await $"cd .forklift; git clone {url_to_clone}".Bash(verbose: false);

        // ... copy /Pages, _Layout, and swap the namespace.

        string next_namespace = Prompt.Input<string>(
            "What should the new forklifted project (namespace) be called?");

        string cwd = Directory.GetCurrentDirectory();
        string forklift_folder = Path.Combine(cwd, ".forklift");

        var downloaded_project =
            url_to_clone.Extract<GitRepo>(
                @"(?<repo_url>.+?/(?<repo_name>\w+)\.git)" // https://regex101.com/r/xCGSkm/2
            ).SingleOrDefault();

        downloaded_project.Dump("the project you just dl'd");

        var all_projects = new Grepper()
                { RootPath = forklift_folder, FileSearchMask = "*.csproj" }
            .GetFileNames();

        string source_folder = $".forklift/{downloaded_project.repo_name}";

        //if(debug) all_projects.Dump("all projects");

        // perform the actual forklift...

        await $"mkdir .forklift/{next_namespace}; ls -a {next_namespace}"
            .Bash();

        await $"cp -r {source_folder} .forlift/{next_namespace}".Bash();

        Console.WriteLine("done!");
    }

    private string PromptForUrl()
    {
        string url =
            Prompt.Input<string>("Please type in a valid url to clone.");
        return url;
    }
}

internal class GitRepo
{
    public string repo_url { get; set; } = string.Empty;
    public string repo_name { get; set; } = string.Empty;
}