#nullable enable

using System.Windows;
using System.Windows.Media;
using WTextBox = System.Windows.Controls.TextBox;
using WTextChangedEventArgs = System.Windows.Controls.TextChangedEventArgs;
using WColor = System.Windows.Media.Color;
using WBrush = System.Windows.Media.SolidColorBrush;

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

		static WBrush? ToBrush(Graphics.Color? color)
		{
			if (color == null) return null;
			return new WBrush(WColor.FromArgb(
				(byte)(color.Alpha * 255), (byte)(color.Red * 255),
				(byte)(color.Green * 255), (byte)(color.Blue * 255)));
		}

		public static void MapText(EntryHandler handler, IEntry entry)
		{
			if (handler.PlatformView.Text != entry.Text)
				handler.PlatformView.Text = entry.Text;
		}

		public static void MapTextColor(EntryHandler handler, IEntry entry)
		{
			var brush = ToBrush(entry.TextColor);
			if (brush != null)
				handler.PlatformView.Foreground = brush;
		}

		public static void MapFont(EntryHandler handler, IEntry entry)
		{
			var font = entry.Font;

			if (font.Size > 0)
				handler.PlatformView.FontSize = font.Size;

			handler.PlatformView.FontWeight = font.Weight >= FontWeight.Bold
				? System.Windows.FontWeights.Bold
				: System.Windows.FontWeights.Normal;

			handler.PlatformView.FontStyle =
				(font.Slant == FontSlant.Italic || font.Slant == FontSlant.Oblique)
					? FontStyles.Italic
					: FontStyles.Normal;

			if (!string.IsNullOrEmpty(font.Family))
				handler.PlatformView.FontFamily = new FontFamily(font.Family);
		}

		public static void MapPlaceholder(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't have a native placeholder property.
		}

		public static void MapPlaceholderColor(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't have a native placeholder property.
		}

		public static void MapIsPassword(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't support password masking; a PasswordBox would be needed.
		}

		public static void MapIsReadOnly(EntryHandler handler, IEntry entry)
		{
			handler.PlatformView.IsReadOnly = entry.IsReadOnly;
		}

		public static void MapMaxLength(EntryHandler handler, IEntry entry)
		{
			handler.PlatformView.MaxLength = entry.MaxLength;
		}

		public static void MapHorizontalTextAlignment(EntryHandler handler, IEntry entry)
		{
			handler.PlatformView.TextAlignment = entry.HorizontalTextAlignment switch
			{
				TextAlignment.Center => System.Windows.TextAlignment.Center,
				TextAlignment.End => System.Windows.TextAlignment.Right,
				_ => System.Windows.TextAlignment.Left,
			};
		}

		public static void MapReturnType(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't have a ReturnType concept.
		}

		public static void MapClearButtonVisibility(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't have a built-in clear button.
		}

		public static void MapIsTextPredictionEnabled(EntryHandler handler, IEntry entry)
		{
			handler.PlatformView.AutoWordSelection = entry.IsTextPredictionEnabled;
		}

		public static void MapKeyboard(EntryHandler handler, IEntry entry)
		{
			// WPF desktop doesn't use software keyboards.
		}

		public static void MapCharacterSpacing(EntryHandler handler, IEntry entry)
		{
			// WPF TextBox doesn't have a direct CharacterSpacing property.
		}

		public static void MapBackground(EntryHandler handler, IEntry entry)
		{
			if (entry.Background is SolidPaint solidPaint)
			{
				var brush = ToBrush(solidPaint.Color);
				if (brush != null)
					handler.PlatformView.Background = brush;
			}
		}
	}
}
