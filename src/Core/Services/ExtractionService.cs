using PS2Bios.Core.Interfaces;
using PS2Bios.Core.Models;

namespace PS2Bios.Core.Services;

public class ExtractionService : IExtractionService
{
    public async Task ExtractAsync(Stream romStream, RomModule module, string outputDirectory, CancellationToken ct = default)
    {
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        string filePath = Path.Combine(outputDirectory, module.Name.Replace("/", "_").Trim());
        
        byte[] buffer = new byte[module.AlignedSize];
        romStream.Seek(module.AbsoluteOffset, SeekOrigin.Begin);
        await romStream.ReadExactlyAsync(buffer, ct);

        await File.WriteAllBytesAsync(filePath, buffer, ct);
    }

    public async Task ExtractAllAsync(Stream romStream, IEnumerable<RomModule> modules, string outputDirectory, IProgress<double>? progress = null, CancellationToken ct = default)
    {
        var moduleList = modules.ToList();
        double total = moduleList.Count;
        int current = 0;

        foreach (var module in moduleList)
        {
            await ExtractAsync(romStream, module, outputDirectory, ct);
            current++;
            progress?.Report((current / total) * 100);
        }
    }
}