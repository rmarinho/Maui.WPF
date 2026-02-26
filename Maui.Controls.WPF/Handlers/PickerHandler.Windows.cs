using System.Windows.Media;
using WComboBox = System.Windows.Controls.ComboBox;
using WSelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class PickerHandler : WPFViewHandler<IPicker, WComboBox>
	{
		protected override WComboBox CreatePlatformView()
		{
			return new WComboBox
			{
				MinWidth = 100,
			};
		}

		protected override void ConnectHandler(WComboBox platformView)
		{
			base.ConnectHandler(platformView);
			platformView.SelectionChanged += OnSelectionChanged;
		}

		protected override void DisconnectHandler(WComboBox platformView)
		{
			platformView.SelectionChanged -= OnSelectionChanged;
			base.DisconnectHandler(platformView);
		}

		void OnSelectionChanged(object sender, WSelectionChangedEventArgs e)
		{
			if (VirtualView == null) return;
			VirtualView.SelectedIndex = PlatformView.SelectedIndex;
		}

		public static void MapTitle(PickerHandler handler, IPicker picker)
		{
		}

		public static void MapSelectedIndex(PickerHandler handler, IPicker picker)
		{
			if (handler.PlatformView.SelectedIndex != picker.SelectedIndex)
				handler.PlatformView.SelectedIndex = picker.SelectedIndex;
		}

		public static void MapTextColor(PickerHandler handler, IPicker picker)
		{
			if (picker.TextColor != null)
				handler.PlatformView.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(picker.TextColor.Alpha * 255),
						(byte)(picker.TextColor.Red * 255),
						(byte)(picker.TextColor.Green * 255),
						(byte)(picker.TextColor.Blue * 255)));
		}

		public static void MapItems(PickerHandler handler, IPicker picker)
		{
			handler.PlatformView.Items.Clear();
			var count = picker.GetCount();
			for (int i = 0; i < count; i++)
			{
				handler.PlatformView.Items.Add(picker.GetItem(i));
			}
		}
	}
}
