using System.Windows;
using System.Windows.Controls;
using WProgressBar = System.Windows.Controls.ProgressBar;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ActivityIndicatorHandler : WPFViewHandler<IActivityIndicator, WProgressBar>
	{
		protected override WProgressBar CreatePlatformView()
		{
			return new WProgressBar
			{
				IsIndeterminate = true,
				Height = 4,
				MinWidth = 100,
			};
		}

		public static void MapIsRunning(ActivityIndicatorHandler handler, IActivityIndicator activityIndicator)
		{
			handler.PlatformView.IsIndeterminate = activityIndicator.IsRunning;
			handler.PlatformView.Visibility = activityIndicator.IsRunning ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
		}

		public static void MapColor(ActivityIndicatorHandler handler, IActivityIndicator activityIndicator)
		{
			if (activityIndicator.Color != null)
				handler.PlatformView.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(activityIndicator.Color.Alpha * 255),
						(byte)(activityIndicator.Color.Red * 255),
						(byte)(activityIndicator.Color.Green * 255),
						(byte)(activityIndicator.Color.Blue * 255)));
		}
	}
}
