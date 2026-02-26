using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Maui.Controls;
using MauiDevFlow.Agent.Core;
using Application = Microsoft.Maui.Controls.Application;

namespace Platform.Maui.WPF.DevFlow
{
	/// <summary>
	/// WPF-specific agent service with native tap and screenshot support.
	/// </summary>
	public class WpfDevFlowAgentService : DevFlowAgentService
	{
		public WpfDevFlowAgentService(AgentOptions? options = null) : base(options) { }

		protected override bool TryNativeTap(VisualElement ve)
		{
			try
			{
				var platformView = ve.Handler?.PlatformView;
				if (platformView is System.Windows.Controls.Button wpfButton)
				{
					wpfButton.Dispatcher.Invoke(() =>
					{
						var peer = new System.Windows.Automation.Peers.ButtonAutomationPeer(wpfButton);
						var invokeProvider = peer.GetPattern(System.Windows.Automation.Peers.PatternInterface.Invoke)
							as System.Windows.Automation.Provider.IInvokeProvider;
						invokeProvider?.Invoke();
					});
					return true;
				}
			}
			catch { }
			return false;
		}

		protected override async Task<byte[]?> CaptureScreenshotAsync(VisualElement rootElement)
		{
			try
			{
				var mauiWindow = Application.Current?.Windows.FirstOrDefault();
				if (mauiWindow?.Handler?.PlatformView is System.Windows.Window wpfWindow)
				{
					byte[]? result = null;
					wpfWindow.Dispatcher.Invoke(() =>
					{
						result = CaptureWpfWindow(wpfWindow);
					});
					if (result != null) return result;
				}
			}
			catch { }
			return await base.CaptureScreenshotAsync(rootElement);
		}

		private static byte[]? CaptureWpfWindow(System.Windows.Window window)
		{
			try
			{
				var content = window.Content as FrameworkElement;
				if (content == null) return null;

				var dpiX = VisualTreeHelper.GetDpi(content).PixelsPerInchX;
				var dpiY = VisualTreeHelper.GetDpi(content).PixelsPerInchY;
				var width = (int)(content.ActualWidth * dpiX / 96.0);
				var height = (int)(content.ActualHeight * dpiY / 96.0);
				if (width <= 0 || height <= 0) return null;

				var renderTarget = new RenderTargetBitmap(width, height, dpiX, dpiY, PixelFormats.Pbgra32);
				renderTarget.Render(content);

				var encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(renderTarget));

				using var ms = new MemoryStream();
				encoder.Save(ms);
				return ms.ToArray();
			}
			catch { return null; }
		}
	}
}
