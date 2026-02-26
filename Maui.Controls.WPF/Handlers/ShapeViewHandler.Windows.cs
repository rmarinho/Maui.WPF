using WBorder = System.Windows.Controls.Border;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ShapeViewHandler : WPFViewHandler<IShapeView, WBorder>
	{
		protected override WBorder CreatePlatformView()
		{
			return new WBorder();
		}

		public static void MapShape(ShapeViewHandler handler, IShapeView shapeView)
		{
		}

		public static void MapFill(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView.Fill is Microsoft.Maui.Graphics.SolidPaint solidPaint && solidPaint.Color != null)
			{
				var c = solidPaint.Color;
				handler.PlatformView.Background = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(c.Alpha * 255),
						(byte)(c.Red * 255),
						(byte)(c.Green * 255),
						(byte)(c.Blue * 255)));
			}
		}

		public static void MapStroke(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView.Stroke is Microsoft.Maui.Graphics.SolidPaint solidPaint && solidPaint.Color != null)
			{
				var c = solidPaint.Color;
				handler.PlatformView.BorderBrush = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(c.Alpha * 255),
						(byte)(c.Red * 255),
						(byte)(c.Green * 255),
						(byte)(c.Blue * 255)));
			}
		}

		public static void MapStrokeThickness(ShapeViewHandler handler, IShapeView shapeView)
		{
			handler.PlatformView.BorderThickness = new System.Windows.Thickness(shapeView.StrokeThickness);
		}
	}
}
