using LoupixDeck.PluginTool;

const string help = $"""
    loupix - LoupixDeck plugin tool

    Usage:
      {NewCommand.Usage}
          Create LoupixDeck.Plugin.<Name> against the latest LoupixDeck.PluginSdk.

      loupix --version
    """;

try
{
    return args switch
    {
        ["new", .. string[] rest] => await NewCommand.RunAsync(rest),
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
