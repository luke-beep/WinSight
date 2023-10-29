using System.Text.Json;
using WinSight.Config;
using WinSight.Constants;
using WinSight.Helpers;
using WinSight.Helpers.Interfaces;
using WinSight.Logging;
using WinSight.Logging.Interfaces;
using WinSight.Providers;
using WinSight.Providers.Interfaces;
using WinSight.Services;
using OperatingSystem = WinSight.Services.OperatingSystem;

namespace WinSight;

internal class Program
{
    private static readonly IUtilities Utilities = new Utilities();

    private static readonly Settings Settings = new("output.json", ColorConstants.Yellow);
    private static readonly TableHandler TableHandler = new(Settings.TableColor);
    private static readonly Dictionary<string, List<object>> PropertiesDictionary = new();

    private static readonly IHardwareInfoProvider HardwareInfoProvider = new HardwareInfoProvider(new Hardware(), PropertiesDictionary, TableHandler);
    private static readonly IOperatingSystemInfoProvider OperatingSystemInfoProvider = new OperatingSystemInfoProvider(new OperatingSystem(), PropertiesDictionary, TableHandler);

    private static readonly ILogger Logger = new Logger();


    private static void Main(string[] args)
    {
        try
        {
            TableHandler.InitializeTable();
            HardwareInfoProvider.AddHardwareInformation();
            OperatingSystemInfoProvider.AddOperatingSystemInformation();

            OutputJson(Settings.OutputJsonPath, PropertiesDictionary, true);
            TableHandler.DisplayTable();
            Logger.Flush();
        }
        catch (Exception ex)
        {
            Logger.LogError($"An error occurred: {ex.Message} {ex.StackTrace}");
            Logger.Flush();
        }
    }

    private static void OutputJson(string outputPath, object information, bool watermark)
    {
        try
        {
            if (watermark) Utilities.AddItemToDict("WinSight", "https://github.com/luke-beep/WinSight", PropertiesDictionary);
            var jsonString = JsonSerializer.Serialize(information, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputPath, jsonString);
            Logger.LogInfo($"JSON output saved to {outputPath}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to save JSON to {outputPath}. Error: {ex.Message}");
        }
    }
}