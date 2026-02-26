using System;
using System.IO;
using System.Threading.Tasks;
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
						// Try absolute/relative URI first
						var uri = new Uri(fileName, UriKind.RelativeOrAbsolute);
						if (!uri.IsAbsoluteUri)
						{
							// Try app directory
							var appDir = AppDomain.CurrentDomain.BaseDirectory;
							var fullPath = Path.Combine(appDir, fileName);
							if (File.Exists(fullPath))
								uri = new Uri(fullPath, UriKind.Absolute);
							else
							{
								// Try Resources folder
								var resPath = Path.Combine(appDir, "Resources", "Images", fileName);
								if (File.Exists(resPath))
									uri = new Uri(resPath, UriKind.Absolute);
							}
						}
						handler.PlatformView.Source = new BitmapImage(uri);
					}
					catch { }
				}
			}
			else if (image.Source is IUriImageSource uriImageSource)
			{
				try
				{
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.UriSource = uriImageSource.Uri;
					bmp.CacheOption = BitmapCacheOption.OnLoad;
					bmp.EndInit();
					handler.PlatformView.Source = bmp;
				}
				catch { }
			}
			else if (image.Source is IStreamImageSource streamImageSource)
			{
				_ = LoadStreamImageAsync(handler, streamImageSource);
			}
		}

		static async Task LoadStreamImageAsync(ImageHandler handler, IStreamImageSource streamSource)
		{
			try
			{
				var stream = await streamSource.GetStreamAsync(System.Threading.CancellationToken.None);
				if (stream == null) return;

				await handler.PlatformView.Dispatcher.InvokeAsync(() =>
				{
					try
					{
						var bmp = new BitmapImage();
						bmp.BeginInit();
						bmp.StreamSource = stream;
						bmp.CacheOption = BitmapCacheOption.OnLoad;
						bmp.EndInit();
						bmp.Freeze();
						handler.PlatformView.Source = bmp;
					}
					catch { }
				});
			}
			catch { }
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
