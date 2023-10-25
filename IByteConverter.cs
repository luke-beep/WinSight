namespace WinSight;

public interface IByteConverter
{
    double ToGibiBytesFromKilobytes(long kilobytes);
    double ToGibibytes(long bytes);
    double ToTebibytes(long bytes);

}