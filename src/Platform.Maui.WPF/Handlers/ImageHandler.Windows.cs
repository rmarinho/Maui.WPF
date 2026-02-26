using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WImage = System.Windows.Controls.Image;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ImageHandler : WPFViewHandler<IImage, WImage>
	{
		protected override WImage CreatePlatformView() => new WImage();

		public static void MapSource(ImageHandler handler, IImage image)
		{
			if (image.Source == null)
				return;

			if (image.Source is IFileImageSource fileImageSource)
			{
				var fileName = fileImageSource.File;
				if (!string.IsNullOrEmpty(fileName))
				{
					try
					{
						var uri = new Uri(fileName, UriKind.RelativeOrAbsolute);
						handler.PlatformView.Source = new BitmapImage(uri);
					}
					catch
					{
						// Image not found
					}
				}
			}
		}

		public static void MapAspect(ImageHandler handler, IImage image)
		{
			handler.PlatformView.Stretch = image.Aspect switch
			{
				Aspect.AspectFit => System.Windows.Media.Stretch.Uniform,
				Aspect.AspectFill => System.Windows.Media.Stretch.UniformToFill,
				Aspect.Fill => System.Windows.Media.Stretch.Fill,
				_ => System.Windows.Media.Stretch.Uniform,
			};
		}
	}
}
