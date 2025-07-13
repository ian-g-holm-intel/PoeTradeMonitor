using PoeAuthenticator.AlphaVSS;

namespace PoeAuthenticator.Services;

public interface IAlphaVssService
{
    void ShadowCopyFile(string srcFilePath, string destFilePath);
}

public class AlphaVssService : IAlphaVssService
{
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
