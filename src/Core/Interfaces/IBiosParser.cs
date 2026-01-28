using PS2Bios.Core.Models;

namespace PS2Bios.Core.Interfaces;

public interface IBiosParser
{
    Task<BiosMetadata> ParseAsync(Stream stream, string fileName, CancellationToken ct = default);
}