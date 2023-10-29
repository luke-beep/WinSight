using WinSight.Helpers;
using WinSight.Helpers.Interfaces;
using WinSight.Providers.Interfaces;
using WinSight.Services.Interfaces;

namespace WinSight.Providers;

public class OperatingSystemInfoProvider : IOperatingSystemInfoProvider
{
    private readonly IOperatingSystem _operatingSystem;
    private readonly IUtilities _utilities;

    private readonly Dictionary<string, List<object>> _propertiesDictionary;

    private readonly TableHandler _tableHandler;

    public OperatingSystemInfoProvider(IOperatingSystem operatingSystem, Dictionary<string, List<object>> propertiesDictionary, TableHandler tableHandler)
    {
        _operatingSystem = operatingSystem;
        _tableHandler = tableHandler;

        _propertiesDictionary = propertiesDictionary;

        _utilities = new Utilities();
    }

    public void AddOperatingSystemInformation()
    {
        var osProperties = new List<Dictionary<string, string>>
        {
            _operatingSystem.OSVersion(),
            _operatingSystem.Kernel(),
            _operatingSystem.Host(),
            _operatingSystem.OSPlatform(),
            _operatingSystem.UserName(),
            _operatingSystem.Uptime(),
            _operatingSystem.Shell(),
            _operatingSystem.Packages(),
            _operatingSystem.ProcessCount(),
            _operatingSystem.DefenderStatus(),
            _operatingSystem.Locale(),
            _operatingSystem.Timezone(),
            _operatingSystem.DesktopEnvironment(),
            _operatingSystem.WindowManager(),
            _operatingSystem.Terminal(),
            _operatingSystem.ShellVersion(),
            _operatingSystem.TerminalFont(),
            _operatingSystem.Cursor(),
            _operatingSystem.NetworkAdapters(),
            _operatingSystem.FirewallStatus()
        };
        foreach (var dict in osProperties)
        {
            ProcessDynamicInfo(dict);
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

