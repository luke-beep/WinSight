using Spectre.Console;
using WinSight.Helpers.Interfaces;
using WinSight.Services.Interfaces;

namespace WinSight.Helpers;

public class TableHandler : ITableHandler
{
    private readonly Table _table;
    private readonly string _color;
    private readonly IOperatingSystem _operatingSystem;

    public TableHandler(string color)
    {
        _color = color;
        _table = new Table();
        _operatingSystem = new Services.OperatingSystem();
    }

    public void InitializeTable()
    {
        _table.AddColumns(new TableColumn("[yellow]Property[/]").Centered(), new TableColumn("[yellow]Value[/]").Centered())
            .Centered()
            .Border(TableBorder.DoubleEdge)
            .Title($"[yellow]WinSight ({_operatingSystem.UserName().Values.First().ToLower()}@{_operatingSystem.Host().Values.First().ToLower()})[/]");
    }

    public void AddRow(string property, string value)
    {
        _table.AddRow(new Markup($"[{_color}]{property}[/]"), new Markup($"[{_color}]{value}[/]"));
    }
    
    public void DisplayTable()
    {
        AnsiConsole.Write(_table);
    }
}