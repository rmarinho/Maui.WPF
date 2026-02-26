namespace Microsoft.Maui.Handlers.WPF
{
	public partial class EntryHandler
	{
		public static IPropertyMapper<IEntry, EntryHandler> Mapper = new PropertyMapper<IEntry, EntryHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IEntry.Text)] = MapText,
			[nameof(IEntry.Placeholder)] = MapPlaceholder,
			[nameof(ITextStyle.TextColor)] = MapTextColor,
			[nameof(ITextStyle.Font)] = MapFont,
			[nameof(IEntry.IsPassword)] = MapIsPassword,
			[nameof(IEntry.IsReadOnly)] = MapIsReadOnly,
			[nameof(ITextAlignment.HorizontalTextAlignment)] = MapHorizontalTextAlignment,
		};

		public static CommandMapper<IEntry, EntryHandler> CommandMapper = new(ViewCommandMapper)
		{
		};

		public EntryHandler() : base(Mapper, CommandMapper)
		{
		}

		public EntryHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
		{
		}
	}
}
