using System.ComponentModel;

namespace LoupixDeck.PluginTool;

/// <summary><c>loupix new</c>: scaffolds a build-ready plugin project.</summary>
internal static class NewCommand
{
    public const string Usage =
        "loupix new <Name> [-o <dir>] [--platform All|Windows|Linux|macOS] [--author <name>] [--sdk-version <x.y.z>] [--local-feed <dir>] [--git]";

    private static readonly string[] Platforms = ["All", "Windows", "Linux", "macOS"];

    public static async Task<int> RunAsync(IReadOnlyList<string> args)
    {
        CommandLine cl = CommandLine.Parse(
            args,
            ["--output", "--platform", "--author", "--sdk-version", "--local-feed"],
            ["--git"],
            new Dictionary<string, string> { ["-o"] = "--output" });
        if (cl.Positional.Count != 1)
            throw new ToolException($"Usage: {Usage}");

        PluginName name = PluginName.Parse(cl.Positional[0]);
        string platform = ParsePlatform(cl.Get("--platform"));
        string author = ParseAuthor(cl.Get("--author"));

        string target = Path.GetFullPath(Path.Combine(cl.Get("--output") ?? Directory.GetCurrentDirectory(), name.ProjectName));
        if (Directory.Exists(target) || File.Exists(target))
            throw new ToolException($"'{target}' already exists. Nothing was created.");

        (Version sdk, string source) = await ResolveSdkVersionAsync(cl.Get("--sdk-version"));
        string? localFeed = cl.Get("--local-feed") is { } feed ? FeedPath(target, Path.GetFullPath(feed)) : null;

        foreach ((string path, string content) in Scaffold(name, platform, author, sdk, localFeed))
        {
            string file = Path.Combine(target, path);
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            File.WriteAllText(file, content);
        }

        if (cl.Has("--git"))
            InitGit(target);

        Console.WriteLine($"Created {name.ProjectName} (id '{name.Id}', LoupixDeck.PluginSdk {Format(sdk)} from {source}).");
        Console.WriteLine($"Next: cd {DisplayPath(target)} && dotnet build");
        if (localFeed is not null)
            Console.WriteLine("nuget.config points at a local feed; delete it before a store release so CI restores from nuget.org.");
        return 0;
    }

    /// <summary>The files of a new plugin, keyed by path relative to the project folder.</summary>
    internal static Dictionary<string, string> Scaffold(PluginName name, string platform, string author, Version sdk, string? localFeed)
    {
        Dictionary<string, string> values = new()
        {
            ["ProjectName"] = name.ProjectName,
            ["Namespace"] = name.ProjectName,
            ["ClassName"] = name.ClassName,
            ["Id"] = name.Id,
            ["DisplayName"] = name.Name,
            ["Platform"] = platform,
            ["Author"] = author,
            ["SdkVersion"] = Format(sdk),
            ["SdkMajor"] = sdk.Major.ToString(),
            ["SdkMinor"] = sdk.Minor.ToString(),
            ["LocalFeed"] = localFeed ?? ""
        };

        Dictionary<string, string> files = new()
        {
            [$"{name.ProjectName}.csproj"] = "Plugin.csproj.tmpl",
            [$"{name.ProjectName}.slnx"] = "Plugin.slnx.tmpl",
            [$"{name.ClassName}.cs"] = "Plugin.cs.tmpl",
            ["plugin.json"] = "plugin.json.tmpl",
            [".gitignore"] = "gitignore.tmpl",
            ["README.md"] = "README.md.tmpl",
            ["CLAUDE.md"] = "CLAUDE.md.tmpl",
            [".github/workflows/release.yml"] = "release.yml.tmpl"
        };
        if (localFeed is not null)
            files["nuget.config"] = "nuget.config.tmpl";

        return files.ToDictionary(f => f.Key, f => Templates.Render(Templates.Read(f.Value), values));
    }

    private static async Task<(Version, string)> ResolveSdkVersionAsync(string? requested)
    {
        if (requested is not null)
        {
            if (!Version.TryParse(requested, out Version? version) || version.Build < 0 || version.Revision >= 0)
                throw new ToolException($"Invalid --sdk-version '{requested}'. Use major.minor.patch, e.g. 1.31.0.");
            return (version, "--sdk-version");
        }

        using HttpClient http = new() { Timeout = TimeSpan.FromSeconds(5) };
        return await SdkVersionResolver.ResolveAsync(http);
    }

    private static string ParsePlatform(string? platform)
    {
        if (platform is null)
            return "All";

        return Platforms.FirstOrDefault(p => p.Equals(platform, StringComparison.OrdinalIgnoreCase))
               ?? throw new ToolException($"Invalid --platform '{platform}'. Use All, Windows, Linux or macOS.");
    }

    // The author lands in a JSON string and a C# string literal, so keep it to plain text.
    private static string ParseAuthor(string? author)
    {
        if (author is null)
            return "";
        if (author.Any(c => c is '"' or '\\' || char.IsControl(c)))
            throw new ToolException("--author must not contain quotes, backslashes or control characters.");
        return author;
    }

    private static void InitGit(string target)
    {
        try
        {
            (int exitCode, string output) = ProcessRunner.Run("git", ["init", "--quiet"], target);
            if (exitCode != 0)
                Console.Error.WriteLine($"warning: git init failed: {output.Trim()}");
        }
        catch (Win32Exception)
        {
            Console.Error.WriteLine("warning: git not found, repository not initialized.");
        }
    }

    // nuget.config resolves relative paths against its own folder. A relative path keeps the
    // side-by-side repo layout portable; anything further away stays absolute.
    private static string FeedPath(string target, string feed)
    {
        string relative = Path.GetRelativePath(target, feed);
        return relative.Split(Path.DirectorySeparatorChar).Count(s => s == "..") <= 2 ? relative : feed;
    }

    private static string DisplayPath(string path)
    {
        string relative = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);
        return relative.StartsWith("..", StringComparison.Ordinal) ? path : relative;
    }

    private static string Format(Version version) => $"{version.Major}.{version.Minor}.{version.Build}";
}
