namespace WinSight;

public interface IUtilities
{
    string GetCommandOutput(string command, string args);
    string RunPowershellCommand(string command);
    string GetDefenderItemValue(string itemName);
}