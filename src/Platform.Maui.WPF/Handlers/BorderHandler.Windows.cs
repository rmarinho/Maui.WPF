using System;
using System.Windows;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Platform.WPF;
using WBorder = System.Windows.Controls.Border;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class BorderHandler : WPFViewHandler<IBorderView, WBorder>
	{
		ContentPanel? _contentPanel;

		protected override WBorder CreatePlatformView()
		{
			return new WBorder
			{
				BorderThickness = new System.Windows.Thickness(1),
				BorderBrush = System.Windows.Media.Brushes.Gray,
				Padding = new System.Windows.Thickness(0),
			};
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);

			_contentPanel = new ContentPanel
			{
				CrossPlatformMeasure = VirtualView.CrossPlatformMeasure,
				CrossPlatformArrange = VirtualView.CrossPlatformArrange,
			};
			PlatformView.Child = _contentPanel;
		}

		static System.Windows.Media.SolidColorBrush? ToBrush(Microsoft.Maui.Graphics.Color? color)
		{
			if (color == null) return null;
			return new System.Windows.Media.SolidColorBrush(
				System.Windows.Media.Color.FromArgb(
					(byte)(color.Alpha * 255), (byte)(color.Red * 255),
					(byte)(color.Green * 255), (byte)(color.Blue * 255)));
		}

		public static void MapContent(BorderHandler handler, IBorderView border)
		{
			if (handler.MauiContext == null || handler._contentPanel == null) return;

			handler._contentPanel.Children.Clear();

			if (border.PresentedContent is IView view)
			{
				handler._contentPanel.Children.Add((UIElement)view.ToPlatform(handler.MauiContext));
			}
		}

		public static void MapStroke(BorderHandler handler, IBorderView border)
		{
			if (border.Stroke is Microsoft.Maui.Graphics.SolidPaint solidPaint)
			{
				var brush = ToBrush(solidPaint.Color);
				if (brush != null)
					handler.PlatformView.BorderBrush = brush;
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

		public static void MapBackground(BorderHandler handler, IBorderView border)
		{
			if (border.Background is SolidPaint solidPaint)
			{
				var brush = ToBrush(solidPaint.Color);
				if (brush != null)
					handler.PlatformView.Background = brush;
			}
		}

		public static void MapPadding(BorderHandler handler, IBorderView border)
		{
			var p = border.Padding;
			handler.PlatformView.Padding = new System.Windows.Thickness(p.Left, p.Top, p.Right, p.Bottom);
		}
	}
}
