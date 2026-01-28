namespace PS2Bios.Core.Models;

public record BiosMetadata(
    string FileName,
    long TotalSize,
    long RomDirOffset,
    IReadOnlyList<RomModule> Modules);