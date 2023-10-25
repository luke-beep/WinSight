namespace WinSight;

public interface IOperatingSystem
{
    string Kernel();
    string OSPlatform();
    string OSVersion();
    string Host();
    string UserName();
    string Uptime();
    string Shell();
    string Packages();
    string ProcessCount();
    string Network();
    string GetDefenderStatus();
    string Locale();
    string Timezone();
    string DesktopEnvironment();
    string WindowManager();
    string Terminal();
    string ShellVersion();
    string TerminalFont();
    string Cursor();
    Dictionary<string, string> GetFirewallStatus();
}