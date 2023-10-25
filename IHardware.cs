namespace WinSight;

public interface IHardware
{
    List<string> GetDiskInfo();
    List<string> Memory();
    List<string> GetGpuInfo();
    List<string> Cpu();
}