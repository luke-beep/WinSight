using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using NetFwTypeLib;
using WinSight.Constants;
using WinSight.Helpers;
using WinSight.Services.Interfaces;

#pragma warning disable CA1416

namespace WinSight.Services;

public class OperatingSystem : IOperatingSystem
{
    private readonly Utilities utilities = new();

    public Dictionary<string, string> Kernel()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.KERNEL, Environment.OSVersion.Version.ToString() }
        };
    }

    public Dictionary<string, string> OSPlatform()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.OS_PLATFORM, RuntimeInformation.OSArchitecture.ToString() }
        };
    }

    public Dictionary<string, string> Host()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.HOST, Environment.MachineName }
        };
    }

    public Dictionary<string, string> UserName()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.USERNAME, Environment.UserName }
        };
    }

    public Dictionary<string, string> ProcessCount()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.PROCESS_COUNT, Process.GetProcesses().Length.ToString() }
        };
    }

    public Dictionary<string, string> Locale()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.LOCALE, CultureInfo.CurrentCulture.DisplayName }
        };
    }

    public Dictionary<string, string> Timezone()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.TIMEZONE, TimeZoneInfo.Local.DisplayName }
        };
    }

    public Dictionary<string, string> DesktopEnvironment()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.DESKTOP_ENVIRONMENT, "Fluent" }
        };
    }

    public Dictionary<string, string> WindowManager()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.WINDOW_MANAGER, "Desktop Window Manager" }
        };
    }

    public Dictionary<string, string> Uptime()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.UPTIME, GetUpTime() }
        };
    }

    private static string GetUpTime()
    {
        var uptime = DateTime.UtcNow - DateTime.UtcNow.AddMilliseconds(-Environment.TickCount);
        var parts = new List<string>();
        switch (uptime.Days)
        {
            case > 0:
                parts.Add(FormatUnit(uptime.Days, "day", "days"));
                break;
        }

        if (uptime.Days > 0 || uptime.Hours > 0) parts.Add(FormatUnit(uptime.Hours, "hour", "hours"));
        parts.Add(FormatUnit(uptime.Minutes, "min", "minutes"));
        return string.Join(", ", parts);

        static string FormatUnit(int count, string s, string p)
        {
            return $"{count} {(count == 1 ? s : p)}";
        }
    }

    public Dictionary<string, string> Shell()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.SHELL, GetShell() }
        };
    }

    private static string GetShell()
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

    public Dictionary<string, string> Packages()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.PACKAGES, GetPackages() }
        };
    }

    private string GetPackages()
    {
        var wingetOutput = utilities.GetCommandOutput("winget", "list");
        const int header = 3;
        var wingetPackages = string.IsNullOrEmpty(wingetOutput) ? 0 : wingetOutput.Split('\n').Length - header;

        return $"{wingetPackages} (winget)";
    }

    public Dictionary<string, string> NetworkAdapters()
    {
        var adapters = new Dictionary<string, string>();

        var query = new ObjectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID IS NOT NULL");

        var searcher = new ManagementObjectSearcher(query);

        foreach (var o in searcher.Get())
        {
            var adapter = (ManagementObject)o;
            var name = adapter["NetConnectionID"].ToString() ?? string.Empty;
            var status = adapter["NetConnectionStatus"].ToString() ?? throw new InvalidOperationException();
            status = ConvertStatusToString(status);
            adapters[name] = status;
        }

        return adapters;
    }

    private static string ConvertStatusToString(string status)
    {
        return status switch
        {
            "0" => "Disconnected",
            "1" => "Connecting",
            "2" => "Connected",
            "3" => "Disconnecting",
            "4" => "Hardware not present",
            "5" => "Hardware disabled",
            "6" => "Hardware malfunction",
            "7" => "Media disconnected",
            "8" => "Authenticating",
            "9" => "Authentication succeeded",
            "10" => "Authentication failed",
            "11" => "Invalid address",
            "12" => "Credentials required",
            _ => "Unknown"
        };
    }

    public Dictionary<string, string> FirewallStatus()
    {
        Dictionary<string, string> firewallStatus = new();
        var type = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
        switch (type)
        {
            case null:
                return firewallStatus;
        }

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

    public Dictionary<string, string> DefenderStatus()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.WINDOWS_DEFENDER_STATUS, GetDefenderStatus() }
        };
    }

    private string GetDefenderStatus()
    {
        var serviceStatus = utilities.GetDefenderItemValue("AMServiceEnabled") == "True" ? "Enabled" : "Disabled";
        return $"{serviceStatus}";
    }

    public Dictionary<string, string> Terminal()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.TERMINAL, GetTerminal() }
        };
    }

    private static string GetTerminal()
    {
        if (!IsWindows11()) return GetShell();
        using var key =
            Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.terminal\OpenWithProgids");
        if (key == null || key.GetValueNames().Length <= 0) return GetShell();

        return key.GetValueNames()[0];
    }

    private static bool IsWindows11()
    {
        return Environment.OSVersion.Version.CompareTo(new Version(10, 0, 22000, 0)) >= 0;
    }

    public Dictionary<string, string> ShellVersion()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.SHELL_VERSION, GetShellVersion() }
        };
    }

    private static string GetShellVersion()
    {
        return "";
    }

    public Dictionary<string, string> TerminalFont()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.TERMINAL_FONT, GetTerminalFont() }
        };
    }

    private static string GetTerminalFont()
    {
        return "";
    }

    public Dictionary<string, string> Cursor()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.CURSOR, GetCursor() }
        };
    }

    private static string GetCursor()
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

    public Dictionary<string, string> OSVersion()
    {
        return new Dictionary<string, string>
        {
            { PropertyConstants.OS, GetOSVersion() }
        };
    }

    private static string GetOSVersion()
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