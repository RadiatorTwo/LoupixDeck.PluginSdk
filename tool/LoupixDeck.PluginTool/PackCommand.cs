using System.IO.Compression;
using System.Security.Cryptography;

namespace LoupixDeck.PluginTool;

/// <summary>
/// <c>loupix pack</c>: builds the plugin and writes the Plugin Store files, the same three files
/// plugin-release.yml attaches to a release: the package zip, plugin.json and SHA256SUMS.
/// </summary>
internal static class PackCommand
{
    public const string Usage = "loupix pack [--project <dir>] [-o <dir>]";

    private const string SdkAssembly = "LoupixDeck.PluginSdk.dll";

    public static int Run(IReadOnlyList<string> args)
    {
        CommandLine cl = CommandLine.Parse(
            args, ["--project", "--output"], [], new Dictionary<string, string> { ["-o"] = "--output" });
        if (cl.Positional.Count > 0)
            throw new ToolException($"Usage: {Usage}");

        string projectDir = Path.GetFullPath(cl.Get("--project") ?? Directory.GetCurrentDirectory());
        string outputDir = Path.GetFullPath(cl.Get("--output") ?? Path.Combine(projectDir, "dist"));

        (PluginManifest manifest, string package) = Pack(projectDir, outputDir);
        Console.WriteLine($"Packed {manifest.Id} {manifest.Version}: {package}");
        return 0;
    }

    /// <summary>Builds and packs the plugin in <paramref name="projectDir"/>; returns the manifest and the zip path.</summary>
    public static (PluginManifest Manifest, string Package) Pack(string projectDir, string outputDir)
    {
        PluginManifest manifest = PluginManifest.Load(Path.Combine(projectDir, "plugin.json"));
        string project = FindProject(projectDir, manifest);

        DirectoryInfo work = Directory.CreateTempSubdirectory("loupix-pack-");
        try
        {
            string build = Path.Combine(work.FullName, "build");
            Console.WriteLine($"Building {Path.GetFileName(project)}...");
            (int exitCode, string output) = ProcessRunner.Run(
                "dotnet",
                ["build", project, "-c", "Release", "-o", build, "-p:DebugSymbols=false", "-p:DebugType=none", "--nologo", "-v", "quiet"],
                projectDir);
            if (exitCode != 0)
                throw new ToolException($"Build failed:{Environment.NewLine}{output.Trim()}");

            string staging = Path.Combine(work.FullName, "staging");
            Stage(build, staging, Path.Combine(projectDir, "plugin.json"));
            if (!File.Exists(Path.Combine(staging, manifest.EntryAssembly)))
                throw new ToolException($"Entry assembly '{manifest.EntryAssembly}' is missing from the build output.");

            Directory.CreateDirectory(outputDir);
            foreach (string stale in Directory.EnumerateFiles(outputDir, $"{manifest.Id}-*.zip"))
                File.Delete(stale);

            string package = Path.Combine(outputDir, manifest.PackageFileName);
            ZipFile.CreateFromDirectory(staging, package);
            string manifestCopy = Path.Combine(outputDir, "plugin.json");
            File.Copy(Path.Combine(projectDir, "plugin.json"), manifestCopy, overwrite: true);
            File.WriteAllText(
                Path.Combine(outputDir, "SHA256SUMS"),
                ChecksumLine(package) + ChecksumLine(manifestCopy));
            return (manifest, package);
        }
        finally
        {
            work.Delete(recursive: true);
        }
    }

    /// <summary>One line in <c>sha256sum</c> format: lowercase hash, two spaces, file name.</summary>
    internal static string ChecksumLine(string file)
    {
        using FileStream stream = File.OpenRead(file);
        return $"{Convert.ToHexStringLower(SHA256.HashData(stream))}  {Path.GetFileName(file)}\n";
    }

    // Everything the build produced except symbols and runtimeconfig files; the SDK is never
    // shipped, the host provides it.
    private static void Stage(string build, string staging, string manifestFile)
    {
        Directory.CreateDirectory(staging);
        foreach (string file in Directory.EnumerateFiles(build))
        {
            string name = Path.GetFileName(file);
            if (!name.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase)
                && !name.EndsWith(".runtimeconfig.json", StringComparison.OrdinalIgnoreCase)
                && !name.Equals(SdkAssembly, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(file, Path.Combine(staging, name));
            }
        }

        foreach (string dir in Directory.EnumerateDirectories(build))
            CopyDirectory(dir, Path.Combine(staging, Path.GetFileName(dir)));

        File.Copy(manifestFile, Path.Combine(staging, "plugin.json"), overwrite: true);
    }

    private static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (string file in Directory.EnumerateFiles(source))
            File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
        foreach (string dir in Directory.EnumerateDirectories(source))
            CopyDirectory(dir, Path.Combine(target, Path.GetFileName(dir)));
    }

    // The project named after the entry assembly, otherwise the only project in the folder.
    private static string FindProject(string projectDir, PluginManifest manifest)
    {
        string named = Path.Combine(projectDir, Path.ChangeExtension(manifest.EntryAssembly, ".csproj"));
        if (File.Exists(named))
            return named;

        string[] projects = Directory.GetFiles(projectDir, "*.csproj");
        return projects.Length == 1
            ? projects[0]
            : throw new ToolException($"Expected one .csproj in '{projectDir}', found {projects.Length}. Name it after the entry assembly.");
    }
}
