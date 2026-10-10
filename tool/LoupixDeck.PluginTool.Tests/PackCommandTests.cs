using LoupixDeck.PluginTool;

namespace LoupixDeck.PluginTool.Tests;

public sealed class PackCommandTests
{
    [Fact]
    public void ChecksumLine_UsesSha256SumFormat()
    {
        // Arrange
        string file = Path.Combine(Path.GetTempPath(), $"loupix-test-{Guid.NewGuid():N}.txt");
        File.WriteAllText(file, "abc");

        try
        {
            // Act
            string line = PackCommand.ChecksumLine(file);

            // Assert: sha256("abc")
            Assert.Equal($"ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad  {Path.GetFileName(file)}\n", line);
        }
        finally
        {
            File.Delete(file);
        }
    }
}
