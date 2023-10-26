namespace WinSight.Services.Interfaces;

public interface IHardware
{
    List<string> GetDiskInfo();
    List<string> Memory();
    List<string> GetGpuInfo();
    List<string> Cpu();
}
