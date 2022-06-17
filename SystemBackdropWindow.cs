﻿using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using WinRT;

namespace MonitorControl
{
	public class SystemBackdropWindow : Window
	{
		public ApplicationTheme ActualTheme = ApplicationTheme.Dark;
		ElementTheme requestedTheme = ElementTheme.Default;
		public ElementTheme RequestedTheme
		{
			get => requestedTheme;
			set
			{
				requestedTheme = value;

				switch (value)
				{
					case ElementTheme.Dark: ActualTheme = ApplicationTheme.Dark; break;
					case ElementTheme.Light: ActualTheme = ApplicationTheme.Light; break;
					case ElementTheme.Default:
						var uiSettings = new Windows.UI.ViewManagement.UISettings();
						var defaultthemecolor = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
						ActualTheme = defaultthemecolor == Colors.Black ? ApplicationTheme.Dark : ApplicationTheme.Light;
						break;
				}
			}
		}

		public SystemBackdropWindow()
		{
			RequestedTheme = App.Current.RequestedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;

			m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
			m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

			SetBackdrop(BackdropType.Mica);
		}

		public enum BackdropType
		{
			Mica,
			DesktopAcrylic,
			DefaultColor,
		}

		WindowsSystemDispatcherQueueHelper m_wsdqHelper;
		BackdropType m_currentBackdrop;
		MicaController m_micaController;
		DesktopAcrylicController m_acrylicController;
		SystemBackdropConfiguration m_configurationSource;

		public void SetBackdrop(BackdropType type)
		{
			m_currentBackdrop = BackdropType.DefaultColor;
			if (m_micaController != null)
			{
				m_micaController.Dispose();
				m_micaController = null;
			}
			if (m_acrylicController != null)
			{
				m_acrylicController.Dispose();
				m_acrylicController = null;
			}
			this.Activated -= Window_Activated;
			this.Closed -= Window_Closed;
			m_configurationSource = null;

			if (type == BackdropType.Mica)
			{
				if (TrySetMicaBackdrop())
				{
					m_currentBackdrop = type;
				}
				else
				{
					type = BackdropType.DesktopAcrylic;
				}
			}
			if (type == BackdropType.DesktopAcrylic)
			{
				if (TrySetAcrylicBackdrop())
				{
					m_currentBackdrop = type;
				}
				else
				{
				}
			}
		}

		bool TrySetMicaBackdrop()
		{
			if (MicaController.IsSupported())
			{
				m_configurationSource = new SystemBackdropConfiguration();
				this.Activated += Window_Activated;
				this.Closed += Window_Closed;

				m_configurationSource.IsInputActive = true;
				switch (RequestedTheme)
				{
					case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; break;
					case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; break;
					case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; break;
				}

				m_micaController = new MicaController();
				m_micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
				m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
				return true;
			}

			return false;
		}

		bool TrySetAcrylicBackdrop()
		{
			if (DesktopAcrylicController.IsSupported())
			{
				m_configurationSource = new SystemBackdropConfiguration();
				this.Activated += Window_Activated;
				this.Closed += Window_Closed;

				m_configurationSource.IsInputActive = true;
				switch (RequestedTheme)
				{
					case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; break;
					case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; break;
					case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; break;
				}

				m_acrylicController = new DesktopAcrylicController();

				m_acrylicController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
				m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
				return true;
			}

			return false;
		}

		private void Window_Activated(object sender, WindowActivatedEventArgs args)
		{
			m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
		}

		private void Window_Closed(object sender, WindowEventArgs args)
		{
			if (m_micaController != null)
			{
				m_micaController.Dispose();
				m_micaController = null;
			}
			if (m_acrylicController != null)
			{
				m_acrylicController.Dispose();
				m_acrylicController = null;
			}
			this.Activated -= Window_Activated;
			m_configurationSource = null;
		}
	}

	class WindowsSystemDispatcherQueueHelper
	{
		[StructLayout(LayoutKind.Sequential)]
		struct DispatcherQueueOptions
		{
			internal int dwSize;
			internal int threadType;
			internal int apartmentType;
		}

		[DllImport("CoreMessaging.dll")]
		private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

		object m_dispatcherQueueController = null;
		public void EnsureWindowsSystemDispatcherQueueController()
		{
			if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
			{
				return;
			}

			if (m_dispatcherQueueController == null)
			{
				DispatcherQueueOptions options;
				options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
				options.threadType = 2;
				options.apartmentType = 2;

				CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
			}
		}
	}
}