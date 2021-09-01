namespace UtilityWpf.Controls.FileSystem
{
    public class FolderBrowser : PathBrowser
    {
        public FolderBrowser()
        {

        }

        protected override BrowserCommand Command { get; } = new FolderBrowserCommand();

    }
}