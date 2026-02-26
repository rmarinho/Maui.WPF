using System.Windows;
using System.Windows.Media;
using WTextBox = System.Windows.Controls.TextBox;
using WTextChangedEventArgs = System.Windows.Controls.TextChangedEventArgs;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class EditorHandler : WPFViewHandler<IEditor, WTextBox>
	{
		protected override WTextBox CreatePlatformView()
		{
			return new WTextBox
			{
				AcceptsReturn = true,
				TextWrapping = System.Windows.TextWrapping.Wrap,
				VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
				MinHeight = 80
			};
		}

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

		public static void MapText(EditorHandler handler, IEditor editor)
		{
			if (handler.PlatformView.Text != editor.Text)
				handler.PlatformView.Text = editor.Text;
		}

		public static void MapPlaceholder(EditorHandler handler, IEditor editor)
		{
		}

		public static void MapTextColor(EditorHandler handler, IEditor editor)
		{
			if (editor.TextColor != null)
				handler.PlatformView.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(editor.TextColor.Alpha * 255),
						(byte)(editor.TextColor.Red * 255),
						(byte)(editor.TextColor.Green * 255),
						(byte)(editor.TextColor.Blue * 255)));
		}

		public static void MapFont(EditorHandler handler, IEditor editor)
		{
			if (editor is ITextStyle textStyle && textStyle.Font.Size > 0)
				handler.PlatformView.FontSize = textStyle.Font.Size;
		}

		public static void MapIsReadOnly(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView.IsReadOnly = editor.IsReadOnly;
		}
	}
}
