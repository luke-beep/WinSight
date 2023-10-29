namespace WinSight.Services.Interfaces;

public interface IHardware
{
    Dictionary<string, string> DiskInfo();
    Dictionary<string, string> MemoryInfo();
    Dictionary<string, string> GpuInfo();
    Dictionary<string, string> CpuInfo();
}
