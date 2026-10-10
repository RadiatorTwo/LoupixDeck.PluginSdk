# LoupixDeck.PluginTool

Command line tool for LoupixDeck plugin development. A single native executable (NativeAOT) for
win-x64, linux-x64, osx-x64 and osx-arm64; installing it needs the .NET 10 SDK or later.

```bash
dotnet tool install -g LoupixDeck.PluginTool
loupix new SpotifyPremium
```

`loupix new <Name>` creates `LoupixDeck.Plugin.<Name>`: project, solution, plugin class, `plugin.json`,
README and a release workflow for the Plugin Store. The project references the latest stable
`LoupixDeck.PluginSdk` on nuget.org (or the version this tool was released with when offline).

Run `loupix --help` for all commands and options.
