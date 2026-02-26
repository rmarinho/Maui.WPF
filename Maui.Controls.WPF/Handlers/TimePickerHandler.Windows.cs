using System;
using System.Windows.Media;
using WTextBox = System.Windows.Controls.TextBox;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class TimePickerHandler : WPFViewHandler<ITimePicker, WTextBox>
	{
		protected override WTextBox CreatePlatformView()
		{
			return new WTextBox
			{
				MinWidth = 80,
				IsReadOnly = false,
			};
		}

		protected override void ConnectHandler(WTextBox platformView)
		{
			base.ConnectHandler(platformView);
			platformView.LostFocus += OnLostFocus;
		}

		protected override void DisconnectHandler(WTextBox platformView)
		{
			platformView.LostFocus -= OnLostFocus;
			base.DisconnectHandler(platformView);
		}

		void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (VirtualView == null) return;
			if (TimeSpan.TryParse(PlatformView.Text, out var time))
				VirtualView.Time = time;
		}

		public static void MapTime(TimePickerHandler handler, ITimePicker timePicker)
		{
			handler.PlatformView.Text = timePicker.Time.ToString(@"hh\:mm");
		}

		public static void MapFormat(TimePickerHandler handler, ITimePicker timePicker)
		{
		}

		public static void MapTextColor(TimePickerHandler handler, ITimePicker timePicker)
		{
			if (timePicker.TextColor != null)
				handler.PlatformView.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(timePicker.TextColor.Alpha * 255),
						(byte)(timePicker.TextColor.Red * 255),
						(byte)(timePicker.TextColor.Green * 255),
						(byte)(timePicker.TextColor.Blue * 255)));
		}
	}
}
