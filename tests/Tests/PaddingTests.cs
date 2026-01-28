using FluentAssertions;
using PS2Bios.Core.Helpers;
using Xunit;

namespace PS2Bios.Tests.Unit;

public class PaddingTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 16)]
    [InlineData(15, 16)]
    [InlineData(16, 16)]
    [InlineData(17, 32)]
    public void Align16_ShouldWorkCorrectly(uint input, uint expected)
    {
        PaddingHelper.Align16(input).Should().Be(expected);
    }
}