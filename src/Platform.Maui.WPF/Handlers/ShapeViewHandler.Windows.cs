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

		static System.Windows.Media.Brush? ToPaintBrush(Microsoft.Maui.Graphics.Paint? paint)
		{
			if (paint is Microsoft.Maui.Graphics.SolidPaint sp && sp.Color != null)
				return ToBrush(sp.Color);
			return null;
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
			else if (shapeView is Microsoft.Maui.Controls.Shapes.Path pathShape)
			{
				try
				{
					var wpfPath = new System.Windows.Shapes.Path();
					if (pathShape.Data != null)
					{
						// MAUI Geometry.ToString() may produce path markup
						var geoStr = pathShape.Data.ToString();
						if (!string.IsNullOrEmpty(geoStr))
							wpfPath.Data = System.Windows.Media.Geometry.Parse(geoStr);
					}
					wpfPath.StrokeThickness = shapeView.StrokeThickness > 0 ? shapeView.StrokeThickness : 1;
					wpfPath.Stroke = ToPaintBrush(shapeView.Stroke);
					wpfPath.Fill = ToPaintBrush(shapeView.Fill);
					wpfPath.Stretch = System.Windows.Media.Stretch.Uniform;
					handler.PlatformView.Child = wpfPath;
				}
				catch { }
			}
			else if (shapeView is Microsoft.Maui.Controls.Shapes.Polygon polygon)
			{
				var wpfPolygon = new System.Windows.Shapes.Polygon();
				if (polygon.Points != null)
				{
					foreach (var pt in polygon.Points)
						wpfPolygon.Points.Add(new System.Windows.Point(pt.X, pt.Y));
				}
				wpfPolygon.StrokeThickness = shapeView.StrokeThickness > 0 ? shapeView.StrokeThickness : 1;
				wpfPolygon.Stroke = ToPaintBrush(shapeView.Stroke);
				wpfPolygon.Fill = ToPaintBrush(shapeView.Fill);
				wpfPolygon.Stretch = System.Windows.Media.Stretch.Uniform;
				handler.PlatformView.Child = wpfPolygon;
			}
			else if (shapeView is Microsoft.Maui.Controls.Shapes.Polyline polyline)
			{
				var wpfPolyline = new System.Windows.Shapes.Polyline();
				if (polyline.Points != null)
				{
					foreach (var pt in polyline.Points)
						wpfPolyline.Points.Add(new System.Windows.Point(pt.X, pt.Y));
				}
				wpfPolyline.StrokeThickness = shapeView.StrokeThickness > 0 ? shapeView.StrokeThickness : 1;
				wpfPolyline.Stroke = ToPaintBrush(shapeView.Stroke);
				wpfPolyline.Fill = ToPaintBrush(shapeView.Fill);
				wpfPolyline.Stretch = System.Windows.Media.Stretch.Uniform;
				handler.PlatformView.Child = wpfPolyline;
			}
		}

		public static void MapFill(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView is Microsoft.Maui.Controls.Shapes.Line)
				return;
			// For Path/Polygon/Polyline, fill is handled in MapShape
			if (shapeView is Microsoft.Maui.Controls.Shapes.Path or Microsoft.Maui.Controls.Shapes.Polygon or Microsoft.Maui.Controls.Shapes.Polyline)
			{
				MapShape(handler, shapeView);
				return;
			}

			if (shapeView.Fill is Microsoft.Maui.Graphics.SolidPaint solidPaint && solidPaint.Color != null)
			{
				handler.PlatformView.Background = ToBrush(solidPaint.Color);
			}
		}

		public static void MapStroke(ShapeViewHandler handler, IShapeView shapeView)
		{
			if (shapeView is Microsoft.Maui.Controls.Shapes.Line or Microsoft.Maui.Controls.Shapes.Path
				or Microsoft.Maui.Controls.Shapes.Polygon or Microsoft.Maui.Controls.Shapes.Polyline)
			{
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
			if (shapeView is Microsoft.Maui.Controls.Shapes.Line or Microsoft.Maui.Controls.Shapes.Path
				or Microsoft.Maui.Controls.Shapes.Polygon or Microsoft.Maui.Controls.Shapes.Polyline)
			{
				MapShape(handler, shapeView);
				return;
			}

			handler.PlatformView.BorderThickness = new System.Windows.Thickness(shapeView.StrokeThickness);
		}
	}
}
