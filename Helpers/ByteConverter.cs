using WinSight.Helpers.Interfaces;

namespace WinSight.Helpers;

public class ByteConverter : IByteConverter
{
    public double ToGibiBytesFromKilobytes(long kilobytes)
    {
        return kilobytes / (double)(1 << 20);
    }

    public double ToGibibytes(long bytes)
    {
        return bytes / (double)(1 << 30);
    }

    public double ToTebibytes(long bytes)
    {
        return bytes / (double)(1L << 40);
    }
}