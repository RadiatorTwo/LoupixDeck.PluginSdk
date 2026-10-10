using System.Diagnostics;

namespace LoupixDeck.PluginTool;

internal static class ProcessRunner
{
    /// <summary>Runs a process to completion and returns its exit code and combined output.</summary>
    public static (int ExitCode, string Output) Run(string fileName, IEnumerable<string> arguments, string workingDirectory)
    {
        ProcessStartInfo info = new(fileName, arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using Process process = Process.Start(info)
                                ?? throw new ToolException($"Could not start '{fileName}'.");
        Task<string> stdout = process.StandardOutput.ReadToEndAsync();
        Task<string> stderr = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        return (process.ExitCode, stdout.Result + stderr.Result);
    }
}
