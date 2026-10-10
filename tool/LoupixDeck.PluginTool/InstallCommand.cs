using System.IO.Compression;

namespace LoupixDeck.PluginTool;

/// <summary><c>loupix install</c>: packs the plugin and installs it into the LoupixDeck user plugins folder.</summary>
internal static class InstallCommand
{
    public const string Usage = "loupix install [--project <dir>] [--debug]";

    // Written by the host at runtime, not shipped in the package; an update keeps them
    // (PluginInstaller.PreservedFileNames).
    private static readonly string[] PreservedFileNames = ["settings.json", "store.json"];

    public static int Run(IReadOnlyList<string> args)
    {
        CommandLine cl = CommandLine.Parse(args, ["--project"], ["--debug"]);
        if (cl.Positional.Count > 0)
            throw new ToolException($"Usage: {Usage}");

        string projectDir = Path.GetFullPath(cl.Get("--project") ?? Directory.GetCurrentDirectory());
        DirectoryInfo work = Directory.CreateTempSubdirectory("loupix-install-");
        try
        {
            (PluginManifest manifest, string package) = PackCommand.Pack(projectDir, work.FullName);
            string target = Path.Combine(PluginsRoot(cl.Has("--debug")), manifest.Id);
            Install(package, target);
            Console.WriteLine($"Installed {manifest.Id} {manifest.Version} to {target}");
            Console.WriteLine("Restart LoupixDeck to load it.");
            return 0;
        }
        finally
        {
            work.Delete(recursive: true);
        }
    }

    /// <summary>
    /// The user plugins folder, resolved like FileDialogHelper.GetConfigDir in the host:
    /// <c>$HOME/.config/LoupixDeck[/debug]/plugins</c>, with the user profile when HOME is unset.
    /// </summary>
    internal static string PluginsRoot(bool debug, string? home = null)
    {
        home ??= Environment.GetEnvironmentVariable("HOME")
                 ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string configDir = Path.Combine(home, ".config", "LoupixDeck");
        return Path.Combine(debug ? Path.Combine(configDir, "debug") : configDir, "plugins");
    }

    internal static void Install(string package, string target)
    {
        Dictionary<string, byte[]> preserved = PreservedFileNames
            .Where(name => File.Exists(Path.Combine(target, name)))
            .ToDictionary(name => name, name => File.ReadAllBytes(Path.Combine(target, name)));

        try
        {
            if (Directory.Exists(target))
                Directory.Delete(target, recursive: true);
            ZipFile.ExtractToDirectory(package, target);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new ToolException($"Could not replace '{target}' ({ex.Message}). Close LoupixDeck and try again.");
        }

        foreach ((string name, byte[] content) in preserved)
            File.WriteAllBytes(Path.Combine(target, name), content);
    }
}
