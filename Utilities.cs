﻿using System.ComponentModel;
using System.Diagnostics;

namespace WinSight;

public class Utilities : IUtilities
{
    public string GetCommandOutput(string command, string args)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process != null) return process.StandardOutput.ReadToEnd();
        }
        catch (Win32Exception)
        {
            return "";
        }

        return "";
    }

    public string RunPowershellCommand(string command)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-Command {command}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        return process.StandardOutput.ReadToEnd();
    }

    public string GetDefenderItemValue(string itemName)
    {
        var commandOutput = RunPowershellCommand("Get-MpComputerStatus");
        var lines = commandOutput.Split('\n');

        foreach (var line in lines)
            if (line.StartsWith(itemName))
                return line.Split(':')[1].Trim();

        return "Unknown";
    }
}