using Spectre.Console;
using WinSight.Constants;
using WinSight.Logging.Interfaces;

namespace WinSight.Logging;

public class Logger : ILogger
{
    private static readonly List<string> LogBuffer = new();
    public void LogInfo(string message)
    {
        LogBuffer.Add($"[{ColorConstants.White}]INFO: {message}[/]");
    }

    public void LogError(string message)
    {
        LogBuffer.Add($"[{ColorConstants.Red}]ERROR: {message}[/]");
    }

    public void LogWarning(string message)
    {
        LogBuffer.Add($"[{ColorConstants.Yellow}]WARNING: {message}[/]");
    }

    public void Flush()
    {
        foreach (var logMessage in LogBuffer)
        {
            try
            {
                AnsiConsole.Write(new Markup(logMessage));
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Failed to print log message with markup: {logMessage}");
                AnsiConsole.WriteLine($"Error: {ex.Message}");
            }
        }
        LogBuffer.Clear();
    }
}