namespace WinSight.Config;

public class Settings
{
    public string OutputJsonPath { get; private set; }
    public string TableColor { get; private set; }

    public Settings(string outputJsonPath, string tableColor)
    {
        OutputJsonPath = outputJsonPath;
        TableColor = tableColor;
    }

}