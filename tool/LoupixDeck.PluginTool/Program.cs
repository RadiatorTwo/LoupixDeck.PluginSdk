using LoupixDeck.PluginTool;

const string help = $"""
    loupix - LoupixDeck plugin tool

    Usage:
      {NewCommand.Usage}
          Create LoupixDeck.Plugin.<Name> against the latest LoupixDeck.PluginSdk.

      {PackCommand.Usage}
          Validate plugin.json, build, and write the Plugin Store files to dist/
          (<id>-<version>-<platform>.zip, plugin.json, SHA256SUMS).

      loupix --version
    """;

try
{
    return args switch
    {
        ["new", .. string[] rest] => await NewCommand.RunAsync(rest),
        ["pack", .. string[] rest] => PackCommand.Run(rest),
        ["--version"] => PrintVersion(),
        [] or ["--help"] or ["-h"] => PrintHelp(),
        _ => throw new ToolException($"Unknown command '{args[0]}'. Run 'loupix --help'.")
    };
}
catch (ToolException ex)
{
    Console.Error.WriteLine($"error: {ex.Message}");
    return 1;
}

int PrintHelp()
{
    Console.WriteLine(help);
    return 0;
}

int PrintVersion()
{
    Console.WriteLine($"loupix {SdkVersionResolver.Embedded}");
    return 0;
}
