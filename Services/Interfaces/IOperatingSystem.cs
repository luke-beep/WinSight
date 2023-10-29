namespace WinSight.Services.Interfaces;

public interface IOperatingSystem
{
    Dictionary<string, string> Kernel();
    Dictionary<string, string> OSPlatform();
    Dictionary<string, string> OSVersion();
    Dictionary<string, string> Host();
    Dictionary<string, string> UserName();
    Dictionary<string, string> Uptime();
    Dictionary<string, string> Shell();
    Dictionary<string, string> Packages();
    Dictionary<string, string> ProcessCount();
    Dictionary<string, string> NetworkAdapters();
    Dictionary<string, string> DefenderStatus();
    Dictionary<string, string> Locale();
    Dictionary<string, string> Timezone();
    Dictionary<string, string> DesktopEnvironment();
    Dictionary<string, string> WindowManager();
    Dictionary<string, string> Terminal();
    Dictionary<string, string> ShellVersion();
    Dictionary<string, string> TerminalFont();
    Dictionary<string, string> Cursor();
    Dictionary<string, string> FirewallStatus();
}