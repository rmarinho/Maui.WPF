using System.Windows;
using System.Windows.Media;
using WTextBox = System.Windows.Controls.TextBox;
using WTextChangedEventArgs = System.Windows.Controls.TextChangedEventArgs;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class EntryHandler : WPFViewHandler<IEntry, WTextBox>
	{
		protected override WTextBox CreatePlatformView() => new WTextBox();

		protected override void ConnectHandler(WTextBox platformView)
		{
			base.ConnectHandler(platformView);
			platformView.TextChanged += OnTextChanged;
		}

		protected override void DisconnectHandler(WTextBox platformView)
		{
			platformView.TextChanged -= OnTextChanged;
			base.DisconnectHandler(platformView);
		}

		void OnTextChanged(object sender, WTextChangedEventArgs e)
		{
			if (VirtualView == null) return;
			VirtualView.Text = PlatformView.Text;
		}

		public static void MapText(EntryHandler handler, IEntry entry)
		{
			if (handler.PlatformView.Text != entry.Text)
				handler.PlatformView.Text = entry.Text;
		}

		public static void MapPlaceholder(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't have native placeholder; use a Tag-based approach or watermark
		}

		public static void MapTextColor(EntryHandler handler, IEntry entry)
		{
			if (entry.TextColor != null)
				handler.PlatformView.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(entry.TextColor.Alpha * 255),
						(byte)(entry.TextColor.Red * 255),
						(byte)(entry.TextColor.Green * 255),
						(byte)(entry.TextColor.Blue * 255)));
		}

		public static void MapFont(EntryHandler handler, IEntry entry)
		{
			if (entry is ITextStyle textStyle && textStyle.Font.Size > 0)
				handler.PlatformView.FontSize = textStyle.Font.Size;
		}

		public static void MapIsPassword(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't support password mode; would need PasswordBox
			// For simplicity, we keep TextBox
		}

		public static void MapIsReadOnly(EntryHandler handler, IEntry entry)
		{
			handler.PlatformView.IsReadOnly = entry.IsReadOnly;
		}

		public static void MapHorizontalTextAlignment(EntryHandler handler, IEntry entry)
		{
			handler.PlatformView.TextAlignment = entry.HorizontalTextAlignment switch
			{
				Microsoft.Maui.TextAlignment.Center => System.Windows.TextAlignment.Center,
				Microsoft.Maui.TextAlignment.End => System.Windows.TextAlignment.Right,
				_ => System.Windows.TextAlignment.Left
			};
		}
	}
}
