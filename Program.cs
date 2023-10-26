using Spectre.Console;
using System.Text.Json;
using WinSight.Constants;
using WinSight.Services;
using WinSight.Services.Interfaces;
using OperatingSystem = WinSight.Services.OperatingSystem;

namespace WinSight;

internal class Program
{
    private static readonly IOperatingSystem OperatingSystem = new OperatingSystem();
    private static readonly IHardware Hardware = new Hardware();
    private static readonly Table Table = new();
    private static readonly PropertyConstants PropertyConstants = new();

    private static readonly Dictionary<string, object> PropertiesDictionary = new();

    private static void Main(string[] args)
    {
        InitializeTable();
        InitializePropertiesDictionary();

        AddWatermark();

        OutputJson();
        OutputTable();
    }

    private static void InitializePropertiesDictionary()
    {
        AddOperatingSystemInformation();
        AddHardwareInformation();
    }

    private static void AddHardwareInformation()
    {
        foreach (var diskInfo in Hardware.GetDiskInfo())
        {
            AddRowToTableAndDict(PropertyConstants.DISK, diskInfo);
        }

        foreach (var gpuInfo in Hardware.GetGpuInfo())
        {
            AddRowToTableAndDict(PropertyConstants.GPU, gpuInfo);
        }

        foreach (var cpuInfo in Hardware.Cpu())
        {
            AddRowToTableAndDict(PropertyConstants.CPU, cpuInfo);
        }

        foreach (var memoryInfo in Hardware.Memory())
        {
            AddRowToTableAndDict(PropertyConstants.MEMORY, memoryInfo);
        }
    }

    private static void AddOperatingSystemInformation()
    {
        AddRowToTableAndDict(PropertyConstants.OS, OperatingSystem.OSVersion());
        AddRowToTableAndDict(PropertyConstants.KERNEL, OperatingSystem.Kernel());
        AddRowToTableAndDict(PropertyConstants.HOST, OperatingSystem.Host());
        AddRowToTableAndDict(PropertyConstants.OS_PLATFORM, OperatingSystem.OSPlatform());
        AddRowToTableAndDict(PropertyConstants.USERNAME, OperatingSystem.UserName());
        AddRowToTableAndDict(PropertyConstants.UPTIME, OperatingSystem.Uptime());
        AddRowToTableAndDict(PropertyConstants.SHELL, OperatingSystem.Shell());
        AddRowToTableAndDict(PropertyConstants.PACKAGES, OperatingSystem.Packages());
        AddRowToTableAndDict(PropertyConstants.PROCESS_COUNT, OperatingSystem.ProcessCount());
        AddRowToTableAndDict(PropertyConstants.NETWORK, OperatingSystem.Network());
        AddRowToTableAndDict(PropertyConstants.WINDOWS_DEFENDER_STATUS, OperatingSystem.GetDefenderStatus());
        AddRowToTableAndDict(PropertyConstants.LOCALE, OperatingSystem.Locale());
        AddRowToTableAndDict(PropertyConstants.TIMEZONE, OperatingSystem.Timezone());
        AddRowToTableAndDict(PropertyConstants.DESKTOP_ENVIRONMENT, OperatingSystem.DesktopEnvironment());
        AddRowToTableAndDict(PropertyConstants.WINDOW_MANAGER, OperatingSystem.WindowManager());
        AddRowToTableAndDict(PropertyConstants.TERMINAL, OperatingSystem.Terminal());
        AddRowToTableAndDict(PropertyConstants.SHELL_VERSION, OperatingSystem.ShellVersion());
        AddRowToTableAndDict(PropertyConstants.TERMINAL_FONT, OperatingSystem.TerminalFont());
        AddRowToTableAndDict(PropertyConstants.CURSOR, OperatingSystem.Cursor());
        foreach (var kvp in OperatingSystem.GetFirewallStatus())
        {
            AddRowToTableAndDict(kvp.Key, kvp.Value);
        }
    }

    private static void OutputJson()
    {
        var jsonString = JsonSerializer.Serialize(PropertiesDictionary, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("output.json", jsonString);
    }

    private static void OutputTable()
    {
        AnsiConsole.Write(Table);
    }

    private static void AddWatermark()
    {
        PropertiesDictionary.Add("WinSight", "https://github.com/luke-beep/WinSight");
    }


    private static void InitializeTable()
    {
        Table.AddColumns(new TableColumn("[u]Property[/]").Centered(), new TableColumn("[u]Value[/]").Centered())
            .Centered()
            .Border(TableBorder.DoubleEdge)
            .Title($"[yellow]WinSight ({OperatingSystem.UserName().ToLower()}@{OperatingSystem.Host().ToLower()})[/]");
    }

    private static void AddRowToTableAndDict(string property, object value)
    {
        Table.AddRow(property, value.ToString() ?? string.Empty);
        if (PropertiesDictionary.ContainsKey(property))
        {
            switch (PropertiesDictionary[property])
            {
                case List<object> list:
                    list.Add(value);
                    break;
                default:
                    PropertiesDictionary[property] = new List<object> { PropertiesDictionary[property], value };
                    break;
            }
        }
        else
        {
            PropertiesDictionary[property] = value;
        }
    }
}