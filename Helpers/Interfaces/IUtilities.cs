namespace WinSight.Helpers.Interfaces;

public interface IUtilities
{
    string GetCommandOutput(string command, string args);
    string RunPowershellCommand(string command);
    string GetDefenderItemValue(string itemName);
    void AddItemToDict(string property, object value, Dictionary<string, List<object>> dict);
}