namespace PS2Bios.Core.Helpers;

public static class PaddingHelper
{
    public static uint Align16(uint size) => (size + 15) & ~15u;
}