using System.Buffers.Binary;
using System.Text;
using FluentAssertions;
using PS2Bios.Core.Services;
using Xunit;

namespace PS2Bios.Tests.Unit;

public class ParserTests
{
    [Fact]
    public async Task ParseAsync_ShouldFailOnInvalidFile()
    {
        using var ms = new MemoryStream(new byte[1024]);
        var parser = new BiosParserService();

        var act = () => parser.ParseAsync(ms, "test.bin");

        await act.Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task ParseAsync_ShouldLocateReset()
    {
        using var ms = new MemoryStream();
        byte[] padding = new byte[64];
        byte[] resetTag = "RESET"u8.ToArray();
        byte[] metadata = new byte[32];
        
        // Simulating ROMDIR size at offset +0x1C
        BinaryPrimitives.WriteUInt32LittleEndian(metadata.AsSpan(0x1C), 32);

        ms.Write(padding);
        ms.Write(resetTag);
        ms.Write(metadata);
        ms.Position = 0;

        var parser = new BiosParserService();
        var result = await parser.ParseAsync(ms, "dummy.bin");

        result.RomDirOffset.Should().Be(64);
    }
}