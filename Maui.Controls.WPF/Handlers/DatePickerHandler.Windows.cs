using WDatePicker = System.Windows.Controls.DatePicker;
using WSelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class DatePickerHandler : WPFViewHandler<IDatePicker, WDatePicker>
	{
		protected override WDatePicker CreatePlatformView()
		{
			return new WDatePicker
			{
				MinWidth = 100,
			};
		}

		protected override void ConnectHandler(WDatePicker platformView)
		{
			base.ConnectHandler(platformView);
			platformView.SelectedDateChanged += OnSelectedDateChanged;
		}

		protected override void DisconnectHandler(WDatePicker platformView)
		{
			platformView.SelectedDateChanged -= OnSelectedDateChanged;
			base.DisconnectHandler(platformView);
		}

		void OnSelectedDateChanged(object? sender, WSelectionChangedEventArgs e)
		{
			if (VirtualView == null || PlatformView.SelectedDate == null) return;
			VirtualView.Date = PlatformView.SelectedDate.Value;
		}

		public static void MapDate(DatePickerHandler handler, IDatePicker datePicker)
		{
			handler.PlatformView.SelectedDate = datePicker.Date;
		}

		public static void MapMinimumDate(DatePickerHandler handler, IDatePicker datePicker)
		{
			handler.PlatformView.DisplayDateStart = datePicker.MinimumDate;
		}

		public static void MapMaximumDate(DatePickerHandler handler, IDatePicker datePicker)
		{
			handler.PlatformView.DisplayDateEnd = datePicker.MaximumDate;
		}

		public static void MapFormat(DatePickerHandler handler, IDatePicker datePicker)
		{
		}
	}
}
