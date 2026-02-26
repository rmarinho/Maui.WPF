namespace Microsoft.Maui.Handlers.WPF
{
	public partial class PickerHandler
	{
		public static IPropertyMapper<IPicker, PickerHandler> Mapper = new PropertyMapper<IPicker, PickerHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IPicker.Title)] = MapTitle,
			[nameof(IPicker.SelectedIndex)] = MapSelectedIndex,
			[nameof(IPicker.TextColor)] = MapTextColor,
			[nameof(IItemDelegate<string>.GetCount)] = MapItems,
		};

		public static CommandMapper<IPicker, PickerHandler> CommandMapper = new(ViewCommandMapper)
		{
		};

		public PickerHandler() : base(Mapper, CommandMapper)
		{
		}

		public PickerHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
		{
		}
	}
}
