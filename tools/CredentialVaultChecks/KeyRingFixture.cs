using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;

internal sealed class KeyRingFixture : IDisposable
{
    private readonly string tempRoot = Path.GetFullPath(Path.GetTempPath());
    private readonly string name = "HyperCredentialKeys_" + Guid.NewGuid().ToString("N");
    private bool created;
    internal string DirectoryPath { get; }

    internal KeyRingFixture()
    {
        DirectoryPath = Path.GetFullPath(Path.Combine(tempRoot, name));
        Guard();
        if (Directory.Exists(DirectoryPath)) throw new VaultCheckFailure("Generated key fixture already exists.");
        Directory.CreateDirectory(DirectoryPath);
        created = true;
    }
    internal IDataProtectionProvider Provider(string app = "Hyper.AdminPanel") =>
        DataProtectionProvider.Create(new DirectoryInfo(DirectoryPath), builder => builder.SetApplicationName(app));

    private void Guard()
    {
        if (!Regex.IsMatch(name, "\\AHyperCredentialKeys_[0-9a-f]{32}\\z", RegexOptions.CultureInvariant)
            || Path.GetFullPath(DirectoryPath) != Path.GetFullPath(Path.Combine(tempRoot, name))
            || Path.GetDirectoryName(DirectoryPath)?.TrimEnd(Path.DirectorySeparatorChar)
                != tempRoot.TrimEnd(Path.DirectorySeparatorChar))
            throw new VaultCheckFailure("Unsafe key fixture directory.");
    }
    public void Dispose()
    {
        if (!created) return;
        Guard();
        if ((File.GetAttributes(DirectoryPath) & FileAttributes.ReparsePoint) != 0
            || Directory.EnumerateFileSystemEntries(DirectoryPath).Any(entry =>
                (File.GetAttributes(entry) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != 0))
            throw new VaultCheckFailure("Unexpected entry in owned key fixture; cleanup refused.");
        Directory.Delete(DirectoryPath, recursive: true);
        created = false;
    }
}
