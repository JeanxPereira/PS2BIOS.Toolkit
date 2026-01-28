using PS2Bios.Core.Models;
using Spectre.Console;

namespace PS2BiosCLI.UI; // Usaremos PS2BiosCLI sem o ponto no meio

public static class OutputRenderer
{
    public static void RenderModuleTable(BiosMetadata metadata)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]BIOS Modules:[/] {metadata.FileName}")
            .Caption($"Total Size: [blue]{metadata.TotalSize} bytes[/]");

        table.AddColumn("[grey]ID[/]");
        table.AddColumn("Name");
        table.AddColumn("Offset");
        table.AddColumn("Raw Size");
        table.AddColumn("Aligned");

        for (int i = 0; i < metadata.Modules.Count; i++)
        {
            var mod = metadata.Modules[i];
            table.AddRow(
                i.ToString(),
                $"[green]{mod.Name}[/]",
                $"[cyan]{mod.HexOffset}[/]",
                mod.RawSize.ToString(),
                mod.HexSize
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderError(string message)
    {
        AnsiConsole.Write(new Panel($"[red]Error:[/] {message}").BorderColor(Color.Red));
    }
}