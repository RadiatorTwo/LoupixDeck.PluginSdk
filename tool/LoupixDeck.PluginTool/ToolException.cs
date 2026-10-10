namespace LoupixDeck.PluginTool;

/// <summary>An expected failure: Program prints the message and exits with code 1, no stack trace.</summary>
internal sealed class ToolException(string message) : Exception(message);
