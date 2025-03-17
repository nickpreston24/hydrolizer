using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.RazorHAT;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using HtmlAgilityPack;
using ReverseMarkdown;
using Sharprompt;
using CodeHelper.Core.Extensions;


/// <summary>
/// Scrapes and translates a webpage to Markdown (.md)
/// </summary>
public class MarkdownloaderService : QueuedService
{
    private readonly ArgsMap arguments;

    public MarkdownloaderService(ArgsMap arguments)
    {
        this.arguments = arguments;
        if (arguments.HasCommand("md") && arguments.HasFlag("--sample"))
            steps.Add(SampleUsage);

        if (arguments.HasCommand("download"))
            steps.Add(DownloadMarkdown);
    }

    public async Task DownloadMarkdown()
    {
        (_, string url) = arguments.WithFlags("--url");

        if (url.IsEmpty())
        {
            url = Prompt.Input<string>("Please enter a URL.");
        }

        var web = new HtmlWeb();
        var document = web.Load(url);

        string html = document.DocumentNode.OuterHtml;
        string markdown = RunReverseMarkdown(html);
        string cwd = Directory.GetCurrentDirectory();
        var md_file_info = new SaveFile(markdown).To(cwd, ".hydrolizer", "md").As("codemaze.md");
        // Console.WriteLine("saved to: " + md_file_info.root);
    }

    private async Task SampleUsage()
    {
        string html = "This a sample <strong>paragraph</strong> from <a href=\"http://test.com\">my site</a>";

        var result = RunHtml2Markdown(html);
        // var result = RunReverseMarkdown(html);
        Console.WriteLine(result);
        string cwd = Directory.GetCurrentDirectory();
        new SaveFile(result).To(cwd).As("sample.md");
    }

    // private static string RunCodeHelperHtmlToMarkdown(string html)
    // {
    //     // return html.Ht();
    // }

    private static string RunHtml2Markdown(string html)
    {
        var converter = new Html2Markdown.Converter();
        var markdown = converter.Convert(html);
        return markdown;
    }

    private static string RunReverseMarkdown(string html, bool debug = false)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;
        var converter = new Converter();

        // TODO: this converter is hot garbage and needs fixing.
        // src: https://github.com/mysticmind/reversemarkdown-net
        string result = converter.Convert(html);
        if (debug) Console.WriteLine("markdown results: \n" + result);
        return result;
    }
}