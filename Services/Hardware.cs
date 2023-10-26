using System.Management;
using Microsoft.Win32;
using WinSight.Helpers;
using WinSight.Services.Interfaces;

#pragma warning disable CA1416

namespace WinSight.Services;

public class Hardware : IHardware
{
    private readonly ByteConverter byteConverter = new();

    public List<string> GetDiskInfo()
    {
        var diskInfos = new List<string>();
        foreach (var drive in DriveInfo.GetDrives())
        {
            switch (drive.IsReady)
            {
                case false:
                    continue;
            }
            var diskType = drive.DriveType;
            var diskName = drive.Name;
            var freeSpace = byteConverter.ToGibibytes(drive.TotalFreeSpace);
            var totalSpace = byteConverter.ToGibibytes(drive.TotalSize);
            var totalSpaceTb = byteConverter.ToTebibytes(drive.TotalSize);
            var usedSpace = totalSpace - freeSpace;
            var percentUsed = (int)(100 * usedSpace / totalSpace);
            var format = drive.DriveFormat;

            diskInfos.Add($"Disk ({diskName} - {diskType}): {usedSpace:0.##} GiB / {totalSpaceTb:0.##} TiB ({percentUsed}%) - {format}");
        }

        return diskInfos;
    }

    public List<string> Memory()
    {
        List<string> memoryInfo = new();
        ManagementObjectSearcher searcher = new("SELECT * FROM Win32_OperatingSystem");
        foreach (var o in searcher.Get())
        {
            var obj = (ManagementObject)o;
            var totalVisibleMemory = Convert.ToDouble(byteConverter.ToGibiBytesFromKilobytes(Convert.ToInt64(obj["TotalVisibleMemorySize"])));
            var freePhysicalMemory = Convert.ToDouble(byteConverter.ToGibiBytesFromKilobytes(Convert.ToInt64(obj["FreePhysicalMemory"])));
            var usedMemory = totalVisibleMemory - freePhysicalMemory;
            var usedMemoryPercent = (int)(100 * usedMemory / totalVisibleMemory);

            memoryInfo.Add($"{usedMemory:0.##} GiB / {totalVisibleMemory:0.##} GiB ({usedMemoryPercent}%)");
        }
        return memoryInfo;
    }

    public List<string> GetGpuInfo()
    {
        List<string> gpuInfo = new();
        ManagementObjectSearcher searcher = new("SELECT * FROM Win32_VideoController");

        foreach (var o in searcher.Get())
        {
            var obj = (ManagementObject)o;
            var gpuName = obj["Name"]?.ToString();
            var dacType = obj["AdapterDACType"]?.ToString();
            switch (dacType)
            {
                case "Internal":
                {
                    var vram = Convert.ToDouble(obj["AdapterRAM"]);
                    gpuInfo.Add($"{gpuName} ({vram / (1L << 30):0.##} GB)");
                    break;
                }
                default:
                    gpuInfo.Add($"{gpuName} ({GetGpuMemoryFromRegistry()} GB)");
                    break;
            }
        }

        return gpuInfo;
    }

    private static double GetGpuMemoryFromRegistry()
    {
        var baseRegistryPath = @"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
        using var baseKey = Registry.LocalMachine.OpenSubKey(baseRegistryPath);

        switch (baseKey)
        {
            case null:
                return 0;
        }
        foreach (var subKeyName in baseKey.GetSubKeyNames())
        {
            using var subKey = baseKey.OpenSubKey(subKeyName);
            var memorySizeObj = subKey?.GetValue("HardwareInformation.qwMemorySize");
            if (memorySizeObj != null)
            {
                return Convert.ToDouble(memorySizeObj) / (1L << 30);
            }
        }
        return 0;
    }

    public List<string> Cpu()
    {
        List<string> cpuInfo = new();
        ManagementObjectSearcher searcher = new("SELECT * FROM Win32_Processor");
        foreach (var o in searcher.Get())
        {
            var obj = (ManagementObject)o;
            var cpuName = obj["Name"]?.ToString();
            var cpuCores = obj["NumberOfCores"]?.ToString();
            var cpuThreads = obj["NumberOfLogicalProcessors"]?.ToString();
            cpuInfo.Add($"{cpuName} ({cpuCores} cores, {cpuThreads} threads)");
        }
        return cpuInfo;
    }
}
