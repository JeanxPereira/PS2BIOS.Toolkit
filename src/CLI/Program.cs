using System.CommandLine;
using PS2BiosCLI.UI;
using PS2Bios.Core.Interfaces;
using PS2Bios.Core.Services;
using PS2Bios.Core.Models;
using Spectre.Console;

var rootCommand = new RootCommand("PS2 BIOS Extraction Tool");

var fileArg = new Argument<FileInfo>("file", "Path to the PS2 BIOS dump");
var allOption = new Option<bool>("--all", "Extract all modules");
var indexOption = new Option<int?>("--index", "Index of a specific module to extract");

var extractCommand = new Command("extract", "Parse and extract modules")
{
    fileArg,
    allOption,
    indexOption
};

extractCommand.SetHandler(async (file, extractAll, index) =>
{
    if (!file.Exists)
    {
        AnsiConsole.MarkupLine("[red]Error:[/] File not found.");
        return;
    }

    try
    {
        IBiosParser parser = new BiosParserService();
        IExtractionService extractor = new ExtractionService();

        var metadata = await AnsiConsole.Status().StartAsync("Parsing BIOS...", async ctx =>
        {
            using var stream = file.OpenRead();
            return await parser.ParseAsync(stream, file.Name);
        });

        OutputRenderer.RenderModuleTable(metadata);

        string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"extracted_{Path.GetFileNameWithoutExtension(file.Name)}");
        
        if (extractAll)
        {
            await AnsiConsole.Progress().StartAsync(async pCtx =>
            {
                var task = pCtx.AddTask("[green]Extracting all modules[/]");
                var progress = new Progress<double>(val => task.Value = val);
                using var stream = file.OpenRead();
                await extractor.ExtractAllAsync(stream, metadata.Modules, outputDir, progress);
            });
            
            AnsiConsole.MarkupLine($"[bold green]Success:[/] All modules saved to [yellow]{outputDir}[/]");
        }
        else if (index.HasValue && index >= 0 && index < metadata.Modules.Count)
        {
            using var stream = file.OpenRead();
            await extractor.ExtractAsync(stream, metadata.Modules[index.Value], outputDir);
            AnsiConsole.MarkupLine($"[bold green]Done:[/] Extracted module {index.Value} to [yellow]{outputDir}[/]");
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteException(ex);
    }
}, fileArg, allOption, indexOption);

rootCommand.AddCommand(extractCommand);
return await rootCommand.InvokeAsync(args);