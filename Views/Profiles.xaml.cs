using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Profiles : Page
    {
        public Profiles()
        {
            this.InitializeComponent();
        }

        internal ProfileState CreateNew = new ProfileState(null, null);

        internal InstanceCore Instance => App.Instance;

        private void ImportClick(object sender, RoutedEventArgs e)
        {

            var ofn = new WinAPI.OPENFILENAME()
            {
                lStructSize = Marshal.SizeOf(typeof(WinAPI.OPENFILENAME)),
                lpstrFilter = "Monitor Control Profile (*.mcp)\0\0",
                lpstrFile = new string(new char[256]),
                lpstrFileTitle = new string(new char[64]),
                lpstrTitle = "Import Monitor Control Profiles"
            };
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;

            if (WinAPI.GetOpenFileName(ref ofn))
            {
                if (ofn.lpstrFile != null && ofn.lpstrFile.Length > 0 && File.Exists(ofn.lpstrFile))
                {
                    App.Instance.ReadProfile(ofn.lpstrFile);
                    App.Instance.Message = String.Format("Profiles from {0} have been imported!", ofn.lpstrFile);
                }
            }
        }
        private void ExportClick(object sender, RoutedEventArgs e)
        {
            var ofn = new WinAPI.OPENFILENAME()
            {
                lStructSize = Marshal.SizeOf(typeof(WinAPI.OPENFILENAME)),
                lpstrFilter = "Monitor Control Profile (*.mcp)\0\0",
                lpstrFile = new string(new char[256]),
                lpstrFileTitle = new string(new char[64]),
                lpstrTitle = "Export Monitor Control Profiles"
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
                    App.Instance.Message = String.Format("Profiles have been exported to: {0}", path);
                }
            }
        }

        public void ConfirmCreateByEnter(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                CreateNewFlyout.Hide();
                CreateNew.ConfirmEditByEnter(sender, e);
            }
        }
        public void ConfirmCreateByClick(object sender, RoutedEventArgs e)
        {
            CreateNewFlyout.Hide();
            CreateNew.Save(sender, e);
        }
    }
}
