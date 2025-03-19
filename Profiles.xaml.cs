using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MonitorControl
{
    /// <summary>
    /// Profiles.xaml 的交互逻辑
    /// </summary>
    public partial class Profiles : Page
    {
        public Profiles()
        {
            InitializeComponent();
            CreateButton.Command = new DelegateCommand(_ => (new CreateProfile()).ShowDialog());
            ImportButton.Command = new DelegateCommand(ImportClick);
            ExportButton.Command = new DelegateCommand(ExportClick);
        }

        public InstanceCore Instance => App.Instance;

        private void ImportClick(object parameter)
        {
            var ofn = new WinAPI.OPENFILENAME()
            {
                lStructSize = Marshal.SizeOf(typeof(WinAPI.OPENFILENAME)),
                lpstrFilter = "Monitor Control Profile (*.mcp)\0\0",
                lpstrFile = new string(new char[256]),
                lpstrFileTitle = new string(new char[64]),
                lpstrTitle = "Import Monitor Control Profiles",
                Flags = WinAPI.OPENFILENAME_FLAGS.OFN_ENABLESIZING
                | WinAPI.OPENFILENAME_FLAGS.OFN_EXPLORER
                | WinAPI.OPENFILENAME_FLAGS.OFN_NOCHANGEDIR
                | WinAPI.OPENFILENAME_FLAGS.OFN_PATHMUSTEXIST
            };
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;

            if (WinAPI.GetOpenFileName(ref ofn))
            {
                if (ofn.lpstrFile != null && ofn.lpstrFile.Length > 0 && File.Exists(ofn.lpstrFile))
                {
                    App.Instance.ReadProfile(ofn.lpstrFile);
                    App.Instance.Message = $"Profiles from {ofn.lpstrFile} have been imported!";
                }
            }
        }

        private void ExportClick(object parameter)
        {
            var ofn = new WinAPI.OPENFILENAME()
            {
                lStructSize = Marshal.SizeOf(typeof(WinAPI.OPENFILENAME)),
                lpstrFilter = "Monitor Control Profile (*.mcp)\0\0",
                lpstrFile = new string(new char[256]),
                lpstrFileTitle = new string(new char[64]),
                lpstrTitle = "Export Monitor Control Profiles",
                Flags = WinAPI.OPENFILENAME_FLAGS.OFN_ENABLESIZING
                | WinAPI.OPENFILENAME_FLAGS.OFN_EXPLORER
                | WinAPI.OPENFILENAME_FLAGS.OFN_NOCHANGEDIR
                | WinAPI.OPENFILENAME_FLAGS.OFN_PATHMUSTEXIST
                | WinAPI.OPENFILENAME_FLAGS.OFN_OVERWRITEPROMPT
            };
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;

            if (WinAPI.GetSaveFileName(ref ofn))
            {
                if (ofn.lpstrFile != null && ofn.lpstrFile.Length > 0)
                {
                    var path = ofn.lpstrFile;
                    if (Path.GetExtension(path).Length == 0)
                        path = Path.ChangeExtension(path, "mcp");
                    App.Instance.WriteProfile(path);
                    App.Instance.Message = $"Profiles have been exported to: {path}";
                }
            }
        }
    }
}
