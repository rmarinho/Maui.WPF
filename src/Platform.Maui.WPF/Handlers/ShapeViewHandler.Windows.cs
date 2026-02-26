using WBorder = System.Windows.Controls.Border;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ShapeViewHandler : WPFViewHandler<IShapeView, WBorder>
	{
		protected override WBorder CreatePlatformView()
		{
			return new WBorder();
		}

		static System.Windows.Media.SolidColorBrush? ToBrush(Microsoft.Maui.Graphics.Color? color)
		{
			if (color == null) return null;
			return new System.Windows.Media.SolidColorBrush(
				System.Windows.Media.Color.FromArgb(
					(byte)(color.Alpha * 255), (byte)(color.Red * 255),
					(byte)(color.Green * 255), (byte)(color.Blue * 255)));
		}

		public static void MapShape(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView is Microsoft.Maui.Controls.Shapes.Ellipse)
			{
				handler.PlatformView.CornerRadius = new System.Windows.CornerRadius(9999);
			}
			else if (shapeView is Microsoft.Maui.Controls.Shapes.RoundRectangle rr)
			{
				handler.PlatformView.CornerRadius = new System.Windows.CornerRadius(
					rr.CornerRadius.TopLeft, rr.CornerRadius.TopRight,
					rr.CornerRadius.BottomRight, rr.CornerRadius.BottomLeft);
			}
			else if (shapeView is Microsoft.Maui.Controls.Shapes.Line line)
			{
				// Render Line as a thin border rotated to match line angle
				var wpfLine = new System.Windows.Shapes.Line
				{
					X1 = line.X1, Y1 = line.Y1,
					X2 = line.X2, Y2 = line.Y2,
					StrokeThickness = shapeView.StrokeThickness > 0 ? shapeView.StrokeThickness : 1,
				};
				if (shapeView.Stroke is Microsoft.Maui.Graphics.SolidPaint sp && sp.Color != null)
					wpfLine.Stroke = ToBrush(sp.Color);
				handler.PlatformView.Child = wpfLine;
			}
		}

		public static void MapFill(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView is Microsoft.Maui.Controls.Shapes.Line)
				return; // Lines don't have fill

			if (shapeView.Fill is Microsoft.Maui.Graphics.SolidPaint solidPaint && solidPaint.Color != null)
			{
				handler.PlatformView.Background = ToBrush(solidPaint.Color);
			}
		}

		public static void MapStroke(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView is Microsoft.Maui.Controls.Shapes.Line)
			{
				// Re-map shape to update line stroke
				MapShape(handler, shapeView);
				return;
			}

			if (shapeView.Stroke is Microsoft.Maui.Graphics.SolidPaint solidPaint && solidPaint.Color != null)
			{
				handler.PlatformView.BorderBrush = ToBrush(solidPaint.Color);
			}
		}

		public static void MapStrokeThickness(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView is Microsoft.Maui.Controls.Shapes.Line)
			{
				MapShape(handler, shapeView);
				return;
			}

			handler.PlatformView.BorderThickness = new System.Windows.Thickness(shapeView.StrokeThickness);
		}
	}
}
