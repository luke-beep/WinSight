using Spectre.Console;

namespace WinSight;

internal class Program
{
    private static void Main(string[] args)
    {
        Hardware hardware = new();
        OperatingSystem operatingSystem = new();

        var table = new Table()
            .Border(TableBorder.DoubleEdge)
            .Title($"[yellow]WinSight ({operatingSystem.UserName().ToLower()}@{operatingSystem.Host().ToLower()})[/]")
            .AddColumn(new TableColumn("[u]Property[/]").Centered())
            .AddColumn(new TableColumn("[u]Value[/]").Centered())
            .AddRow("OS", operatingSystem.OSVersion())
            .AddRow("Kernel", operatingSystem.Kernel())
            .AddRow("Host", operatingSystem.Host())
            .AddRow("OS Platform", operatingSystem.OSPlatform())
            .AddRow("User Name", operatingSystem.UserName())
            .AddRow("Uptime", operatingSystem.Uptime())
            .AddRow("Shell", operatingSystem.Shell())
            .AddRow("Packages", operatingSystem.Packages())
            .AddRow("Process Count", operatingSystem.ProcessCount())
            .AddRow("Network", operatingSystem.Network())
            .AddRow("Windows Defender Status", operatingSystem.GetDefenderStatus())
            .AddRow("Locale", operatingSystem.Locale())
            .AddRow("Timezone", operatingSystem.Timezone())
            .AddRow("Desktop Environment", operatingSystem.DesktopEnvironment())
            .AddRow("Window Manager", operatingSystem.WindowManager())
            .AddRow("Terminal", operatingSystem.Terminal())
            .AddRow("Shell Version", operatingSystem.ShellVersion())
            .AddRow("Terminal Font", operatingSystem.TerminalFont())
            .AddRow("Cursor", operatingSystem.Cursor())
            .Centered();

        foreach (var diskInfo in hardware.GetDiskInfo())
        {
            table.AddRow("Disk", diskInfo);
        }

        foreach (var gpuInfo in hardware.GetGpuInfo())
        {
            table.AddRow("GPU", gpuInfo);
        }

        foreach (var cpuInfo in hardware.Cpu())
        {
            table.AddRow("CPU", cpuInfo);
        }

        foreach (var kvp in operatingSystem.GetFirewallStatus())
        {
            table.AddRow(kvp.Key, kvp.Value);
        }

        foreach (var memoryInfo in hardware.Memory())
        {
            table.AddRow("Memory", memoryInfo);
        }


        AnsiConsole.Write(table);
    }
}