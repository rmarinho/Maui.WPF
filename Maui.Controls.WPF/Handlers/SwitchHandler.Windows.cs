using WCheckBox = System.Windows.Controls.CheckBox;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class SwitchHandler : WPFViewHandler<ISwitch, WCheckBox>
	{
		protected override WCheckBox CreatePlatformView()
		{
			return new WCheckBox
			{
				Content = "Toggle",
			};
		}

		protected override void ConnectHandler(WCheckBox platformView)
		{
			base.ConnectHandler(platformView);
			platformView.Checked += OnToggled;
			platformView.Unchecked += OnToggled;
		}

		protected override void DisconnectHandler(WCheckBox platformView)
		{
			platformView.Checked -= OnToggled;
			platformView.Unchecked -= OnToggled;
			base.DisconnectHandler(platformView);
		}

		void OnToggled(object sender, System.Windows.RoutedEventArgs e)
		{
			if (VirtualView == null) return;
			VirtualView.IsOn = PlatformView.IsChecked ?? false;
		}

		public static void MapIsOn(SwitchHandler handler, ISwitch @switch)
		{
			if (handler.PlatformView.IsChecked != @switch.IsOn)
				handler.PlatformView.IsChecked = @switch.IsOn;
		}

		public static void MapTrackColor(SwitchHandler handler, ISwitch @switch)
		{
			if (@switch.TrackColor != null)
				handler.PlatformView.Background = ToBrush(@switch.TrackColor);
		}

		public static void MapThumbColor(SwitchHandler handler, ISwitch @switch)
		{
			// WPF CheckBox does not expose a native thumb element
		}

		static System.Windows.Media.SolidColorBrush? ToBrush(Microsoft.Maui.Graphics.Color? color)
		{
			if (color == null) return null;
			return new System.Windows.Media.SolidColorBrush(
				System.Windows.Media.Color.FromArgb(
					(byte)(color.Alpha * 255), (byte)(color.Red * 255),
					(byte)(color.Green * 255), (byte)(color.Blue * 255)));
		}
	}
}
