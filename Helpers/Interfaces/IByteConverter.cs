namespace WinSight.Helpers.Interfaces;

public interface IByteConverter
{
    double ToGibiBytesFromKilobytes(long kilobytes);
    double ToGibibytes(long bytes);
    double ToTebibytes(long bytes);

}