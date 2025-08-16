using PoeAuthenticator.AlphaVSS;

namespace PoeAuthenticator.Services;

/// <summary>
/// Interface for Volume Shadow Copy Service operations using AlphaVSS.
/// </summary>
public interface IAlphaVssService
{
    /// <summary>
    /// Creates a shadow copy of a file to bypass file locking issues.
    /// </summary>
    /// <param name="srcFilePath">The source file path to copy from.</param>
    /// <param name="destFilePath">The destination file path to copy to.</param>
    void ShadowCopyFile(string srcFilePath, string destFilePath);
}

/// <summary>
/// Implementation of Volume Shadow Copy Service operations for accessing locked files.
/// </summary>
public class AlphaVssService : IAlphaVssService
{
    /// <summary>
    /// Creates a shadow copy of the specified file to bypass file locking.
    /// </summary>
    /// <param name="srcFilePath">The source file path to copy from.</param>
    /// <param name="destFilePath">The destination file path to copy to.</param>
    public void ShadowCopyFile(string srcFilePath, string destFilePath)
    {
        // Initialize the shadow copy subsystem.
        using (VssBackup vss = new VssBackup())
        {
            vss.Setup(Path.GetPathRoot(srcFilePath)!);
            string snap_path = vss.GetSnapshotPath(srcFilePath);

            // Here we use the AlphaFS library to make the copy.
            Alphaleonis.Win32.Filesystem.File.Copy(snap_path, destFilePath, true);
        }
    }
}
