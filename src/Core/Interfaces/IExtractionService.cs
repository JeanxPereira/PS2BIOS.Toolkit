using PS2Bios.Core.Models;

namespace PS2Bios.Core.Interfaces;

public interface IExtractionService
{
    Task ExtractAsync(Stream romStream, RomModule module, string outputDirectory, CancellationToken ct = default);
    Task ExtractAllAsync(Stream romStream, IEnumerable<RomModule> modules, string outputDirectory, IProgress<double>? progress = null, CancellationToken ct = default);
}