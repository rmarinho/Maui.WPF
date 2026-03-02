#nullable enable
using System;
using System.Windows.Controls;
using Microsoft.Maui.Graphics;
using WRect = global::System.Windows.Rect;
using WSize = global::System.Windows.Size;

namespace Microsoft.Maui.Platform.WPF
{
	public class LayoutPanel : Panel
	{
		internal Func<double, double, Size>? CrossPlatformMeasure { get; set; }
		internal Func<Rect, Size>? CrossPlatformArrange { get; set; }

		public bool ClipsToBounds { get; set; }

		public LayoutPanel()
		{
		}

		protected override WSize MeasureOverride(WSize availableSize)
		{
			if (CrossPlatformMeasure == null)
			{
				return base.MeasureOverride(availableSize);
			}

			var width = availableSize.Width;
			var height = availableSize.Height;

			var crossPlatformSize = CrossPlatformMeasure(width, height);

			// WPF requires all children to be measured during MeasureOverride.
			// MAUI's CrossPlatformMeasure may not call WPF Measure on all children
			// (e.g., views with explicit WidthRequest/HeightRequest).
			foreach (System.Windows.UIElement child in InternalChildren)
			{
				child.Measure(availableSize);
			}

			width = crossPlatformSize.Width;
			height = crossPlatformSize.Height;

			return new WSize(width, height);
		}

		protected override WSize ArrangeOverride(WSize finalSize)
		{
			if (CrossPlatformArrange == null)
			{
				return base.ArrangeOverride(finalSize);
			}

			var width = finalSize.Width;
			var height = finalSize.Height;

			CrossPlatformArrange(new Rect(0, 0, width, height));

			return finalSize;
		}
	}
}
