using System.Buffers.Binary;
using System.Text;
using PS2Bios.Core.Helpers;
using PS2Bios.Core.Interfaces;
using PS2Bios.Core.Models;

namespace PS2Bios.Core.Services;

public class BiosParserService : IBiosParser
{
    private static ReadOnlySpan<byte> ResetSignature => "RESET"u8;

    public async Task<BiosMetadata> ParseAsync(Stream stream, string fileName, CancellationToken ct = default)
    {
        var buffer = new byte[stream.Length];
        await stream.ReadExactlyAsync(buffer, ct);
        ReadOnlyMemory<byte> romMemory = buffer;
        ReadOnlySpan<byte> romSpan = romMemory.Span;

        int resetIndex = romSpan.IndexOf(ResetSignature);
        if (resetIndex == -1)
            throw new InvalidDataException("RESET signature not found.");

        uint romDirSize = BinaryPrimitives.ReadUInt32LittleEndian(romSpan.Slice(resetIndex + 0x1C, 4));
        var modules = ParseDirectory(romSpan.Slice(resetIndex, (int)romDirSize));

        return new BiosMetadata(fileName, stream.Length, resetIndex, modules);
    }

    private List<RomModule> ParseDirectory(ReadOnlySpan<byte> data)
    {
        var modules = new List<RomModule>();
        int entries = (data.Length / 16) - 1; 
        long currentOffset = 0;

        for (int i = 0; i < entries; i++)
        {
            var entry = data.Slice(i * 16, 16);
            string rawName = Encoding.ASCII.GetString(entry.Slice(0, 10)).TrimEnd('\0').Trim();
            string name = (string.IsNullOrWhiteSpace(rawName) || rawName == "-") 
                ? $"PAD_{i:D3}" 
                : rawName;

            uint rawSize = BinaryPrimitives.ReadUInt32LittleEndian(entry.Slice(12, 4));
            uint alignedSize = PaddingHelper.Align16(rawSize);

            modules.Add(new RomModule(name, currentOffset, rawSize, alignedSize));
            
            currentOffset += alignedSize;
        }

        return modules;
    }
}