using WButton = System.Windows.Controls.Button;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ButtonHandler : WPFViewHandler<IButton, WButton>
	{
		protected override WButton CreatePlatformView() => new WButton();

		protected override void ConnectHandler(WButton platformView)
		{
			base.ConnectHandler(platformView);
			platformView.Click += OnClick;
		}

		protected override void DisconnectHandler(WButton platformView)
		{
			platformView.Click -= OnClick;
			base.DisconnectHandler(platformView);
		}

		void OnClick(object sender, System.Windows.RoutedEventArgs e)
		{
			VirtualView?.Clicked();
			VirtualView?.Released();
		}

		public static void MapText(ButtonHandler handler, IButton button)
		{
			handler.PlatformView.Content = (button as IText)?.Text ?? string.Empty;
		}

		public static void MapTextColor(ButtonHandler handler, IButton button)
		{
			if (button is ITextStyle ts && ts.TextColor != null)
			{
				var c = ts.TextColor;
				handler.PlatformView.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(c.Alpha * 255),
						(byte)(c.Red * 255), (byte)(c.Green * 255), (byte)(c.Blue * 255)));
			}
		}

		public static void MapFont(ButtonHandler handler, IButton button)
		{
			if (button is ITextStyle textStyle && textStyle.Font.Size > 0)
				handler.PlatformView.FontSize = textStyle.Font.Size;
		}

		public static void MapPadding(ButtonHandler handler, IButton button)
		{
			handler.PlatformView.Padding = new System.Windows.Thickness(
				button.Padding.Left, button.Padding.Top,
				button.Padding.Right, button.Padding.Bottom);
		}
	}
}
