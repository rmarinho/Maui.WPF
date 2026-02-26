using WBorder = System.Windows.Controls.Border;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class BoxViewHandler : WPFViewHandler<IShapeView, WBorder>
	{
		protected override WBorder CreatePlatformView()
		{
			return new WBorder
			{
				Background = System.Windows.Media.Brushes.Transparent,
			};
		}

		static System.Windows.Media.SolidColorBrush? ToBrush(Microsoft.Maui.Graphics.Color? color)
		{
			if (color == null) return null;
			return new System.Windows.Media.SolidColorBrush(
				System.Windows.Media.Color.FromArgb(
					(byte)(color.Alpha * 255), (byte)(color.Red * 255),
					(byte)(color.Green * 255), (byte)(color.Blue * 255)));
		}

		public static void MapColor(BoxViewHandler handler, IShapeView view)
		{
			if (view.Fill is Microsoft.Maui.Graphics.SolidPaint solidPaint)
			{
				var brush = ToBrush(solidPaint.Color);
				if (brush != null)
					handler.PlatformView.Background = brush;
			}
		}

		public static void MapCornerRadius(BoxViewHandler handler, IShapeView view)
		{
			if (view.Shape is Microsoft.Maui.Controls.Shapes.RoundRectangle roundRect)
			{
				handler.PlatformView.CornerRadius = new System.Windows.CornerRadius(
					roundRect.CornerRadius.TopLeft,
					roundRect.CornerRadius.TopRight,
					roundRect.CornerRadius.BottomRight,
					roundRect.CornerRadius.BottomLeft);
			}
		}

		public static void MapBackground(BoxViewHandler handler, IShapeView view)
		{
			if (view.Background is SolidPaint solidPaint)
			{
				var brush = ToBrush(solidPaint.Color);
				if (brush != null)
					handler.PlatformView.Background = brush;
			}
		}
	}
}
