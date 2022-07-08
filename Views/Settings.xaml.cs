using Microsoft.UI.Xaml.Controls;
using System.IO;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        internal InstanceCore Instance => App.Instance;
        internal Setting SettingManager => App.SettingManager;

        private void ProfileLocationClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var ofn = new WinAPI.OPENFILENAME()
            {
                lStructSize = Marshal.SizeOf(typeof(WinAPI.OPENFILENAME)),
                lpstrFilter = "Monitor Control Profile (*.mcp)\0\0",
                lpstrFile = new string(new char[256]),
                lpstrFileTitle = new string(new char[64]),
                lpstrTitle = "Change Monitor Control Profile Location",
                Flags = WinAPI.OPENFILENAME_FLAGS.OFN_ENABLESIZING | WinAPI.OPENFILENAME_FLAGS.OFN_EXPLORER | WinAPI.OPENFILENAME_FLAGS.OFN_NOCHANGEDIR | WinAPI.OPENFILENAME_FLAGS.OFN_PATHMUSTEXIST
                
            };
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            if (SettingManager.ProfilePath != null)
                ofn.lpstrInitialDir = SettingManager.ProfilePath;

            if (WinAPI.GetSaveFileName(ref ofn))
            {
                if (ofn.lpstrFile != null && ofn.lpstrFile.Length > 0)
                {
                    var path = ofn.lpstrFile;
                    if (Path.GetExtension(path).Length == 0)
                        path = Path.ChangeExtension(path, "mcp");

                    SettingManager.ProfilePath = path;
                }
            }

        }
    }
}
