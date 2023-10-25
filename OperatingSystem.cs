using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using NetFwTypeLib;
#pragma warning disable CA1416

namespace WinSight;

public class OperatingSystem : IOperatingSystem
{
    private readonly Utilities utilities = new();
    public string Network()
    {
        return "";
    }

    public string Kernel()
    {
        return Environment.OSVersion.Version.ToString();
    }

    public string OSPlatform()
    {
        return RuntimeInformation.OSArchitecture.ToString();
    }

    public string Host()
    {
        return Environment.MachineName;
    }

    public string UserName()
    {
        return Environment.UserName;
    }

    public string Uptime()
    {
        var uptime = DateTime.UtcNow - DateTime.UtcNow.AddMilliseconds(-Environment.TickCount);

        var parts = new List<string>();

        if (uptime.Days > 0) parts.Add(FormatUnit(uptime.Days, "day", "days"));
        if (uptime.Days > 0 || uptime.Hours > 0) parts.Add(FormatUnit(uptime.Hours, "hour", "hours"));

        parts.Add(FormatUnit(uptime.Minutes, "min", "minutes"));

        return string.Join(", ", parts);

        static string FormatUnit(int count, string s, string p)
        {
            return $"{count} {(count == 1 ? s : p)}";
        }
    }

    public string Shell()
    {
        try
        {
            var currentProcessId = Environment.ProcessId;
            var searcher =
                new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ProcessId = {currentProcessId}");
            foreach (var obj in searcher.Get())
            {
                var parentId = Convert.ToInt32(obj["ParentProcessId"]);
                var parentProcess = Process.GetProcessById(parentId);
                return parentProcess.ProcessName;
            }
        }
        catch
        {
            return "Unknown";
        }

        return "Unknown";
    }

    public string Packages()
    {
        var wingetOutput = utilities.GetCommandOutput("winget", "list");
        const int header = 3;
        var wingetPackages = string.IsNullOrEmpty(wingetOutput) ? 0 : wingetOutput.Split('\n').Length - header;

        return $"{wingetPackages} (winget)";
    }

    public string ProcessCount()
    {
        return Process.GetProcesses().Length.ToString();
    }


    public Dictionary<string, string> GetFirewallStatus()
    {
        Dictionary<string, string> firewallStatus = new();
        var type = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
        if (type == null) return firewallStatus;
        var firewallManager = (INetFwMgr)Activator.CreateInstance(type)!;
        firewallStatus.Add("Domain Firewall Profile",
            firewallManager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN).FirewallEnabled
                ? "Enabled"
                : "Disabled");
        firewallStatus.Add("Private Firewall Profile",
            firewallManager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD).FirewallEnabled
                ? "Enabled"
                : "Disabled");
        firewallStatus.Add("Current Firewall Profile",
            firewallManager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_CURRENT).FirewallEnabled
                ? "Enabled"
                : "Disabled");

        return firewallStatus;
    }

    public string GetDefenderStatus()
    {
        var serviceStatus = utilities.GetDefenderItemValue("AMServiceEnabled") == "True" ? "Enabled" : "Disabled";
        return $"{serviceStatus}";
    }

    public string Locale()
    {
        return CultureInfo.CurrentCulture.DisplayName;
    }

    public string Timezone()
    {
        return TimeZoneInfo.Local.DisplayName;
    }

    public string DesktopEnvironment()
    {
        return "Fluent";
    }

    public string WindowManager()
    {
        return "Desktop Window Manager";
    }

    public string Terminal()
    {
        return "";
    }

    public string ShellVersion()
    {
        return "";
    }

    public string TerminalFont()
    {
        return "";
    }

    public string Cursor()
    {
        var Theme = "Unknown";
        var Size = "Unknown";
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors");
            if (key != null)
            {
                Theme = key.GetValue(null)?.ToString() ?? "Unknown";
                if (key.GetValue("CursorBaseSize") is int cursorBaseSize) Size = cursorBaseSize.ToString();
            }
        }
        catch
        {
            return "";
        }

        return $"{Theme} ({Size}px)";
    }

    public string OSVersion()
    {
        ManagementObjectSearcher searcher = new("SELECT * FROM Win32_OperatingSystem");
        foreach (var search in searcher.Get().Cast<ManagementObject>())
        {
            var os = search["Caption"].ToString()?.Replace("Microsoft", "").Trim();
            var architecture = search["OSArchitecture"].ToString();
            return $"{os} {architecture}";
        }

        return "Unknown OS";
    }
}