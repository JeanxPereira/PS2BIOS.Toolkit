namespace PS2Bios.Core.Models;

public record RomModule(
    string Name,
    long AbsoluteOffset,
    uint RawSize,
    uint AlignedSize)
{
    public double SizeInKB => RawSize / 1024.0;
    public string HexOffset => $"0x{AbsoluteOffset:X8}";
    public string HexSize => $"0x{RawSize:X8}";
}