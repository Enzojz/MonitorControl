using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;

namespace MonitorControl
{
    /// <summary>
    /// CreateProfile.xaml 的交互逻辑
    /// </summary>
    public partial class CreateProfile : Window
    {
        public CreateProfile()
        {
            InitializeComponent();

            WindowChrome.SetWindowChrome(
                this,
                new WindowChrome
                {
                    CaptionHeight = 40,
                    CornerRadius = default,
                    GlassFrameThickness = new Thickness(-1),
                    ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                    UseAeroCaptionButtons = true
                }
            );

            (new WindowInteropHelper(this)).EnsureHandle();
        }

        private void ProfileNameChanged(object sender, TextChangedEventArgs e)
        {
            this.CreateButton.IsEnabled = App.Instance.CanSaveProfile(ProfileNameTextBox.Text);
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            if (App.Instance.CanSaveProfile(ProfileNameTextBox.Text))
            {
                App.Instance.SaveProfile(ProfileNameTextBox.Text);
            }
            this.Close();
        }
    }
}
