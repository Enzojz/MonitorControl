using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace MonitorControl
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
        }

        public int test => 3;

        public InstanceCore Instance => App.Instance;
        public Setting SettingManager => App.SettingManager;

        private void ProfileLocationClick(object sender, EventArgs e)
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
