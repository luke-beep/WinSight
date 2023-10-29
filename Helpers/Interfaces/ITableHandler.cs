namespace WinSight.Helpers.Interfaces;

public interface ITableHandler
{
    public void InitializeTable();
    public void AddRow(string property, string value);
    public void DisplayTable();
}