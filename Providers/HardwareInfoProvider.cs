using WinSight.Helpers;
using WinSight.Helpers.Interfaces;
using WinSight.Providers.Interfaces;
using WinSight.Services.Interfaces;

namespace WinSight.Providers;

public class HardwareInfoProvider : IHardwareInfoProvider
{
    private readonly IHardware _hardware;
    private readonly IUtilities _utilities;

    private readonly Dictionary<string, List<object>> _propertiesDictionary;

    private readonly TableHandler _tableHandler;

    public HardwareInfoProvider(IHardware hardware, Dictionary<string, List<object>> propertiesDictionary, TableHandler tableHandler)
    {
        _hardware = hardware;
        _tableHandler = tableHandler;

        _propertiesDictionary = propertiesDictionary;

        _utilities = new Utilities();
    }

    public void AddHardwareInformation()
    {
        List<Dictionary<string, string>> hardwareProperties = new()
        {
            _hardware.DiskInfo(),
            _hardware.GpuInfo(),
            _hardware.CpuInfo(),
            _hardware.MemoryInfo()
        };
        foreach (var hardwareInfo in hardwareProperties)
        {
            ProcessDynamicInfo(hardwareInfo);
        }
    }

    private void ProcessDynamicInfo(Dictionary<string, string> dynamicInfo)
    {
        foreach (var item in dynamicInfo)
        {
            _tableHandler.AddRow(item.Key, item.Value);
            _utilities.AddItemToDict(item.Key, new List<object> { item.Value }, _propertiesDictionary);
        }
    }
}