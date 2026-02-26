using Microsoft.Maui.Devices;

namespace Microsoft.Maui.Essentials.WPF
{
	public class WPFDeviceDisplay : IDeviceDisplay
	{
		public bool KeepScreenOn { get; set; }

		public DisplayInfo MainDisplayInfo
		{
			get
			{
				var screen = System.Windows.SystemParameters.PrimaryScreenWidth;
				var height = System.Windows.SystemParameters.PrimaryScreenHeight;
				var dpi = GetDpi();
				var density = dpi / 96.0;
				return new DisplayInfo(
					width: screen * density,
					height: height * density,
					density: density,
					orientation: screen > height ? DisplayOrientation.Landscape : DisplayOrientation.Portrait,
					rotation: DisplayRotation.Rotation0,
					rate: 60);
			}
		}

		public event EventHandler<DisplayInfoChangedEventArgs>? MainDisplayInfoChanged;

		static double GetDpi()
		{
			using var source = new System.Windows.Interop.HwndSource(new System.Windows.Interop.HwndSourceParameters());
			var dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
			return dpiX;
		}
	}
}
