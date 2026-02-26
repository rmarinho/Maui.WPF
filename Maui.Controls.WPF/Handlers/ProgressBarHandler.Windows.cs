using System.Windows.Media;
using WProgressBar = System.Windows.Controls.ProgressBar;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ProgressBarHandler : WPFViewHandler<IProgress, WProgressBar>
	{
		protected override WProgressBar CreatePlatformView()
		{
			return new WProgressBar
			{
				Minimum = 0,
				Maximum = 1,
				Height = 4,
				MinWidth = 100,
			};
		}

		public static void MapProgress(ProgressBarHandler handler, IProgress progress)
		{
			handler.PlatformView.Value = progress.Progress;
		}

		public static void MapProgressColor(ProgressBarHandler handler, IProgress progress)
		{
			if (progress.ProgressColor != null)
				handler.PlatformView.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(progress.ProgressColor.Alpha * 255),
						(byte)(progress.ProgressColor.Red * 255),
						(byte)(progress.ProgressColor.Green * 255),
						(byte)(progress.ProgressColor.Blue * 255)));
		}
	}
}
