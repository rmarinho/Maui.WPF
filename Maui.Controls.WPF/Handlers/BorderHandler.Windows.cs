using Microsoft.Maui.Platform;
using WBorder = System.Windows.Controls.Border;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class BorderHandler : WPFViewHandler<IBorderView, WBorder>
	{
		protected override WBorder CreatePlatformView()
		{
			return new WBorder
			{
				BorderThickness = new System.Windows.Thickness(1),
				BorderBrush = System.Windows.Media.Brushes.Gray,
				Padding = new System.Windows.Thickness(8),
			};
		}

		public static void MapContent(BorderHandler handler, IBorderView border)
		{
			if (handler.MauiContext == null) return;

			if (border.PresentedContent is IView view)
			{
				handler.PlatformView.Child = (System.Windows.UIElement)view.ToPlatform(handler.MauiContext);
			}
		}

		public static void MapStroke(BorderHandler handler, IBorderView border)
		{
			if (border.Stroke is Microsoft.Maui.Graphics.SolidPaint solidPaint && solidPaint.Color != null)
			{
				var c = solidPaint.Color;
				handler.PlatformView.BorderBrush = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(c.Alpha * 255),
						(byte)(c.Red * 255),
						(byte)(c.Green * 255),
						(byte)(c.Blue * 255)));
			}
		}

		public static void MapStrokeThickness(BorderHandler handler, IBorderView border)
		{
			handler.PlatformView.BorderThickness = new System.Windows.Thickness(border.StrokeThickness);
		}

		public static void MapStrokeShape(BorderHandler handler, IBorderView border)
		{
			if (border.Shape is Microsoft.Maui.Controls.Shapes.RoundRectangle roundRect)
			{
				handler.PlatformView.CornerRadius = new System.Windows.CornerRadius(
					roundRect.CornerRadius.TopLeft,
					roundRect.CornerRadius.TopRight,
					roundRect.CornerRadius.BottomRight,
					roundRect.CornerRadius.BottomLeft);
			}
		}
	}
}
