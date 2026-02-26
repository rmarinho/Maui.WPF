using System.Windows.Controls;
using System.Windows.Media;
using WBorder = System.Windows.Controls.Border;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class BoxViewHandler : WPFViewHandler<IShapeView, WBorder>
	{
		protected override WBorder CreatePlatformView()
		{
			return new WBorder
			{
				MinWidth = 40,
				MinHeight = 40,
				Background = Brushes.Transparent,
			};
		}

		public static void MapFill(BoxViewHandler handler, IShapeView view)
		{
			if (view.Fill is Microsoft.Maui.Graphics.SolidPaint solidPaint && solidPaint.Color != null)
			{
				var c = solidPaint.Color;
				handler.PlatformView.Background = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(c.Alpha * 255),
						(byte)(c.Red * 255),
						(byte)(c.Green * 255),
						(byte)(c.Blue * 255)));
			}
		}
	}
}
