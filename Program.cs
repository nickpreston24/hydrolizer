using CodeMechanic.Shargs;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Sharpify.Core;
using RunModeExtensions = CodeMechanic.RazorHAT.RunModeExtensions;

internal class Program
{
    static async Task Main(string[] args)
    {
        var arguments = new ArgsMap(args);
        bool debug = arguments.HasFlag("--debug");

        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                "./logs/hydrolizer.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

        (bool run_as_web, bool run_as_cli) =
            RunModeExtensions.GetRunModes(arguments);

        // if (debug)
        //     Console.WriteLine($"{nameof(run_as_web)}: {run_as_web}");

        if (debug)
            Console.WriteLine($"{nameof(run_as_cli)}: {run_as_cli}");

        if (run_as_cli)
        {
            await RunAsCli(arguments, logger);
        }
    }

    static async Task RunAsCli(ArgsMap arguments, Logger logger)
    {
        var services = CreateServices(arguments, logger);
        Application app = services.GetRequiredService<Application>();
        await app.Run();
    }

    private static ServiceProvider CreateServices(ArgsMap arguments,
        Logger logger)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(arguments)
            .AddSingleton<Logger>(logger)
            .AddScoped<BasicSampleRefactors>()
            .AddScoped<IDLFun>()
            .AddScoped<BashrcService>()
            .AddScoped<MarkdownloaderService>()
            .AddScoped<JavascriptRefactors>()
            .AddScoped<PinesToHydroConverter>()
            .AddScoped<RazorHATProjectGenerator>()
            .AddScoped<FlatbufferFixer>()
            .AddScoped<CdnLibraryImporter>()
            .AddSingleton<Application>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}

public class Application
{
    private readonly Logger logger;
    private readonly BasicSampleRefactors refactor_samples;
    private readonly BashrcService bashrc_service;
    private readonly MarkdownloaderService md_downloader_service;
    private readonly IDLFun idlfun;
    private readonly JavascriptRefactors js_refactors;
    private readonly PinesToHydroConverter pines_to_hydro;
    private readonly RazorHATProjectGenerator razorhat_generator;
    private readonly FlatbufferFixer flatbuffer_fixer;
    private readonly CdnLibraryImporter cdn_importer;

    public Application(
        Logger logger,
        BasicSampleRefactors refactor_samples,
        BashrcService bashrc_service,
        MarkdownloaderService md_downloader_service,
        JavascriptRefactors js_refactors,
        IDLFun idlFun,
        PinesToHydroConverter pines_to_hydro,
        RazorHATProjectGenerator razors,
        FlatbufferFixer flatbuffer_fixer,
        CdnLibraryImporter cdn_importer
    )
    {
        this.logger = logger;
        this.refactor_samples = refactor_samples;
        this.bashrc_service = bashrc_service;
        this.md_downloader_service = md_downloader_service;
        idlfun = idlFun;
        this.js_refactors = js_refactors;
        this.pines_to_hydro = pines_to_hydro;
        razorhat_generator = razors;
        this.flatbuffer_fixer = flatbuffer_fixer;
        this.cdn_importer = cdn_importer;
    }

    public async Task Run()
    {
        await refactor_samples.Run();
        await bashrc_service.Run();
        await md_downloader_service.Run();
        await idlfun.Run();
        await js_refactors.Run();
        await pines_to_hydro.Run();
        await razorhat_generator.Run();
        await flatbuffer_fixer.Run();
    }
}